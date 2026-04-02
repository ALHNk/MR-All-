using UnityEngine;
using UnityEngine.UI;

public class ChangeDegrees : MonoBehaviour
{
	public CustomSlider degreeScrollbar;
	public TMPro.TMP_Text valuetext;
	public MotorSending sender;
	public int motorid;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		if(sender == null)
		{
			sender = FindObjectOfType<MotorSending>();
		}
	}
    
	public void SetScrollbarValue(float value)
	{
		degreeScrollbar.SetValue(value);
	}
	public void SetValueText()
	{
		valuetext.text = degreeScrollbar.GetValue().ToString("F2");
	}
	public void ApplyDegree()
	{
		float degree = degreeScrollbar.GetValue();
		if(degree < 0.01f)
		{
			degree = 0.01f;
		}
		else if(degree > 0.98f)
		{
			degree = 0.98f;
		}
		sender.SendValues(degree, "twodegree", motorid);
	}
}


//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.IO;

//public class ChangeDegrees : MonoBehaviour
//{
//	public CustomSlider degreeScrollbar;
//	public TMPro.TMP_Text valuetext;
//	public MotorSending sender;
//	public int motorid;

//	private StreamWriter logWriter;
//	private static int commandCounter = 0;

//	void Start()
//	{
//		if (sender == null)
//			sender = FindObjectOfType<MotorSending>();

//		string logPath = Path.Combine(Application.persistentDataPath,
//			$"command_log_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
//		logWriter = new StreamWriter(logPath, append: false);
//		logWriter.WriteLine("delay_estimate,cmd_id,timestamp_ms,event,value,motor_id");
//		logWriter.Flush();
//		Debug.Log($"Command log: {logPath}");
//	}

//	public void SetScrollbarValue(float value) => degreeScrollbar.SetValue(value);

//	public void SetValueText() => valuetext.text = degreeScrollbar.GetValue().ToString("F2");

//	public void ApplyDegree()
//	{
//		float degree = degreeScrollbar.GetValue();
//		if (degree < 0.01f) degree = 0.01f;

//		int cmdId = ++commandCounter;
//		long tsMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

//		// Log before send
//		logWriter?.WriteLine($"delay_estimate,{cmdId},{tsMs},SEND,{degree},{motorid}");
//		logWriter?.Flush();

//		sender.SendValues(degree, "estimate", motorid);
//	}

//	void OnDestroy() => logWriter?.Close();
//	void OnApplicationQuit() => logWriter?.Close();
//}
