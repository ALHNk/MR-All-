using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GetVideoMetrics : MonoBehaviour
{
	public string host = "10.63.119.82";
	public int port = 12345;
	public Material display;
	public RawImage fishEye;
	public bool isConverting;
    
	private UdpClient udpClient;
	public FishEyeCon fishEyeCon;
    
	private Queue<byte[]> queue = new Queue<byte[]>();
	private object queueLock = new object();
    
	private Texture2D inputTexture;
	private Texture2D outputTexture;
    
	private int frameCount = 0;
	private Material materialInstance;
	public bool isConnected = false;
	private bool threadStarted = false;

	// ── Metrics ──────────────────────────────────────────────────────────
	private int framesThisSecond = 0;
	private float fpsTimer = 0f;
	private float currentFPS = 0f;

	private long totalBytesReceived = 0;
	private int totalFramesDecoded = 0;
	private int totalFramesDropped = 0;   // incomplete frames discarded
	private int totalQueueDrops = 0;      // times queue was cleared

	private float minFrameSize = float.MaxValue;
	private float maxFrameSize = 0f;
	private double avgFrameSize = 0;      // running average

	// Per-frame reassembly timing
	private Dictionary<uint, long> frameFirstChunkTime = new Dictionary<uint, long>();

	private StreamWriter logWriter;
	private object logLock = new object();
	private string logPath;
	// ─────────────────────────────────────────────────────────────────────

	private Dictionary<uint, Dictionary<ushort, byte[]>> frameChunks
		= new Dictionary<uint, Dictionary<ushort, byte[]>>();
	private Dictionary<uint, ushort> frameTotalChunks
		= new Dictionary<uint, ushort>();

	void Start()
	{
		if (display != null)
		{
			materialInstance = Instantiate(display);
			GetComponent<Renderer>().material = materialInstance;
		}
		else
		{
			Debug.LogError($"{gameObject.name}: Display material not assigned!");
		}

		inputTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);

		try
		{
			udpClient = new UdpClient(port);
			udpClient.Client.ReceiveBufferSize = 5 * 1024 * 1024;
		}
			catch (Exception e)
			{
				Debug.LogError($"{gameObject.name}: Failed to bind to port {port}: {e.Message}");
				return;
			}

			// Open log file
		logPath = Path.Combine(Application.persistentDataPath,
			$"stream_log_{gameObject.name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
		try
		{
			logWriter = new StreamWriter(logPath, append: false);
			logWriter.WriteLine("timestamp,event,fps,frame_size_bytes,reassembly_ms,queue_drops,frames_dropped,avg_frame_size");
			logWriter.Flush();
			Debug.Log($"{gameObject.name}: Logging to {logPath}");
		}
			catch (Exception e)
			{
				Debug.LogError($"{gameObject.name}: Failed to open log: {e.Message}");
			}
	}

	void Update()
	{
		if (isConnected && !threadStarted)
		{
			threadStarted = true;
			Thread t = new Thread(ReceiveLoop);
			t.IsBackground = true;
			t.Start();
		}

		// FPS counter (counts frames applied to texture, i.e. real video FPS)
		fpsTimer += Time.unscaledDeltaTime;
		if (fpsTimer >= 1f)
		{
			currentFPS = framesThisSecond / fpsTimer;
			framesThisSecond = 0;
			fpsTimer = 0f;

			// Log FPS every second
			WriteLog("fps_tick", currentFPS, 0, 0);
			Debug.Log($"{gameObject.name}: FPS={currentFPS:F1} AvgSize={avgFrameSize:F0}B Dropped={totalFramesDropped} QDrops={totalQueueDrops}");
		}

		byte[] frame = null;
		lock (queueLock)
		{
			if (queue.Count > 0)
				frame = queue.Dequeue();
		}

		if (frame != null)
			ApplyFrame(frame);
	}

	void ReceiveLoop()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
		IPAddress.TryParse(host, out IPAddress allowedHost);

		while (isConnected)
		{
			try
			{
				byte[] packet = udpClient.Receive(ref remoteEP);
				if (allowedHost != null && !remoteEP.Address.Equals(allowedHost)) continue;
				if (packet.Length < 8) continue;

				uint frameId = (uint)IPAddress.NetworkToHostOrder(
					(int)BitConverter.ToUInt32(packet, 0));
				ushort chunkIndex = (ushort)IPAddress.NetworkToHostOrder(
					(short)BitConverter.ToUInt16(packet, 4));
				ushort totalChunks = (ushort)IPAddress.NetworkToHostOrder(
					(short)BitConverter.ToUInt16(packet, 6));

				byte[] chunkData = new byte[packet.Length - 8];
				Array.Copy(packet, 8, chunkData, 0, chunkData.Length);

				if (!frameChunks.ContainsKey(frameId))
				{
					frameChunks[frameId] = new Dictionary<ushort, byte[]>();
					frameTotalChunks[frameId] = totalChunks;
					frameFirstChunkTime[frameId] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
				}

				frameChunks[frameId][chunkIndex] = chunkData;

				if (frameChunks[frameId].Count == totalChunks)
				{
					// Measure reassembly time
					long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
					long reassemblyMs = now - (frameFirstChunkTime.ContainsKey(frameId)
						? frameFirstChunkTime[frameId] : now);
					frameFirstChunkTime.Remove(frameId);

					// Reassemble
					int totalSize = 0;
					for (ushort i = 0; i < totalChunks; i++)
						totalSize += frameChunks[frameId][i].Length;

					byte[] jpeg = new byte[totalSize];
					int pos = 0;
					for (ushort i = 0; i < totalChunks; i++)
					{
						Array.Copy(frameChunks[frameId][i], 0, jpeg, pos,
							frameChunks[frameId][i].Length);
						pos += frameChunks[frameId][i].Length;
					}

					frameChunks.Remove(frameId);
					frameTotalChunks.Remove(frameId);

					// Update size metrics
					totalBytesReceived += totalSize;
					totalFramesDecoded++;
					if (totalSize < minFrameSize) minFrameSize = totalSize;
					if (totalSize > maxFrameSize) maxFrameSize = totalSize;
					avgFrameSize += (totalSize - avgFrameSize) / totalFramesDecoded; // running avg

					// Log this frame
					WriteLog("frame", currentFPS, totalSize, reassemblyMs);

					// Clean up old incomplete frames
					var oldFrames = new List<uint>();
					foreach (var k in frameChunks.Keys)
					{
						if (k < frameId - 10)
						{
							oldFrames.Add(k);
							totalFramesDropped++;
						}
					}
					foreach (var k in oldFrames)
					{
						frameChunks.Remove(k);
						frameTotalChunks.Remove(k);
						frameFirstChunkTime.Remove(k);
					}

					lock (queueLock)
					{
						if (queue.Count > 3)
						{
							queue.Clear();
							totalQueueDrops++;
						}
						queue.Enqueue(jpeg);
					}
				}
			}
				catch (Exception e)
				{
					if (isConnected) Debug.LogError($"UDP error: {e.Message}");
				}
		}
	}

	void ApplyFrame(byte[] data)
	{
		frameCount++;
		framesThisSecond++;  // for FPS counter

		if (inputTexture.LoadImage(data))
		{
			if (materialInstance != null)
				materialInstance.SetTexture("_BaseMap", inputTexture);
		}
		else
		{
			Debug.LogWarning($"{gameObject.name}: Bad JPEG frame");
		}
	}

	private void WriteLog(string eventName, float fps, long frameSize, long reassemblyMs)
	{
		if (logWriter == null) return;
		lock (logLock)
		{
			try
			{
				logWriter.WriteLine(
					$"{DateTime.Now:HH:mm:ss.fff}," +
					$"{eventName}," +
					$"{fps:F1}," +
					$"{frameSize}," +
					$"{reassemblyMs}," +
					$"{totalQueueDrops}," +
					$"{totalFramesDropped}," +
					$"{avgFrameSize:F0}");
				logWriter.Flush();
			}
				catch { /* don't crash stream over logging */ }
		}
	}

	// Call this from UI button or on destroy to print summary
	public void PrintSummary()
	{
		string summary =
			$"=== Stream Summary [{gameObject.name}] ===\n" +
			$"Total frames decoded : {totalFramesDecoded}\n" +
			$"Total frames dropped : {totalFramesDropped}\n" +
			$"Total queue drops    : {totalQueueDrops}\n" +
			$"Total data received  : {totalBytesReceived / 1024f / 1024f:F2} MB\n" +
			$"Avg frame size       : {avgFrameSize:F0} bytes\n" +
			$"Min frame size       : {minFrameSize} bytes\n" +
			$"Max frame size       : {maxFrameSize} bytes\n" +
			$"Last FPS             : {currentFPS:F1}\n" +
			$"Log saved to         : {logPath}";

		Debug.Log(summary);
		WriteLog("summary", currentFPS, 0, 0);
	}

	void OnDestroy()
	{
		isConnected = false;
		PrintSummary();
		logWriter?.Close();

		if (inputTexture != null) DestroyImmediate(inputTexture);
		if (outputTexture != null) DestroyImmediate(outputTexture);
		if (materialInstance != null) DestroyImmediate(materialInstance);
	}

	void OnApplicationQuit()
	{
		isConnected = false;
		udpClient?.Close();
		logWriter?.Close();
	}
}