using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;

public class GetVideo : MonoBehaviour
{
	public string host = "10.224.123.82";
	public int port = 12345;
	//public RawImage[] displays;
	public RawImage fish;
	public RawImage display;

	private TcpClient client;
	private NetworkStream stream;
	private byte[] buffer = new byte[4096];
	private byte[] frameBuffer = new byte[5 * 1024 * 1024];
	private int framePos = 0;
	private bool foundHeader = false;
	
	private FishEyeCon fishEyeCon;

	void Start()
	{
		new Thread(ReceiveLoop).Start();
		fishEyeCon = FindObjectOfType<FishEyeCon>();
	}

	void ReceiveLoop()
	{
		try
		{
			client = new TcpClient(host, port);
			stream = client.GetStream();
			Debug.Log("Connected to stream");

			while (true)
			{
				int len = stream.Read(buffer, 0, buffer.Length);
				for (int i = 0; i < len; i++)
				{
					byte b = buffer[i];

					if (!foundHeader)
					{
						// ищем начало JPEG (FFD8)
						if (i > 0 && buffer[i - 1] == 0xFF && b == 0xD8)
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
						// ищем конец JPEG (FFD9)
						if (framePos > 2 && frameBuffer[framePos - 2] == 0xFF && frameBuffer[framePos - 1] == 0xD9)
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
				Debug.LogError(e);
			}
	}

	void UpdateFrame(byte[] data)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
			if (tex.LoadImage(data))
			{	
				fish.texture = tex;
				
				Texture2D converted = fishEyeCon.Convert(tex);
				display.texture = converted;
				
				//Texture2D[] parts = fishEyeCon.SplitInto3(tex);
				//for(int i = 0; i < parts.Length; i++)
				//{
				//	Texture2D converted = fishEyeCon.Convert(parts[i]);
				//	displays[i].texture = converted;
				//}
			}
				
			else
			{
				Debug.LogWarning("Bad JPEG frame");
			}
				
		});
	}

	void OnApplicationQuit()
	{
		stream?.Close();
		client?.Close();
	}
}
