using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net.Sockets;
using System.Text;

public class Send : MonoBehaviour
{
	private TcpClient tcpClient;
	private NetworkStream stream;
	public Text responce;
	private bool isConnected = false;
	public string SECRET;
	
	
	private string static_ip = "10.39.129.142";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
		string ip = static_ip;
		
		try{
			tcpClient = new TcpClient(ip, 5050);
			stream = tcpClient.GetStream();
			byte[] data = Encoding.ASCII.GetBytes(SECRET);
			stream.Write(data,0,data.Length);
			byte[] buffer = new byte[1024];
			int byteRead = stream.Read(buffer,0,buffer.Length) ;
			string tempResponce = Encoding.ASCII.GetString(buffer, 0, byteRead);
			string expeted = "accepted\nvelocity:";
			
			responce.text = tempResponce;
			
		}catch (System.Exception e)
		{
			responce.text = "Connection error" + e.Message;
		}
		yield return null;
	}
	
	public void SendValuesTCP(float value, int motor)
	{
		string msg = "motor:" + motor.ToString() + " angle:" + value.ToString("F2");
		byte[] data = Encoding.ASCII.GetBytes(msg);
		
		if(stream != null)
		{
			stream.Write(data,0,data.Length);
		}
		//Debug.LogError("nextData is Send: " + msg + " To IP: " + ip);
		
	}
	
	// Sent to all game objects before the application is quit.
	protected void OnApplicationQuit()
	{
		tcpClient?.Close();
		stream?.Close();
	}
}
