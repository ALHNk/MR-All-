using UnityEngine;

public class WheelSender : MonoBehaviour
{
	float prevAngle = 0f;
	public float decreaseAngel;
	public string what;
	public MotorSending sender;
	public int motor;
	
	private float smoothedAngle = 0f;
	public float smoothSpeed = 5f;
	public float rotationDeadzone = 0.5f;
	public float wheelBasedRotationDeadzone = 5.0f;
	public float wheelBasedRotationMaxAmplitude = 10.0f;
	public float rotationMaxAmplitue = 2.0f;
	
	public TMPro.TMP_Text uitext;
	
	private bool isZeroSend;
	void Start()
	{
		if(sender == null)
		{
			sender = FindObjectOfType<MotorSending>();
		}
	}

	public void InitializeSmoothedAngle(float currentAngle)
	{
		smoothedAngle = currentAngle;
		prevAngle = currentAngle;
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if(!isZeroSend && transform.localRotation.eulerAngles.y == 70f)
		{
			SendZero();
		}
	}
	
	public void SendZero()
	{
		//sender.SendValues(0f, what, motor);
		//sender.SendValues(0f, "prot", motor);
		//sender.SendSan(0f, motor);
		//sender.SendProt(0f, motor);
		sender.SetPacketSan(0f);
		sender.SetPacketProt(0f);
		prevAngle = 70f;
		isZeroSend = true;
		uitext.text = "0";
	}
	public void	SendWbrZero()
	{
		sender.SendWbr(0f, 0);
	}
    
	public void SanRotation(float directAngle)
	{
		//smoothedAngle = Mathf.Lerp(smoothedAngle, directAngle, Time.deltaTime * smoothSpeed);
	
		//if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && sender.isConnected)
		//{
		//	float sendingAngle = smoothedAngle - decreaseAngel;
		//	uitext.text = smoothedAngle.ToString("F1");
		//	sender.SendValues(sendingAngle, what, motor);
		//	prevAngle = smoothedAngle;
		//}
		
		float angle1 = transform.localRotation.eulerAngles.y;
		//float angle1 = directAngle;
		isZeroSend = false;
		if(angle1 > 180)
		{
			angle1 -= 360f;
		}	
		smoothedAngle = Mathf.Lerp(smoothedAngle, angle1, Time.deltaTime * smoothSpeed);
		if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && Mathf.Abs(prevAngle - smoothedAngle) < rotationMaxAmplitue && sender.isConnected)
		{
			float sendingAngle = smoothedAngle - decreaseAngel;
			uitext.text = angle1.ToString("F1");
			//sender.SendValues(-sendingAngle, what, motor);
			//sender.SendSan(-sendingAngle, motor);
			sender.SetPacketSan(-sendingAngle);
		}
		prevAngle = angle1;
	}
	public void WheelBasedRotation()
	{
		float angle1 = transform.localRotation.eulerAngles.y;
		isZeroSend = false;
		if(angle1 > 180)
		{
			angle1 -= 360f;
		}	
		smoothedAngle = Mathf.Lerp(smoothedAngle, angle1, Time.deltaTime * smoothSpeed);
		if(Mathf.Abs(prevAngle - smoothedAngle) > wheelBasedRotationDeadzone && Mathf.Abs(prevAngle - smoothedAngle) < wheelBasedRotationMaxAmplitude && sender.isConnected)
		{
			float sendingAngle = smoothedAngle - decreaseAngel;
			uitext.text = angle1.ToString("F1");
			//sender.SendValues(-sendingAngle, "wbr", 0);
			sender.SendWbr(-sendingAngle, 0);
		}
		prevAngle = angle1;
	}
    
	public void OnPointRotation()
	{
		float angle1 = transform.localRotation.eulerAngles.y;
		isZeroSend = false;
		if(angle1 > 180)
		{
			angle1 -= 360f;
		}	
		smoothedAngle = Mathf.Lerp(smoothedAngle, angle1, Time.deltaTime * smoothSpeed);
		if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && Mathf.Abs(prevAngle - smoothedAngle) < rotationMaxAmplitue && sender.isConnected)
		{
			float sendingAngle = smoothedAngle - decreaseAngel;
			uitext.text = sendingAngle.ToString("F1");
			//sender.SendValues(sendingAngle, "prot", motor);
			//sender.SendProt(sendingAngle, motor);
			sender.SetPacketProt(sendingAngle);
			
		}
		prevAngle = angle1;
	}
	
	public void printYAngle(float angle)
	{
		//uitext.text = angle.ToString("F1");
	}
}