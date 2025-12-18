using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GetVideo : MonoBehaviour
{
	public string host = "10.63.119.82";
	public int port = 12345;
	public Material display;
	public RawImage fishEye;
	public bool isConverting;
    
	private UdpClient udpClient;
	private byte[] frameBuffer = new byte[5 * 1024 * 1024];
	private int framePos = 0;
	private bool foundHeader = false;
	public FishEyeCon fishEyeCon;
    
	private Queue<byte[]> queue = new Queue<byte[]>();
	private object queueLock = new object();
    
	private Texture2D inputTexture;
	private Texture2D outputTexture;
    
	// For debugging
	private int frameCount = 0;
	private Material materialInstance;
	public bool isConnected = false;
	
	private bool threadStarted = false;
    
	void Start()
	{
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
        
		inputTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
		outputTexture = null;
        
		try
		{
			udpClient = new UdpClient(port);
			udpClient.Client.ReceiveBufferSize = 5 * 1024 * 1024;
			Debug.Log($"{gameObject.name}: Listening on port {port}, waiting for discovery...");
		}
			catch (Exception e)
			{
				Debug.LogError($"{gameObject.name}: Failed to bind to port {port}: {e.Message}");
				return;
			}
        
	}
	
	void Update()
	{
		if (isConnected && !threadStarted)
		{
			threadStarted = true;
			Debug.Log($"{gameObject.name}: Discovery complete, starting receive thread");
			Thread t = new Thread(ReceiveLoop);
			t.IsBackground = true;
			t.Start();
		}
		
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
	
    
	void ReceiveLoop()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
		IPAddress allowedHost = null; // NEW: cache parsed host IP
	
		try
		{
			while (isConnected)
			{
				byte[] packet = udpClient.Receive(ref remoteEP);
			
				// NEW: Filter by discovered host
				if (allowedHost == null)
				{
					IPAddress.TryParse(host, out allowedHost);
				}
			
				if (allowedHost != null && !remoteEP.Address.Equals(allowedHost))
				{
					continue; // Ignore packets from other IPs
				}
			
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
								if (queue.Count > 3)
								{
									queue.Clear();
								}
								//StartCoroutine(SaveBytes(jpegData));
								queue.Enqueue(jpegData);
							}
						}
					}
				}
			}
		}
			catch (Exception e)
			{
				if (isConnected)
				{
					//Debug.LogError($"{gameObject.name} UDP Receive Error: {e.Message}");
				}
			}
	}
	void ApplyFrame(byte[] data)
	{
		frameCount++;
    
		if (inputTexture.LoadImage(data))
		{
			//Texture outputTexture;  // Changed from Texture2D
		
			//if(isConverting)
			//{
			//	outputTexture = fishEyeCon.Convert(inputTexture);  // No second parameter
			//}
			//else 
			//{
			//	outputTexture = inputTexture;
			//}
        
			//if (materialInstance != null)
			//{
			//	materialInstance.SetTexture("_BaseMap", outputTexture);
			//}
			//if(fishEye != null)
			//{
			//	fishEye.texture = outputTexture;
			//}
			outputTexture = inputTexture;
		}
		else
		{
			Debug.LogWarning($"{gameObject.name}: Bad JPEG frame");
		}
	}
	
	//for testing only
	public IEnumerator SaveBytes(byte[] data)
	{

		var filePath = Path.Combine(Application.persistentDataPath, "MyBinaryFile.bin");

		try
		{
			File.WriteAllBytes(filePath, data);
			Debug.LogWarning($"File saved to: {filePath}");
		}
			catch (Exception e)
			{
				Debug.LogError($"Something went wrong while writing a file: {e}");
			}
		yield return null;

	}
	//
    
	void OnDestroy()
	{
		isConnected = false; // NEW: stop receive thread
		
		if (inputTexture != null)
			DestroyImmediate(inputTexture);
		if (outputTexture != null)
			DestroyImmediate(outputTexture);
		if (materialInstance != null)
			DestroyImmediate(materialInstance);
	}
    
	void OnApplicationQuit()
	{
		isConnected = false; // NEW: stop receive thread
		udpClient?.Close();
	}
}