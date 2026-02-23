//using UnityEngine;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections;
//using System.Text;

//public class Discover : MonoBehaviour
//{
//	public GetVideo[] streamers;
//	public int discoveryPort = 5000;
//	public float discoveryTimeout = 5f;
//	public float retryDelay = 2f;
	
//	private UdpClient discoveryClient;
//	private bool isDiscovering = false;
	
//	public MotorSending motor;
//	public TCPHeartbeat heart;
//	private bool isConnected = false;
    
//	void Start()
//	{
//		//StartCoroutine(DiscoverAndConnect());
//		if(motor == null)
//		{
//			motor = FindObjectOfType<MotorSending>();
//		}
//	}
	
//	public void DiscoverStart()
//	{
//		if(isConnected == false)
//		{
//			StartCoroutine(DiscoverAndConnect());
//		}
		
//	}
	
//	IEnumerator ConnectMotor()
//	{
//		yield return new WaitForSeconds(0.9f);
//		heart.ConnectToServer();
//	}
	
//	IEnumerator ConnectTCP()
//	{
//		yield return new WaitForSeconds(0.9f);
//		motor.Connect();
//	}
	
//	IEnumerator DiscoverAndConnect()
//	{
//		discoveryClient = new UdpClient();
//		discoveryClient.EnableBroadcast = true;

//		byte[] discoveryData = Encoding.ASCII.GetBytes("DISCOVER");
//		IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Parse("10.201.51.255"), discoveryPort);
//		discoveryClient.Send(discoveryData, discoveryData.Length, broadcastEP);
    
//		Debug.Log("Discover: Sent discovery packet");

//		// Non-blocking wait
//		float elapsed = 0f;
//		byte[] response = null;
//		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

//		var asyncResult = discoveryClient.BeginReceive(null, null);
    
//		while (!asyncResult.IsCompleted && elapsed < discoveryTimeout)
//		{
//			elapsed += Time.deltaTime;
//			yield return null;  // yield every frame, no freeze
//		}

//		if (asyncResult.IsCompleted)
//		{
//			response = discoveryClient.EndReceive(asyncResult, ref remoteEP);
//			string ackMessage = Encoding.ASCII.GetString(response);
        
//			if (ackMessage == "ACK")
//			{
//				Debug.Log($"Discover: ACK from {remoteEP.Address}");
//				string discoveredHost = remoteEP.Address.ToString();
            
//				foreach (GetVideo streamer in streamers)
//				{
//					if (streamer != null)
//					{
//						streamer.host = discoveredHost;
//						streamer.isConnected = true;
//					}
//				}
//				motor.SetDiscoveredIp(discoveredHost);
//				heart.SetIP(discoveredHost);
//				isConnected = true;
//			}
//		}
//		else
//		{
//			Debug.LogWarning("Discover: Timeout, retrying...");
//			discoveryClient.Close();
//			discoveryClient = null;
//			yield return new WaitForSeconds(retryDelay);
//			StartCoroutine(DiscoverAndConnect());
//			yield break;
//		}

//		discoveryClient?.Close();
//		discoveryClient = null;
    
//		StartCoroutine(ConnectTCP());
//		StartCoroutine(ConnectMotor());
//	}
	
//	void OnDestroy()
//	{
//		isDiscovering = false;
//		if (discoveryClient != null)
//		{
//			discoveryClient.Close();
//			discoveryClient = null;
//		}
//	}
	
//	void OnApplicationQuit()
//	{
//		isDiscovering = false;
//		discoveryClient?.Close();
//	}
//}





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
	public string jetsonIP = "10.201.51.4"; // <-- direct IP, no broadcast

	private UdpClient discoveryClient;

	public MotorSending motor;
	public TCPHeartbeat heart;
	private bool isConnected = false;

	void Start()
	{
		if (motor == null)
			motor = FindObjectOfType<MotorSending>();
	}

	public void DiscoverStart()
	{
		if (!isConnected)
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

				foreach (GetVideo streamer in streamers)
				{
					if (streamer != null)
					{
						streamer.host = discoveredHost;
						streamer.isConnected = true;
					}
				}
				motor.SetDiscoveredIp(discoveredHost);
				heart.SetIP(discoveredHost);
				isConnected = true;

				StartCoroutine(ConnectTCP());
				StartCoroutine(ConnectMotor());
			}
			else
			{
				Debug.LogWarning($"Discover: Unexpected response: {ackMessage}");
				yield return new WaitForSeconds(retryDelay);
				StartCoroutine(DiscoverAndConnect());
			}
		}
		else
		{
			Debug.LogWarning("Discover: Timeout, retrying...");
			yield return new WaitForSeconds(retryDelay);
			StartCoroutine(DiscoverAndConnect());
		}

		discoveryClient?.Close();
		discoveryClient = null;
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