using UnityEngine;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using TMPro;

public class TimeManagedLeverControl : MonoBehaviour
{
	public OVRHand rightHand, leftHand;
	private HandGrabInteractable grabInt;
	
	private bool fingerRight1, fingerRight2, fingerRight3;
	private bool fingerLeft1, fingerLeft2, fingerLeft3;
	
	private bool isFirst = true;
	
	public Transform lever;

	private Vector3 previousHandPos;

	public float maxAllowedHandSpeed = 0.2f;
	public float rotationSpeed = 50f;

	[SerializeField] float minRotation = -45f;
	[SerializeField] float maxRotation = 45f;
	
	public TMP_Text RPM_Text;								
	
	[SerializeField]  float coef;
	
	public MotorSending sender;
	[SerializeField] int motorId;
	
	public HandGrabInteractable rightGrip;
	
	private bool isStopped;
		
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		grabInt = GetComponentInChildren<HandGrabInteractable>();
	}
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		float currentAngle = lever.eulerAngles.z;
		if(currentAngle >180f) currentAngle -= 360f;
		if((grabInt.State == InteractableState.Normal) && ( currentAngle < (1.0f/coef) && currentAngle >(-1.0f/coef)))
		{
			Vector3 newEuler = lever.localEulerAngles;
			newEuler.z = 0f;
			lever.localEulerAngles = newEuler;
			RPM_Text.text = "0";
			if(!isStopped)
			{
				sender.SendValues(0f, "speed", motorId);
				isStopped = true;
			}
		}
		else{
			isStopped = false;
		}
	}
	
	protected void OnTriggerStay(Collider other)
	{
		if(grabInt.State == InteractableState.Select)
		{
			if(other.CompareTag("FingerRight1")) fingerRight1 = true;
			if(other.CompareTag("FingerRight2")) fingerRight2 = true;
			if(other.CompareTag("FingerRight3")) fingerRight3 = true;
			if(other.CompareTag("FingerLeft1")) fingerLeft1 = true;
			if(other.CompareTag("FingerLeft2")) fingerLeft2 = true;
			if(other.CompareTag("FingerLeft3")) fingerLeft3 = true;
			
			canRotate();
			//RotateByHand(true);
		}
		
		
	}
	
	private void canRotate()
	{
		
		if((!fingerRight1 || !fingerRight2 || !fingerRight3) && (!fingerLeft1 || !fingerLeft2 || !fingerLeft3))
		{
			isFirst = true;
			return;
		}
		if(fingerLeft1 && fingerLeft2 && fingerLeft3) 
		{
			RotateByHand(false);
		}
		else
		{
			RotateByHand(true);
		}
	}
	
	private void RotateByHand(bool isRight)
	{
		Transform hand;
		if(isRight)
		{
			hand = rightHand.transform ;
		}
		else{
			hand = leftHand.transform ;
		}
		
		if (hand == null)
		{
			Debug.LogError("No Hand Found");
			return;
		}
		if(isFirst)
		{
			previousHandPos = hand.position;
			isFirst = false;
		}
		Vector3 currentHandPos = hand.position;
		

		float handSpeed = Vector3.Distance(currentHandPos, previousHandPos) / Time.deltaTime;
						
		if (handSpeed > maxAllowedHandSpeed)
		{
			return;
		}
		
		Vector3 currentLocal = lever.InverseTransformPoint(currentHandPos);
		Vector3 previousLocal = lever.InverseTransformPoint(previousHandPos);

		Vector3 localDelta = currentLocal - previousLocal;

		
		float movement = -localDelta.x;	

		float currentAngle = lever.localEulerAngles.z;
		if (currentAngle > 180f) currentAngle -= 360f; 

		float newAngle = Mathf.Clamp(currentAngle + movement * rotationSpeed, minRotation, maxRotation);
		
		Vector3 newEuler = lever.localEulerAngles;
		newEuler.z = newAngle;
		lever.localEulerAngles = newEuler;
		float speedToSend = newAngle * coef;
		RPM_Text.text = speedToSend.ToString("F1");
		sender.SendValues(speedToSend, "speed", motorId);
		previousHandPos = currentHandPos;
		
	}
	public float GetMaxAllowedHandSpeed()
	{
		return maxAllowedHandSpeed;
	}
	
	public void SetMaxAllowedHandSpeed(float inputSense)
	{
		maxAllowedHandSpeed = inputSense;
	}
	
	protected void OnTriggerExit(Collider other)
	{
		isFirst = true;
		fingerRight1 = false;
		fingerRight2 = false;
		fingerRight3 = false;
		fingerLeft1 = false;
		fingerLeft2 = false;
		fingerLeft3 = false;
	}
}
