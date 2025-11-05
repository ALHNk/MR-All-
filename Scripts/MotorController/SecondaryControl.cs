using UnityEngine;
using Oculus.Interaction;

public class SecondaryCoontrol : MonoBehaviour
{
	public GrabFreeTransformer gft;
	public GameObject mainMotor;
	public float coef;
	
	[Header("Constraints")]
	public float xFreeze, yFreeze, zFreeze;
	
	private	bool isFirst = true;
	private Vector3 freezePosition ;
	
	public GameObject scaleText;
		
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		freezePosition = new Vector3(xFreeze, yFreeze, zFreeze);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
	// OnTriggerStay is called once per frame for every Collider other that is touching the trigger.
	protected void OnTriggerStay(Collider other)
	{
		if(other.tag == "MainMotor")
		{
			if(isFirst)
			{
				LockConstraints();
				scaleText.SetActive(false);
				isFirst = false;
			}
			transform.position = freezePosition;
			mainMotor.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y*coef, 0f);
		}
	}   
	
	// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	protected void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("MainMotor"))
		{
			isFirst = true;
			scaleText.SetActive(true);
			UnLockConstraints();
		}
	}
    
	public void LockConstraints()
	{
		var constraints = new TransformerUtils.PositionConstraints()
		{
			XAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = xFreeze, Max = xFreeze}
			},
			YAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = yFreeze, Max = yFreeze}
			},
			ZAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = true, 
				AxisRange = new TransformerUtils.FloatRange() {Min = zFreeze, Max = zFreeze}
			}
		};
		gft.InjectOptionalPositionConstraints(constraints);
	}
	
	public void UnLockConstraints()
	{
		var constraints = new TransformerUtils.PositionConstraints()
		{
			XAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = false, 
			},
			YAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = false, 
			},
			ZAxis = new TransformerUtils.ConstrainedAxis()
			{
				ConstrainAxis = false, 
			}
		};
		gft.InjectOptionalPositionConstraints(constraints);
	}
}
