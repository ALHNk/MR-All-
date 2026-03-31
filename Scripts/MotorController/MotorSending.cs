//using UnityEngine;
//using System.Collections;
//using System.Net.Sockets;
//using System.Text;
//using Newtonsoft.Json;
//using TMPro;
//using UnityEngine.UI;
//public class MotorSending : MonoBehaviour
//{
//	private UdpClient udpClient;
//	private TcpClient tcpClient;
//	private NetworkStream stream;
//	public GameObject motorSim;
//	public TMP_InputField ipField;
//	public Text responce;
//	private TouchScreenKeyboard keyboard;
//	[SerializeField] private string SECRET;
	
//	//public int motor;
	
//	public bool isTCP;
//	public bool isConnected = false;
	
//	public VelocityManagement velMan1, velMan2;
	
//	public MoveMotor moveMotor1, moveMotor2;
	
//	private string staticIP = "10.39.129.122";
//	public string discoveredIp = "";
	
//	private bool isTorqueOn = false;
//	public TMP_Text torqueText;
//	public Image torqueImage;
//	public ChangeDegrees degreeApplicator;
	
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//	    if(!isTCP) udpClient = new UdpClient();
//	    if(degreeApplicator == null)
//	    {
//	    	degreeApplicator = FindObjectOfType<ChangeDegrees>();
//	    }
//    }

//	public void OpenKeyBoard()
//	{
//		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
//	}
//	//public void CloseKeyBoard()
//	//{
//	//	keyboard
//	//}
	
//    // Update is called once per frame
//	void Update()
//    {
//	    //if(keyboard != null)
//	    //{
//	    //	ipField.text = keyboard.text;
	    	
//	    //}
    
//    }
    
//	public void SetDiscoveredIp(string ip)
//	{
//		discoveredIp = ip;
//	}
    
//	public void Connect()
//	{
//		StartCoroutine(ConnectTo());
//	}
    
//	public IEnumerator ConnectTo()
//	{
//		if(isConnected)
//		{
//			yield break;
//		}
		
//		string ip = "";
//		if(discoveredIp != "")
//		{
//			ip = discoveredIp;
//		}
//		else if(ipField.text == "") 
//		{
//			ip = staticIP;
//		}
//		else ip = ipField.text;
//		try{
//			tcpClient = new TcpClient(ip, 5050);
//			stream = tcpClient.GetStream();
//			byte[] data = Encoding.ASCII.GetBytes(SECRET);
//			stream.Write(data,0,data.Length);
//			byte[] buffer = new byte[1024];
//			int byteRead = stream.Read(buffer,0,buffer.Length) ;
//			string tempResponce = Encoding.ASCII.GetString(buffer, 0, byteRead);
			
//			var motorStatus = JsonConvert.DeserializeObject<MotorStatus>(tempResponce);
			
//			if (motorStatus.status == "accepted")
//			{
//				isConnected = true;
				
//				if(velMan1 != null)
//				{
//					velMan1.SetVelocityCome(motorStatus.motor1velocity);
//				}
//				if(velMan2 != null)
//				{
//					velMan2.SetVelocityCome(motorStatus.motor2velocity);
//				}
				

//				float m1Pos = motorStatus.motor1position;
//				float m2Pos = motorStatus.motor2position;
//				float m1Low = motorStatus.motor1limitlow;
//				float m1Up = motorStatus.motor1limitup;
//				float m2Low = motorStatus.motor2limitlow;
//				float m2Up = motorStatus.motor2limitup;
				
//				//moveMotor1.StartMotorPosition(m1Pos, m1Low, m1Up);
//				//moveMotor2.StartMotorPosition(m2Pos, m2Low, m2Up);
//				degreeApplicator.SetScrollbarValue(m1Pos);
//				degreeApplicator.SetValueText();
//				isConnected = true;
				
//			}
			
