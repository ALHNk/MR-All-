using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;

public class TCPHeartbeat : MonoBehaviour
{
	private TcpClient tcpClient;
	private NetworkStream stream;
	private Thread heartbeatThread;
	private bool isRunning = false;
    
	public string serverIP = ""; 
	public int tcpPort = 12345;

	void Start()
	{
		ConnectToServer();
	}
	public void SetIP(string ip)
	{
		serverIP = ip;
	}

	public void ConnectToServer()
	{
		try
		{            
			tcpClient = new TcpClient();
			tcpClient.Connect(serverIP, tcpPort);
			stream = tcpClient.GetStream();
                        
			isRunning = true;
			heartbeatThread = new Thread(HeartbeatLoop);
			heartbeatThread.IsBackground = true;
			heartbeatThread.Start();
		}
			catch (Exception e)
			{
				Debug.LogError($"Failed to connect to TCP server: {e.Message}");
			}
	}

	private void HeartbeatLoop()
	{
		byte[] heartbeat = new byte[1] { 0xFF }; 
        
		while (isRunning)
		{
			try
			{
				if (stream != null && stream.CanWrite)
				{
					stream.Write(heartbeat, 0, 1);
					stream.Flush();
				}
                
				Thread.Sleep(1000);
			}
				catch (Exception e)
				{
					Debug.LogError($"Heartbeat error: {e.Message}");
					break;
				}
		}
	}

	void OnApplicationQuit()
	{
		Disconnect();
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Disconnect();
		}
	}

	void OnDestroy()
	{
		Disconnect();
	}

	public void Disconnect()
	{
        
		isRunning = false;
        
		if (heartbeatThread != null && heartbeatThread.IsAlive)
		{
			heartbeatThread.Join(1000); 
		}
        
		if (stream != null)
		{
			stream.Close();
			stream = null;
		}
        
		if (tcpClient != null)
		{
			tcpClient.Close();
			tcpClient = null;
		}
        
	}
}