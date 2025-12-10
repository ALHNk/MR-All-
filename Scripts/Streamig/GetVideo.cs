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

    void Start()
    {
	    //fishEyeCon = GetComponentInChildren<FishEyeCon>();
	    //display = Instantiate(display);

        udpClient = new UdpClient(port);
        udpClient.Client.ReceiveBufferSize = 5 * 1024 * 1024;

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
                                queue.Enqueue(jpegData);
                            }
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
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