//			//string expeted = "accepted\nvelocity:";
//			//if(tempResponce.StartsWith(expeted))
//			//{
//			//	isConnected = true;
//			//	string velocityString = tempResponce.Substring(expeted.Length).Trim();
//			//	if(float.TryParse(velocityString, out float velocityCome))
//			//	{
//			//		velMan.SetVelocityCome(velocityCome);
//			//	}
//			//}
//			responce.text = tempResponce;
			
			
			
//		}catch (System.Exception e)
//		{
//			responce.text = "Connection error" + e.Message;
//		}
//		yield return null;
//	}
	
    
//	public string TakeSecret()
//	{
//		stream = tcpClient.GetStream();
//		byte[] data = Encoding.ASCII.GetBytes(SECRET);
//		stream.Write(data,0,data.Length);
//		byte[] buffer = new byte[1024];
//		int byteRead = stream.Read(buffer,0,buffer.Length);
//		string secret = Encoding.ASCII.GetString(buffer, 0, byteRead);
		
//		return secret;
//	}
    
//	public void SendValues(float value, string what, int motorId)
//	{
//		if(isTCP) SendValuesTCP(value, what, motorId);
//		else SendValuesUDP(value, what, motorId);
//	}
    
//	private void SendValuesUDP(float value, string what, int motorId)
//	{
//		string msg = "motor:" + motorId +what + ":" + value.ToString("F2") + "\n";
//		byte[] data = Encoding.ASCII.GetBytes(msg);
//		string ip = "";
//		if(ipField.text == "") 
//		{
//			ip = staticIP;
//		}
//		else ip = ipField.text;
		
//		udpClient.Send(data, data.Length, ip, 5000); 
//		//Debug.LogError("nextData is Send: " + msg + " To IP: " + ip);
		
//	}
//	private void SendValuesTCP(float value, string what, int motorId)
//	{
//		if(!isConnected)
//		{
//			return;
//		}
//		string msg = "motor:" + motorId + what + ":" + value.ToString("F2") + "\n";
//		byte[] data = Encoding.ASCII.GetBytes(msg);
//		stream.Write(data,0,data.Length);
		
//	}
//	public void SendValues(int value, string what)
//	{
//		if(!isConnected)
//		{
//			return;
//		}
//		string msg = what + ":" + value.ToString() + "\n";
//		byte[] data = Encoding.ASCII.GetBytes(msg);
//		stream.Write(data,0,data.Length);
		
//	}
//	public void SendValues(int value, string what, string secret)
//	{
//		if(!isConnected)
//		{
//			return;
//		}
//		string msg = what + ":" + value.ToString()+ secret + "\n";
//		byte[] data = Encoding.ASCII.GetBytes(msg);
//		stream.Write(data,0,data.Length);
		
//	}
	
//	public void TorqueSwitch()
//	{
//		if(!isTorqueOn)
//		{
//			SendValues(1, "torque");
//		}
//		else
//		{
//			SendValues(0, "torque");
//		}
//		byte[] buffer = new byte[256];
//		int byteRead = stream.Read(buffer,0,buffer.Length);
//		string torqueStatus = Encoding.ASCII.GetString(buffer, 0, byteRead);
//		if(torqueStatus.Equals("on\n"))
//		{
//			isTorqueOn = true;
//			torqueImage.color = Color.green;
//			torqueText.text = "Torqued on";
//		}
//		else if(torqueStatus.Equals("off\n"))
//		{
//			isTorqueOn = false;
//			torqueImage.color = Color.blue;
//			torqueText.text = "Torqued off";
//		}
//	}
	
//	public void ESTOP()
//	{
//		string msg = "ESTOP";
//		byte[] data = Encoding.ASCII.GetBytes(msg);
//		stream.Write(data, 0, data.Length);
//	}
	
//	// Sent to all game objects before the application is quit.
//	protected void OnApplicationQuit()
//	{
//		udpClient?.Close();
//		tcpClient?.Close();
//		stream?.Close();
//	}
	
//}



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

