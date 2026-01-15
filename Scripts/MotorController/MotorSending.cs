using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
public class MotorSending : MonoBehaviour
{
	private UdpClient udpClient;
	private TcpClient tcpClient;
	private NetworkStream stream;
	public GameObject motorSim;
	public TMP_InputField ipField;
	float prevAngle = 0f;
	public Text responce;
	private TouchScreenKeyboard keyboard;
	[SerializeField] private string SECRET;
	
	public int motor;
	
	public bool isTCP;
	private bool isConnected = false;
	
	public VelocityManagement velMan1, velMan2;
	
	public MoveMotor moveMotor1, moveMotor2;
	
	private string staticIP = "10.39.129.122";
	public string discoveredIp = "";
	
	private bool isTorqueOn = false;
	public TMP_Text torqueText;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(!isTCP) udpClient = new UdpClient();
    }

	public void OpenKeyBoard()
	{
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	}
	//public void CloseKeyBoard()
	//{
	//	keyboard
	//}
	
    // Update is called once per frame
	void Update()
    {
	    float angle1 = motorSim.transform.rotation.eulerAngles.y;
	    
	    //Debug.LogError("angleY: " + angle1);
	    if(prevAngle != angle1 && isConnected)
	    {
	    	//SendValues(angle1, "angle", motor);
	    	float sendingAngle = angle1-45f;
	    	SendValues(sendingAngle, "san", motor);
	    	prevAngle = angle1;
	    }
	    
	    if(keyboard != null)
	    {
	    	ipField.text = keyboard.text;
	    	
	    }
	    
    
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

				velMan1.SetVelocityCome(motorStatus.motor1velocity);
				velMan2.SetVelocityCome(motorStatus.motor2velocity);

				float m1Pos = motorStatus.motor1position;
				float m2Pos = motorStatus.motor2position;
				float m1Low = motorStatus.motor1limitlow;
				float m1Up = motorStatus.motor1limitup;
				float m2Low = motorStatus.motor2limitlow;
				float m2Up = motorStatus.motor2limitup;
				
				//moveMotor1.StartMotorPosition(m1Pos, m1Low, m1Up);
				//moveMotor2.StartMotorPosition(m2Pos, m2Low, m2Up);
				isConnected = true;
				
			}
			
			//string expeted = "accepted\nvelocity:";
			//if(tempResponce.StartsWith(expeted))
			//{
			//	isConnected = true;
			//	string velocityString = tempResponce.Substring(expeted.Length).Trim();
			//	if(float.TryParse(velocityString, out float velocityCome))
			//	{
			//		velMan.SetVelocityCome(velocityCome);
			//	}
			//}
			responce.text = tempResponce;
			
			
			
		}catch (System.Exception e)
		{
			responce.text = "Connection error" + e.Message;
		}
		yield return null;
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
		//Debug.LogError("nextData is Send: " + msg + " To IP: " + ip);
		
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
		byte[] buffer = new byte[256];
		int byteRead = stream.Read(buffer,0,buffer.Length);
		string torqueStatus = Encoding.ASCII.GetString(buffer, 0, byteRead);
		if(torqueStatus.Equals("on\n"))
		{
			isTorqueOn = true;
			torqueText.text = "Torque off";
		}
		else if(torqueStatus.Equals("off\n"))
		{
			isTorqueOn = false;
			torqueText.text = "Torque on";
		}
	}
	
	// Sent to all game objects before the application is quit.
	protected void OnApplicationQuit()
	{
		udpClient?.Close();
		tcpClient?.Close();
		stream?.Close();
	}
	
}
