using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;

public class MotorSending : MonoBehaviour
{
	private UdpClient udpClient;
	private UdpClient _udpControlClient = new UdpClient(5053); // UDP for real-time commands
	private const int UDP_PORT = 5052;

	private TcpClient tcpClient;
	private NetworkStream stream;
	public GameObject motorSim;
	public TMP_InputField ipField;
	public Text responce;
	private TouchScreenKeyboard keyboard;
	[SerializeField] private string SECRET;
	
	//public int motor;
	
	public bool isTCP;
	public bool isConnected = false;
	
	public VelocityManagement velMan1, velMan2;
	
	public MoveMotor moveMotor1, moveMotor2;
	
	private string staticIP = "10.39.129.122";
	public string discoveredIp = "";
	
	private bool isTorqueOn = false;
	public TMP_Text torqueText;
	public Image torqueImage;
	public ChangeDegrees degreeApplicator;
	
	private readonly Queue<string> _messages = new Queue<string>();
	private readonly object _queueLock = new object();
	private Thread _readThread;
	
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if(!isTCP) udpClient = new UdpClient();
		if(degreeApplicator == null)
		{
			degreeApplicator = FindObjectOfType<ChangeDegrees>();
		}
	}

	public void OpenKeyBoard()
	{
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	}
	
    
	public void SetDiscoveredIp(string ip)
	{
		discoveredIp = ip;
	}
    
	public void Connect()
	{
		StartCoroutine(ConnectTo());
	}
    
	public IEnumerator ConnectTo()
	{
		if(isConnected)
		{
			yield break;
		}
		
		string ip = "";
		if(discoveredIp != "")
		{
			ip = discoveredIp;
		}
		else if(ipField.text == "") 
		{
			ip = staticIP;
		}
		else ip = ipField.text;
		try{
			tcpClient = new TcpClient(ip, 5050);
			stream = tcpClient.GetStream();
			byte[] data = Encoding.ASCII.GetBytes(SECRET);
			stream.Write(data,0,data.Length);
			byte[] buffer = new byte[1024];
			int byteRead = stream.Read(buffer,0,buffer.Length) ;
			string tempResponce = Encoding.ASCII.GetString(buffer, 0, byteRead);
			
			var motorStatus = JsonConvert.DeserializeObject<MotorStatus>(tempResponce);
			
			if (motorStatus.status == "accepted")
			{
				isConnected = true;
				_udpControlClient.Connect(ip, UDP_PORT);
				if(velMan1 != null)
				{
					velMan1.SetVelocityCome(motorStatus.motor1velocity);
				}
				if(velMan2 != null)
				{
					velMan2.SetVelocityCome(motorStatus.motor2velocity);
				}
				

				float m1Pos = motorStatus.motor1position;
				float m2Pos = motorStatus.motor2position;
				float m1Low = motorStatus.motor1limitlow;
				float m1Up = motorStatus.motor1limitup;
				float m2Low = motorStatus.motor2limitlow;
				float m2Up = motorStatus.motor2limitup;
				
				//moveMotor1.StartMotorPosition(m1Pos, m1Low, m1Up);
				//moveMotor2.StartMotorPosition(m2Pos, m2Low, m2Up);
				degreeApplicator.SetScrollbarValue(m1Pos);
				degreeApplicator.SetValueText();
				isConnected = true;
				
				_readThread = new Thread(ReadLoop);
				_readThread.IsBackground = true;
				_readThread.Start();
				
			}
			
			responce.text = tempResponce;
			
		}catch (System.Exception e)
		{
			responce.text = "Connection error" + e.Message;
		}
		yield return null;
	}
	
	private void ReadLoop()
	{
		byte[] buffer = new byte[1024];
		while(isConnected)
		{
			int n = stream.Read(buffer, 0, buffer.Length);
			if(n <= 0) break;
			string msg = Encoding.ASCII.GetString(buffer, 0, n).Trim();
			lock(_queueLock)
				_messages.Enqueue(msg);
		}
	}

	void Update()
	{
		lock(_queueLock)
		{
			while(_messages.Count > 0)
			{	
				string msg = _messages.Dequeue();
				if(msg.Equals("on") || msg.Equals("on\n"))
				{
					isTorqueOn = true;
					torqueImage.color = Color.green;
					torqueText.text = "Torqued on";
				}
				else if(msg.Equals("off") || msg.Equals("off\n"))
				{
					isTorqueOn = false;
					torqueImage.color = Color.blue;
					torqueText.text = "Torqued off";
				}
			}
		}
	}
    
	public string TakeSecret()
	{
		stream = tcpClient.GetStream();
		byte[] data = Encoding.ASCII.GetBytes(SECRET);
		stream.Write(data,0,data.Length);
		byte[] buffer = new byte[1024];
		int byteRead = stream.Read(buffer,0,buffer.Length);
		string secret = Encoding.ASCII.GetString(buffer, 0, byteRead);
		
		return secret;
	}
    
	public void SendValues(float value, string what, int motorId)
	{
		if(isTCP) SendValuesTCP(value, what, motorId);
		else SendValuesUDP(value, what, motorId);
	}
    
	private void SendValuesUDP(float value, string what, int motorId)
	{
		string msg = "motor:" + motorId +what + ":" + value.ToString("F2") + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		string ip = "";
		if(ipField.text == "") 
		{
			ip = staticIP;
		}
		else ip = ipField.text;
		
		udpClient.Send(data, data.Length, ip, 5000); 
	}

	private void SendValuesTCP(float value, string what, int motorId)
	{
		if(!isConnected)
		{
			return;
		}
		string msg = "motor:" + motorId + what + ":" + value.ToString("F2") + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		stream.Write(data,0,data.Length);
		
	}
	public void SendValues(int value, string what)
	{
		if(!isConnected)
		{
			return;
		}
		string msg = what + ":" + value.ToString() + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		stream.Write(data,0,data.Length);
		
	}
	public void SendValues(int value, string what, string secret)
	{
		if(!isConnected)
		{
			return;
		}
		string msg = what + ":" + value.ToString()+ secret + "\n";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		stream.Write(data,0,data.Length);
		
	}
	
	public void TorqueSwitch()
	{
		if(!isTorqueOn)
		{
			SendValues(1, "torque");
		}
		else
		{
			SendValues(0, "torque");
		}
		// reply now comes via ReadLoop → Update(), no blocking read here
	}
	
	public void ESTOP()
	{
		string msg = "ESTOP";
		byte[] data = Encoding.ASCII.GetBytes(msg);
		stream.Write(data, 0, data.Length);
	}

	// ── UDP real-time commands ─────────────────────────────────────────────────

	private void SendUDP(string msg)
	{
		if(!isConnected) return;
		byte[] data = Encoding.ASCII.GetBytes(msg);
		_udpControlClient.Send(data, data.Length);
	}

	public void SendSpeed(float speed, int motorId)
	{
		SendUDP("motor:" + motorId + "speed:" + speed.ToString("F2") + "\n");
	}

	public void SendSan(float san, int motorId)
	{
		SendUDP("motor:" + motorId + "san:" + san.ToString("F2") + "\n");
	}

	public void SendProt(float prot, int motorId)
	{
		SendUDP("motor:" + motorId + "prot:" + prot.ToString("F2") + "\n");
	}

	public void SendWbr(float wbr, int motorId)
	{
		SendUDP("motor:" + motorId + "wbr:" + wbr.ToString("F2") + "\n");
	}

	// ─────────────────────────────────────────────────────────────────────────

	// Sent to all game objects before the application is quit.
	protected void OnApplicationQuit()
	{
		udpClient?.Close();
		_udpControlClient?.Close();
		tcpClient?.Close();
		stream?.Close();
	}
	
}