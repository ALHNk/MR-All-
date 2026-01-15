using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;

public class Discover : MonoBehaviour
{
	public GetVideo[] streamers;
	public int discoveryPort = 5000;
	public float discoveryTimeout = 5f;
	public float retryDelay = 2f;
	
	private UdpClient discoveryClient;
	private bool isDiscovering = false;
	
	public MotorSending motor;
	public TCPHeartbeat heart;
    
	void Start()
	{
		//StartCoroutine(DiscoverAndConnect());
		if(motor == null)
		{
			motor = FindObjectOfType<MotorSending>();
		}
	}
	
	public void DiscoverStart()
	{
		StartCoroutine(DiscoverAndConnect());
	}
	
	IEnumerator ConnectMotor()
	{
		yield return new WaitForSeconds(0.9f);
		heart.ConnectToServer();
	}
	
	IEnumerator ConnectTCP()
	{
		yield return new WaitForSeconds(0.9f);
		motor.Connect();
	}
	
	IEnumerator DiscoverAndConnect()
	{
		if (streamers == null || streamers.Length == 0)
		{
			Debug.LogError("Discover: No streamers assigned!");
			yield break;
		}
		
		Debug.Log($"Discover: Starting discovery for {streamers.Length} streamers on port {discoveryPort}...");
		
		bool success = false;
		isDiscovering = true;
		
		try
		{
			// Create discovery client
			discoveryClient = new UdpClient();
			discoveryClient.EnableBroadcast = true;
			discoveryClient.Client.ReceiveTimeout = (int)(discoveryTimeout * 1000);
			
			// Send discovery packet
			byte[] discoveryData = Encoding.ASCII.GetBytes("DISCOVER");
			IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
			
			discoveryClient.Send(discoveryData, discoveryData.Length, broadcastEP);
			Debug.Log($"Discover: Sent discovery packet to broadcast:{discoveryPort}");
			
			// Wait for ACK
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
			byte[] response = discoveryClient.Receive(ref remoteEP);
			
			string ackMessage = Encoding.ASCII.GetString(response);
			if (ackMessage == "ACK")
			{
				Debug.Log($"Discover: ACK received from {remoteEP.Address}");
				
				// Update all streamers with discovered host
				string discoveredHost = remoteEP.Address.ToString();
				foreach (GetVideo streamer in streamers)
				{
					if (streamer != null)
					{
						streamer.host = discoveredHost;
						streamer.isConnected = true;
						
					}
				}
				motor.SetDiscoveredIp(discoveredHost);
				//motor.Connect();
				heart.SetIP(discoveredHost);
				
				success = true;
			}
			else
			{
				Debug.LogWarning($"Discover: Unexpected response: {ackMessage}");
			}
		}
			catch (SocketException e)
			{
				Debug.LogError($"Discover: Timeout or socket error: {e.Message}");
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Discover: Failed: {e.Message}");
			}
		finally
		{
			if (discoveryClient != null)
			{
				discoveryClient.Close();
				discoveryClient = null;
			}
			isDiscovering = false;
			StartCoroutine(ConnectTCP());
			StartCoroutine(ConnectMotor());
		}
		
		
		
		// Retry if failed
		if (!success)
		{
			Debug.Log($"Discover: Retrying in {retryDelay} seconds...");
			yield return new WaitForSeconds(retryDelay);
			StartCoroutine(DiscoverAndConnect());
		}
	}
	
	void OnDestroy()
	{
		isDiscovering = false;
		if (discoveryClient != null)
		{
			discoveryClient.Close();
			discoveryClient = null;
		}
	}
	
	void OnApplicationQuit()
	{
		isDiscovering = false;
		discoveryClient?.Close();
	}
}