// ─────────────────────────────────────────────────────────────────────────────
//  MotorSending — fixed version
//
//  Key changes vs. original:
//   1. All stream.Read() calls moved OFF the main thread (AsyncReadLoop).
//      The main thread never blocks waiting for the server.
//   2. TCP framing: incoming bytes are accumulated until a '\n' arrives,
//      so partial packets are handled correctly.
//   3. Dead-band filter: continuous values (speed, san, prot, twodegree, wbr)
//      are only sent when they change by more than a threshold.
//   4. SendValuesTCP uses a lock so writes from Update() don't race with
//      the coroutine / UI thread.
//   5. TorqueSwitch no longer blocks — it sends the command and waits for
//      the server reply asynchronously.
// ─────────────────────────────────────────────────────────────────────────────

public class MotorSending : MonoBehaviour
{
	// ── Inspector fields ───────────────────────────────────────────────────────
	public  GameObject      motorSim;
	public  TMP_InputField  ipField;
	public  Text            responce;
	[SerializeField] private string SECRET;

	public  bool            isTCP        = true;
	public  bool            isConnected  = false;

	public  VelocityManagement velMan1, velMan2;
	public  MoveMotor          moveMotor1, moveMotor2;
	public  TMP_Text           torqueText;
	public  Image              torqueImage;
	public  ChangeDegrees      degreeApplicator;

	// ── Dead-band thresholds — tune to taste ──────────────────────────────────
	private const float DEADBAND_SPEED    = 0.5f;
	private const float DEADBAND_ANGLE    = 0.5f;
	private const float DEADBAND_SAN      = 0.3f;
	private const float DEADBAND_PROT     = 0.3f;
	private const float DEADBAND_2DEGREE  = 0.005f;
	private const float DEADBAND_WBR      = 0.5f;

	// ── Private state ─────────────────────────────────────────────────────────
	private TcpClient     tcpClient;
	private NetworkStream stream;
	private UdpClient     udpClient;

	private bool   isTorqueOn = false;
	private string staticIP   = "10.39.129.122";
	public  string discoveredIp = "";

	// Lock for stream.Write — reads happen on a background thread
	private readonly object writeLock = new object();

	// Thread-safe queue: background reader posts messages; main thread consumes
	private readonly Queue<string> incomingMessages = new Queue<string>();
	private readonly object        queueLock        = new object();

	private CancellationTokenSource readCts;

	// Last-sent value tracking for dead-band filtering
	private float lastSpeed    = float.MaxValue;
	private float lastAngle    = float.MaxValue;
	private float lastSan      = float.MaxValue;
	private float lastProt     = float.MaxValue;
	private float last2Degree  = float.MaxValue;
	private float lastWbr      = float.MaxValue;

	// ─────────────────────────────────────────────────────────────────────────
	//  Unity lifecycle
	// ─────────────────────────────────────────────────────────────────────────

	void Start()
	{
		if (!isTCP)
			udpClient = new UdpClient();

		if (degreeApplicator == null)
			degreeApplicator = FindObjectOfType<ChangeDegrees>();
	}

	void Update()
	{
		// Drain incoming messages on the main thread (safe to touch Unity objects)
		lock (queueLock)
		{
			while (incomingMessages.Count > 0)
				HandleServerMessage(incomingMessages.Dequeue());
		}
	}

	protected void OnApplicationQuit()
	{
		readCts?.Cancel();
		udpClient?.Close();
		stream?.Close();
		tcpClient?.Close();
	}

	// ─────────────────────────────────────────────────────────────────────────
	//  Connection
	// ─────────────────────────────────────────────────────────────────────────

	public void Connect() => StartCoroutine(ConnectTo());

