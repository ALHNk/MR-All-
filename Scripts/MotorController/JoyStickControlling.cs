using UnityEngine;
using TMPro;

public class JoyStickControlling : MonoBehaviour
{
	[Header("Speed Settings")]
	[SerializeField] private float speed = 0f;
	[SerializeField] private float speedAngle = 0f;
    
	[Header("Input Sensitivity")]
	[SerializeField] private float speedRange = 5f;
	[SerializeField] private float angleRange = 45f;
	[SerializeField] private float deadzone = 0.3f;
	
	[SerializeField] private TMP_Text RPM;
	
	public int motorid;
	
	private float prevSpeed, prevSpeedAngle;
	
	public MotorSending motor;
	
	protected void Start()
	{
		prevSpeed = 0;
		prevSpeedAngle = 0;
		
		if(motor == null)
		{
			motor = FindObjectOfType<MotorSending>();
		}
	}
    
	void Update()
	{
		Vector2 rightJoystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
		Vector2 leftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
		speed = rightJoystick.y * speedRange;
		speedAngle = leftJoystick.x * angleRange;
		

		if(Mathf.Abs(speed) < deadzone) speed = 0f;
		if(Mathf.Abs(speedAngle) < deadzone) speedAngle = 0f;
		

		if(speed != prevSpeed)
		{
			RPM.text = speed.ToString();
			if(speed != 0 || prevSpeed != 0) 
			{
				motor.SendValues(speed, "speed", motorid);
				prevSpeed = speed;
			}
		}

		if(speedAngle != prevSpeedAngle)
		{
			if(speedAngle != 0 || prevSpeedAngle != 0) 
			{
				motor.SendValues(speedAngle, "san", motorid);
				prevSpeedAngle = speedAngle;
			}
		}
	}
    
	public float GetSpeed()
	{
		return speed;
	}
    
	public float GetSpeedAngle()
	{
		return speedAngle;
	}
}