using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class MoveMotor : MonoBehaviour
{
	public bool isSan;
	
	private Quaternion prevFingerRotation;
	private bool isFirst = true;
	
	public OVRHand handRight, handLeft;
	[SerializeField] private float yAngle = 0;
	
	private bool fingerRight1, fingerRight2, fingerRight3;
	private bool fingerLeft1, fingerLeft2, fingerLeft3;
	
	private GrabFreeTransformer gft;
	public float lowLimiti = 0, highLimiti = 360;
	public HandGrabInteractable grabInt;
	public float maxAllowedHandAngularSpeed;
	
	private Grabbable grabbable;
	[SerializeField]
	private GameObject wheelMesh;
	private WheelSender wheelSender;
	
	
	private bool fingersValid = false;
	
	private float previousMeshAngle = 0f;
	
	private bool isZeroed;
	
    void Start()
	{
		gft = GetComponent<GrabFreeTransformer>();
		//StartMotorPosition(12, 45, 87);
		
		
		
		if(grabInt == null)
		{
			grabInt = GetComponentInChildren<HandGrabInteractable>();
		}
		grabbable = GetComponent<Grabbable>();
		if(wheelSender == null) {
			wheelSender = wheelMesh.GetComponent<WheelSender>();
		}
		
		previousMeshAngle = transform.localRotation.eulerAngles.y;
	}
    
	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	protected void FixedUpdate()
	{
		RotateMesh();
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if(grabbable.SelectingPoints.Count == 0 && !isZeroed)
		{
			transform.localRotation = Quaternion.Euler(0, 45f, 0);
			wheelMesh.transform.localRotation = Quaternion.Euler(0, 45f, 0);
			wheelSender.SendZero();
			isZeroed = true;
		}
	}

	protected void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("FingerRight1")) fingerRight1 = true;
		if(other.CompareTag("FingerRight2")) fingerRight2 = true;
		if(other.CompareTag("FingerRight3")) fingerRight3 = true;
		if(other.CompareTag("FingerLeft1")) fingerLeft1 = true;
		if(other.CompareTag("FingerLeft2")) fingerLeft2 = true;
		if(other.CompareTag("FingerLeft3")) fingerLeft3 = true;
		isZeroed = false;
		
		
		if(grabbable.SelectingPoints.Count == 2)
		{
			wheelSender.OnPointRotation();
		}
		else if(grabbable.SelectingPoints.Count == 1)
		{
			if(grabInt.State == InteractableState.Select)
			{
				canRotate();
				if(fingersValid)
				{
					wheelSender.SanRotation(yAngle);	
				}
					
			}
			
		}
		
	}
	

	
	public void canRotate()
	{
		if((!fingerRight1 || !fingerRight2 || !fingerRight3) && (!fingerLeft1 || !fingerLeft2 || !fingerLeft3))
		{
			isFirst = true;
			fingersValid = false;
			return;
		}
		
		if(!fingersValid)
		{
			wheelSender.InitializeSmoothedAngle(yAngle);
			fingersValid = true;
		}
		
		if( fingerRight1 && fingerRight2 && fingerRight3)
		{
			RotateByHand(true);
		}
		else
		{
			RotateByHand(false);
		}
	}
	
	private void RotateByHand(bool isRight)
	{
		Transform hand = null;
		if(isRight) hand = handRight.transform;
		else hand = handLeft.transform;
		if(hand == null)
		{
			Debug.LogError("No Hands");
			return;
		}
		if(isFirst)
		{
			prevFingerRotation = hand.rotation;
			isFirst = false;
			return;
		}
		
		float angularDelta = Quaternion.Angle(prevFingerRotation, hand.rotation);
		float handAngularSpeed = angularDelta / Time.deltaTime; 

		if (handAngularSpeed > maxAllowedHandAngularSpeed)
		{
			prevFingerRotation = hand.rotation;
			return;
		}
		
		float deltaY = Mathf.DeltaAngle(
			prevFingerRotation.eulerAngles.y,
			hand.eulerAngles.y
		);
		
		//float deltaY = hand.eulerAngles.y - prevFingerRotation.eulerAngles.y;
		float newAngle = yAngle + deltaY;
		if(newAngle >= lowLimiti && newAngle <= highLimiti)
		{
			yAngle = newAngle;
			//transform.localRotation= Quaternion.Euler(0, yAngle, 0);
			//wheelSender.printYAngle(yAngle);
			prevFingerRotation = hand.rotation;
		}
		
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
		//transform.localRotation = Quaternion.Euler(0, 45f, 0);
		//wheelMesh.transform.localRotation = Quaternion.Euler(0, 45f, 0);
		yAngle = 45f;
		if(isSan) 
		{
			
			//wheelSender.SendZero();
		}
	}
	
	public void StartMotorPosition(float positionNow)
	{
		yAngle = positionNow;
		transform.rotation = Quaternion.Euler(0, positionNow, 0);
		
	}
	
	public void RotateMesh()
	{
		float currentAngle = wheelMesh.transform.localEulerAngles.y;
		if(currentAngle > 180f) currentAngle -= 360f;
    
		float targetAngle = transform.localRotation.eulerAngles.y;
		if(targetAngle > 180f) targetAngle -= 360f;
	
		//float angularDelta = Mathf.Abs(Mathf.DeltaAngle(previousMeshAngle, targetAngle));
		//float angularSpeed = angularDelta / Time.deltaTime;
	
		//if(angularSpeed > maxAllowedHandAngularSpeed)
		//{
		//	previousMeshAngle = currentAngle;
		//	return;
		//}
    
		float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 5f);
    
		Vector3 newEuler = wheelMesh.transform.localEulerAngles;
		newEuler.y = newAngle;
		wheelMesh.transform.localEulerAngles = newEuler;
	
		previousMeshAngle = targetAngle;
	}
	
}
		