	public IEnumerator ConnectTo()
	{
		if (isConnected) yield break;

		string ip = discoveredIp != "" ? discoveredIp
			: ipField.text  != "" ? ipField.text
			: staticIP;

		// Run blocking TCP connect + auth on a thread pool thread
		Task<string> connectTask = Task.Run(() => BlockingConnect(ip));

		// Wait without blocking Unity's main thread
		while (!connectTask.IsCompleted)
			yield return null;

		if (connectTask.IsFaulted)
		{
			responce.text = "Connection error: " + connectTask.Exception?.GetBaseException().Message;
			yield break;
		}

		string json = connectTask.Result;
		responce.text = json;

		if (string.IsNullOrEmpty(json)) yield break;

		var status = JsonConvert.DeserializeObject<MotorStatus>(json);
		if (status?.status != "accepted") yield break;

		isConnected = true;

		velMan1?.SetVelocityCome(status.motor1velocity);
		velMan2?.SetVelocityCome(status.motor2velocity);

		degreeApplicator?.SetScrollbarValue(status.motor1position);
		degreeApplicator?.SetValueText();

		// Start background read loop
		readCts = new CancellationTokenSource();
		_ = AsyncReadLoop(readCts.Token);
	}

	/// <summary>
	/// Runs on a thread-pool thread — does the blocking connect + auth handshake.
	/// Returns the server's JSON reply, or throws on error.
	/// </summary>
	private string BlockingConnect(string ip)
	{
		tcpClient = new TcpClient();
		tcpClient.NoDelay = true;          // Disable Nagle on the Unity side too
		tcpClient.Connect(ip, 5050);
		stream = tcpClient.GetStream();

		// Send secret
		byte[] secretBytes = Encoding.ASCII.GetBytes(SECRET);
		stream.Write(secretBytes, 0, secretBytes.Length);

		// Read reply (initial status JSON — ends with '}')
		// We use a simple accumulator here too
		byte[] buf = new byte[1024];
		int total = 0;
		while (true)
		{
			int n = stream.Read(buf, total, buf.Length - total);
			if (n <= 0) break;
			total += n;
			// The JSON reply ends with '}'
			string partial = Encoding.ASCII.GetString(buf, 0, total);
			if (partial.TrimEnd().EndsWith("}")) break;
			if (total >= buf.Length) break;
		}

		return Encoding.ASCII.GetString(buf, 0, total);
	}

	// ─────────────────────────────────────────────────────────────────────────
	//  Background read loop  (runs on thread pool, never touches Unity objects)
	// ─────────────────────────────────────────────────────────────────────────

	private async Task AsyncReadLoop(CancellationToken ct)
	{
		var accum = new StringBuilder(512);
		byte[] buf = new byte[512];

		try
		{
			while (!ct.IsCancellationRequested && isConnected)
			{
				int n = await stream.ReadAsync(buf, 0, buf.Length, ct);
				if (n == 0)
				{
					// Server closed connection
					EnqueueMessage("__DISCONNECTED__");
					break;
				}

				accum.Append(Encoding.ASCII.GetString(buf, 0, n));

				// Extract every complete '\n'-terminated line
				string accumulated = accum.ToString();
				int nlIdx;
				while ((nlIdx = accumulated.IndexOf('\n')) >= 0)
				{
					string line = accumulated.Substring(0, nlIdx).TrimEnd('\r');
					accumulated  = accumulated.Substring(nlIdx + 1);
					EnqueueMessage(line);
				}

				accum.Clear();
				accum.Append(accumulated);
			}
		}
			catch (System.Exception e) when (!(e is TaskCanceledException))
			{
				EnqueueMessage("__ERROR__:" + e.Message);
			}
	}

	private void EnqueueMessage(string msg)
	{
		lock (queueLock)
			incomingMessages.Enqueue(msg);
	}

	/// <summary>
	/// Called on the main thread (from Update) for every complete server message.
	/// Safe to touch any Unity object here.
	/// </summary>
	private void HandleServerMessage(string msg)
	{
		if (msg == "__DISCONNECTED__")
		{
			isConnected = false;
			responce.text = "Disconnected";
			return;
		}
		if (msg.StartsWith("__ERROR__:"))
		{
			isConnected = false;
			responce.text = msg;
			return;
		}
		if (msg == "on")
		{
			isTorqueOn = true;
			if (torqueImage) torqueImage.color = Color.green;
			if (torqueText)  torqueText.text   = "Torqued on";
			return;
		}
		if (msg == "off")
		{
			isTorqueOn = false;
			if (torqueImage) torqueImage.color = Color.blue;
			if (torqueText)  torqueText.text   = "Torqued off";
			return;
		}
		if (msg == "estop")
		{
			responce.text = "EMERGENCY STOP";
			return;
		}
		// Add more server→client messages here as needed
	}

