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
	public float rotationMaxAmplitue = 2.0f;
	
	public TMPro.TMP_Text huitext;
	
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
		if(prevAngle != 45f && transform.localRotation.eulerAngles.y == 45f)
		{
			SendZero();
		}
	}
	
	public void SendZero()
	{
		sender.SendValues(0f, what, motor);
		sender.SendValues(0f, "prot", motor);
	}
    
	public void SanRotation(float directAngle)
	{
		//smoothedAngle = Mathf.Lerp(smoothedAngle, directAngle, Time.deltaTime * smoothSpeed);
	
		//if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && sender.isConnected)
		//{
		//	float sendingAngle = smoothedAngle - decreaseAngel;
		//	huitext.text = smoothedAngle.ToString("F1");
		//	sender.SendValues(sendingAngle, what, motor);
		//	prevAngle = smoothedAngle;
		//}
		
		float angle1 = transform.localRotation.eulerAngles.y;
		if(angle1 > 180)
		{
			angle1 -= 360f;
		}	
		smoothedAngle = Mathf.Lerp(smoothedAngle, angle1, Time.deltaTime * smoothSpeed);
		if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && Mathf.Abs(prevAngle - smoothedAngle) < rotationMaxAmplitue && sender.isConnected)
		{
			float sendingAngle = smoothedAngle - decreaseAngel;
			huitext.text = smoothedAngle.ToString("F1");
			sender.SendValues(sendingAngle, what, motor);
			prevAngle = smoothedAngle;
		}
	}
    
	public void OnPointRotation()
	{
		float angle1 = transform.localRotation.eulerAngles.y;
		if(angle1 > 180)
		{
			angle1 -= 360f;
		}	
		smoothedAngle = Mathf.Lerp(smoothedAngle, angle1, Time.deltaTime * smoothSpeed);
		if(Mathf.Abs(prevAngle - smoothedAngle) > rotationDeadzone && Mathf.Abs(prevAngle - smoothedAngle) < rotationMaxAmplitue && sender.isConnected)
		{
			float sendingAngle = smoothedAngle - decreaseAngel;
			huitext.text = smoothedAngle.ToString("F1");
			sender.SendValues(sendingAngle, "prot", motor);
			prevAngle = smoothedAngle;
		}
	}
	
	public void printYAngle(float angle)
	{
		huitext.text = angle.ToString("F1");
	}
}