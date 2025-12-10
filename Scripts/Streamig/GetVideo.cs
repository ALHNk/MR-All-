using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class GetVideo : MonoBehaviour
{
	public string host = "10.224.123.82";
	public int port = 12345;
	public Material display;
    
	private UdpClient udpClient;
	private byte[] frameBuffer = new byte[5 * 1024 * 1024];
	private int framePos = 0;
	private bool foundHeader = false;
	public FishEyeCon fishEyeCon;
    
	private Queue<byte[]> queue = new Queue<byte[]>();
	private object queueLock = new object();
    
	// Reusable textures to avoid memory allocation every frame
	private Texture2D inputTexture;
	private Texture2D outputTexture;
    
	// For debugging
	private int frameCount = 0;
	private Material materialInstance;
    
	void Start()
	{
		// CRITICAL: Create material instance to prevent sharing between objects
		if (display != null)
		{
			materialInstance = Instantiate(display);
			GetComponent<Renderer>().material = materialInstance;
			Debug.Log($"{gameObject.name}: Created material instance");
		}
		else
		{
			Debug.LogError($"{gameObject.name}: Display material not assigned!");
		}
        
		// Create reusable textures
		inputTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
		outputTexture = null;
        
		try
		{
			udpClient = new UdpClient(port);
			udpClient.Client.ReceiveBufferSize = 5 * 1024 * 1024;
			Debug.Log($"{gameObject.name}: Listening on port {port}");
		}
			catch (Exception e)
			{
				Debug.LogError($"{gameObject.name}: Failed to bind to port {port}: {e.Message}");
				return;
			}
        
		Thread t = new Thread(ReceiveLoop);
		t.IsBackground = true;
		t.Start();
	}
    
	void ReceiveLoop()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
		try
		{
			while (true)
			{
				byte[] packet = udpClient.Receive(ref remoteEP);
				for (int i = 0; i < packet.Length; i++)
				{
					byte b = packet[i];
					if (!foundHeader)
					{
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
						if (framePos > 2 &&
							frameBuffer[framePos - 2] == 0xFF &&
							frameBuffer[framePos - 1] == 0xD9)
						{
							byte[] jpegData = new byte[framePos];
							Array.Copy(frameBuffer, jpegData, framePos);
							foundHeader = false;
							framePos = 0;
                            
							lock (queueLock)
							{
								// Keep only the latest frame to avoid queue buildup
								if (queue.Count > 3)
									queue.Clear();
								queue.Enqueue(jpegData);
							}
						}
					}
				}
			}
		}
			catch (Exception e)
			{
				Debug.LogError($"{gameObject.name} UDP Receive Error: {e.Message}");
			}
	}
    
	void Update()
	{
		byte[] frame = null;
		lock (queueLock)
		{
			if (queue.Count > 0)
				frame = queue.Dequeue();
		}
        
		if (frame != null)
		{
			ApplyFrame(frame);
		}
	}
    
	void ApplyFrame(byte[] data)
	{
		frameCount++;
        
		// Reuse the same texture instead of creating new one
		if (inputTexture.LoadImage(data))
		{
			// Convert will handle texture reuse internally
			outputTexture = fishEyeCon.Convert(inputTexture, outputTexture);
            
			// Use material instance, not the original material
			if (materialInstance != null)
			{
				materialInstance.SetTexture("_BaseMap", outputTexture);
			}
            
			// Debug every 60 frames
			if (frameCount % 60 == 0)
			{
				Debug.Log($"{gameObject.name}: Processed {frameCount} frames on port {port}");
			}
		}
		else
		{
			Debug.LogWarning($"{gameObject.name}: Bad JPEG frame");
		}
	}
    
	void OnDestroy()
	{
		if (inputTexture != null)
			DestroyImmediate(inputTexture);
		if (outputTexture != null)
			DestroyImmediate(outputTexture);
		if (materialInstance != null)
			DestroyImmediate(materialInstance);
	}
    
	void OnApplicationQuit()
	{
		udpClient?.Close();
	}
}