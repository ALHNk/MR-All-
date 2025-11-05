using UnityEngine;
using Oculus.Interaction;

public class MoveMotor : MonoBehaviour
{

	
	private Quaternion prevFingerRotation;
	private bool isFirst = true;
	
	public OVRHand handRight, handLeft;
	[SerializeField] private float yAngle = 0;
	
	private bool fingerRight1, fingerRight2, fingerRight3;
	private bool fingerLeft1, fingerLeft2, fingerLeft3;
	
	private GrabFreeTransformer gft;
	private float lowLimiti = 0, highLimiti = 360;
	
    void Start()
	{
		gft = GetComponent<GrabFreeTransformer>();
		//StartMotorPosition(12, 45, 87);
    }

	protected void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("FingerRight1")) fingerRight1 = true;
		if(other.CompareTag("FingerRight2")) fingerRight2 = true;
		if(other.CompareTag("FingerRight3")) fingerRight3 = true;
		if(other.CompareTag("FingerLeft1")) fingerLeft1 = true;
		if(other.CompareTag("FingerLeft2")) fingerLeft2 = true;
		if(other.CompareTag("FingerLeft3")) fingerLeft3 = true;
		
		canRotate();
		
	}
	
	public void canRotate()
	{
		if((!fingerRight1 || !fingerRight2 || !fingerRight3) && (!fingerLeft1 || !fingerLeft2 || !fingerLeft3))
		{
			isFirst = true;
			return;
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
		}
		
		float deltaY = hand.eulerAngles.y - prevFingerRotation.eulerAngles.y;
		float newAngle = yAngle + deltaY;
		if(newAngle >= lowLimiti && newAngle <= highLimiti)
		{
			yAngle = newAngle;
			transform.rotation = Quaternion.Euler(0, yAngle, 0);
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
	}
	
	public void StartMotorPosition(float positionNow, float positionLow, float positionUp)
	{
		transform.rotation = Quaternion.Euler(0, positionNow, 0);
		lowLimiti = positionLow;
		highLimiti = positionUp;
		var constraints = new TransformerUtils.RotationConstraints()
		{
			XAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = 0, Max = 0}
			},
			YAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = positionUp, Max = positionLow}
			},
			ZAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = 0, Max = 0}
			}
		};
		gft.InjectOptionalRotationConstraints(constraints);
	}
	
}
		