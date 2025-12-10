using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;

public class GetVideo : MonoBehaviour
{
	public string host = "10.224.123.82";
	public int port = 12345;

	//public RawImage fish;
	public Material display;

	private UdpClient udpClient;

	private byte[] frameBuffer = new byte[5 * 1024 * 1024];
	private int framePos = 0;
	private bool foundHeader = false;

	private FishEyeCon fishEyeCon;

	void Start()
	{
		fishEyeCon = GetComponentInChildren<FishEyeCon>();

		udpClient = new UdpClient(port);   // bind to listen port
		udpClient.Client.ReceiveBufferSize = 5 * 1024 * 1024;

		new Thread(ReceiveLoop).Start();
	}

	void ReceiveLoop()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

		try
		{
			while (true)
			{
				byte[] packet = udpClient.Receive(ref remoteEP);

				// Process the entire UDP packet
				for (int i = 0; i < packet.Length; i++)
				{
					byte b = packet[i];

					if (!foundHeader)
					{
						// Search for JPEG Start FFD8
						if (i > 0 && packet[i - 1] == 0xFF && b == 0xD8)
						{
							foundHeader = true;
							framePos = 0;
							frameBuffer[framePos++] = 0xFF;
							frameBuffer[framePos++] = 0xD8;
						}
					}
					else
					{
						frameBuffer[framePos++] = b;

						// Detect JPEG End FFD9
						if (framePos > 2 &&
							frameBuffer[framePos - 2] == 0xFF &&
							frameBuffer[framePos - 1] == 0xD9)
						{
							byte[] jpegData = new byte[framePos];
							Array.Copy(frameBuffer, jpegData, framePos);

							foundHeader = false;
							framePos = 0;

							UpdateFrame(jpegData);
						}
					}
				}
			}
		}
			catch (Exception e)
			{
				Debug.LogError("UDP Receive Error: " + e.Message);
			}
	}

	void UpdateFrame(byte[] data)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);

			if (tex.LoadImage(data))
			{
				Texture2D converted = fishEyeCon.Convert(tex);
				display.SetTexture("_BaseMap", converted);			
			}
			else
			{
				Debug.LogWarning("Bad JPEG frame");
			}
		});
	}

	void OnApplicationQuit()
	{
		udpClient?.Close();
	}
}