	// ─────────────────────────────────────────────────────────────────────────
	//  Sending helpers
	// ─────────────────────────────────────────────────────────────────────────

	public void SetDiscoveredIp(string ip) => discoveredIp = ip;

	public void SendValues(float value, string what, int motorId)
	{
		if (isTCP) SendValuesTCP(value, what, motorId);
		else       SendValuesUDP(value, what, motorId);
	}

	private void SendValuesTCP(float value, string what, int motorId)
	{
		if (!isConnected) return;
		string msg  = "motor:" + motorId + what + ":" + value.ToString("F2") + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		lock (writeLock)
			stream.Write(data, 0, data.Length);
	}

	private void SendValuesUDP(float value, string what, int motorId)
	{
		string ip  = ipField.text != "" ? ipField.text : staticIP;
		string msg = "motor:" + motorId + what + ":" + value.ToString("F2") + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		udpClient.Send(data, data.Length, ip, 5000);
	}

	// Int overloads (torque, lock)
	public void SendValues(int value, string what)
	{
		if (!isConnected) return;
		string msg  = what + ":" + value + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		lock (writeLock)
			stream.Write(data, 0, data.Length);
	}
	public void SendValues(int value, string what, string secret)
	{
		if (!isConnected) return;
		string msg  = what + ":" + value + secret + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		lock (writeLock)
			stream.Write(data, 0, data.Length);
	}

	// ── ESTOP — bypass dead-band, send immediately ────────────────────────────
	public void ESTOP()
	{
		if (!isConnected) return;
		byte[] data = Encoding.ASCII.GetBytes("ESTOP\n");
		lock (writeLock)
			stream.Write(data, 0, data.Length);
	}

	// ── Torque — no longer blocks waiting for reply ───────────────────────────
	public void TorqueSwitch()
	{
		SendValues(isTorqueOn ? 0 : 1, "torque");
		// Reply ("on\n" / "off\n") arrives via AsyncReadLoop → HandleServerMessage
	}

	// ─────────────────────────────────────────────────────────────────────────
	//  Dead-band filtered send methods
	//  Call these from your UI / joystick scripts instead of SendValues directly
	// ─────────────────────────────────────────────────────────────────────────

	public void SendSpeed(float speed, int motorId)
	{
		if (Mathf.Abs(speed - lastSpeed) < DEADBAND_SPEED) return;
		lastSpeed = speed;
		SendValues(speed, "speed:", motorId);
	}

	public void SendAngle(float angle, int motorId)
	{
		if (Mathf.Abs(angle - lastAngle) < DEADBAND_ANGLE) return;
		lastAngle = angle;
		SendValues(angle, "angle:", motorId);
	}

	public void SendSan(float san, int motorId)
	{
		if (Mathf.Abs(san - lastSan) < DEADBAND_SAN) return;
		lastSan = san;
		SendValues(san, "san:", motorId);
	}

	public void SendProt(float prot, int motorId)
	{
		if (Mathf.Abs(prot - lastProt) < DEADBAND_PROT) return;
		lastProt = prot;
		SendValues(prot, "prot:", motorId);
	}

	public void SendTwoDegree(float td, int motorId)
	{
		if (Mathf.Abs(td - last2Degree) < DEADBAND_2DEGREE) return;
		last2Degree = td;
		SendValues(td, "twodegree:", motorId);
	}

	public void SendWbr(float wbr, int motorId)
	{
		if (Mathf.Abs(wbr - lastWbr) < DEADBAND_WBR) return;
		lastWbr = wbr;
		SendValues(wbr, "wbr:", motorId);
	}
}