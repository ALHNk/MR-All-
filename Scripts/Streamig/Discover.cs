using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class Discover : MonoBehaviour
{
	public GetVideo[] streamers;
	public int discoveryPort = 5000;
	public float discoveryTimeout = 5f;
	public float retryDelay = 2f;
	public string jetsonIP = "10.201.51.4";
    
	private UdpClient discoveryClient;
	public MotorSending motor;
	public TCPHeartbeat heart;
	private bool isConnected = false;
	
	public Image connectImage;
	public TMPro.TMP_Text connectText;

	void Start()
	{
		if (motor == null)
			motor = FindObjectOfType<MotorSending>();
	}

	public void DiscoverStart()
	{
		if (!isConnected)
		{
			StartCoroutine(DiscoverAndConnect());
		}
			
		else
		{
			CloseAllConnections();
		}
	}

	IEnumerator DiscoverAndConnect()
	{
		discoveryClient = new UdpClient();
		byte[] discoveryData = Encoding.ASCII.GetBytes("DISCOVER");
		IPEndPoint jetsonEP = new IPEndPoint(IPAddress.Parse(jetsonIP), discoveryPort);
		discoveryClient.Send(discoveryData, discoveryData.Length, jetsonEP);
		Debug.Log($"Discover: Sent DISCOVER to {jetsonIP}:{discoveryPort}");

		float elapsed = 0f;
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
		var asyncResult = discoveryClient.BeginReceive(null, null);

		while (!asyncResult.IsCompleted && elapsed < discoveryTimeout)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		if (asyncResult.IsCompleted)
		{
			byte[] response = discoveryClient.EndReceive(asyncResult, ref remoteEP);
			string ackMessage = Encoding.ASCII.GetString(response);

			if (ackMessage == "ACK")
			{
				Debug.Log($"Discover: ACK from {remoteEP.Address}");
				string discoveredHost = remoteEP.Address.ToString();

				// Set up video streamers
				foreach (GetVideo streamer in streamers)
				{
					if (streamer != null)
					{
						streamer.host = discoveredHost;
						streamer.ConnectUDP();
						streamer.isConnected = true;
					}
				}

				// Set motor IP and connect
				motor.SetDiscoveredIp(discoveredHost);
				motor.Connect();

				// TCP heartbeat: Quest listens, Jetson will connect to us
				// No need to set IP or call ConnectToServer anymore
				// TCPHeartbeat is already listening from its Start()

				isConnected = true;
				connectImage.color = Color.green;
				connectText.text = "Coned";
			}
			else
			{
				Debug.LogWarning($"Discover: Unexpected response: {ackMessage}");
				discoveryClient?.Close();
				discoveryClient = null;
				yield return new WaitForSeconds(retryDelay);
				StartCoroutine(DiscoverAndConnect());
				yield break;
			}
		}
		else
		{
			Debug.LogWarning("Discover: Timeout, retrying...");
			discoveryClient?.Close();
			discoveryClient = null;
			yield return new WaitForSeconds(retryDelay);
			StartCoroutine(DiscoverAndConnect());
			yield break;
		}

		discoveryClient?.Close();
		discoveryClient = null;
	}
	
	public void CloseAllConnections()
	{
		foreach (GetVideo streamer in streamers)
		{
			if (streamer != null)
			{
				streamer.Disconnect();
			}
		}
		motor.DisconnectMotors();
		discoveryClient?.Close();
		heart.Disconnect();
		isConnected = false;
		connectImage.color = Color.red;
		connectText.text = "Disconed";
	}

	void OnDestroy()
	{
		discoveryClient?.Close();
	}

	void OnApplicationQuit()
	{
		discoveryClient?.Close();
	}
}