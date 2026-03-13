using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPHeartbeat : MonoBehaviour
{
	public int tcpPort = 12345;
	public bool isConnected = false;

	private TcpListener listener;
	private TcpClient tcpClient;
	private NetworkStream stream;
	private Thread listenThread;
	private Thread readThread;
	private bool isRunning = false;

	void Start()
	{
		isRunning = true;
		listenThread = new Thread(ListenLoop);
		listenThread.IsBackground = true;
		listenThread.Start();
	}

	private void ListenLoop()
	{
		while (isRunning)
		{
			try
			{
				listener = new TcpListener(IPAddress.Any, tcpPort);
				listener.Start();
				Debug.Log($"TCP listening on port {tcpPort}...");

				tcpClient = listener.AcceptTcpClient(); // blocks until Jetson connects
				listener.Stop();

				stream = tcpClient.GetStream();
				isConnected = true;
				Debug.Log("Jetson TCP connected");

				// Read heartbeats - if this dies, Jetson disconnected
				ReadLoop();

				isConnected = false;
				Debug.Log("Jetson disconnected, re-listening...");
			}
				catch (Exception e)
				{
					if (isRunning) Debug.LogError($"TCP listen error: {e.Message}");
					isConnected = false;
					try { listener?.Stop(); } catch {}
					Thread.Sleep(1000);
				}
		}
	}

	private void ReadLoop()
	{
		byte[] buf = new byte[1];
		try
		{
			while (isRunning)
			{
				int r = stream.Read(buf, 0, 1);
				if (r <= 0) break; // disconnected
			}
		}
			catch { /* disconnected */ }
			finally
		{
			stream?.Close();
			tcpClient?.Close();
			stream = null;
			tcpClient = null;
		}
	}

	void OnDestroy() => Disconnect();
	void OnApplicationQuit() => Disconnect();
	void OnApplicationPause(bool pause) { if (pause) Disconnect(); }

	public void Disconnect()
	{
		isRunning = false;
		isConnected = false;
		try { stream?.Close(); } catch {}
		try { tcpClient?.Close(); } catch {}
		try { listener?.Stop(); } catch {}
		listenThread?.Join(1000);
	}
}