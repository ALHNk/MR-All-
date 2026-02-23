using UnityEngine;

public class EnvPos : MonoBehaviour
{
	public float increment;
	public StreamingHolder strHold;
	public OVRHand leftHand, rightHand;
	[SerializeField] private float threshold = 0.8f;
	
	private bool wasRightPinch, wasLeftPinch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    wasLeftPinch = false;
	    wasRightPinch = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if(strHold.isLocked == false)
	    {
	    	if(OVRInput.GetDown(OVRInput.Button.Four))
		    {
		    	EnvUP();
		    }
		    if(OVRInput.GetDown(OVRInput.Button.Two))
		    {
		    	EnvDown();
		    }
		    float leftPinching = leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
		    float rightPinching = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
		    bool isLeftPinch = leftPinching > threshold;
		    bool isRightPinch = rightPinching > threshold;
		    if(isLeftPinch && !wasLeftPinch)
		    {
		    	EnvUP();
		    	wasLeftPinch = true;
		    }
		    else if(!isLeftPinch && wasLeftPinch)
		    {
		    	wasLeftPinch = false;
		    }
		    if(isRightPinch && !wasRightPinch)
		    {
		    	EnvDown();
		    	wasRightPinch = true;
		    }
		    else if(!isRightPinch && wasRightPinch)
		    {
		    	wasRightPinch = false;
		    }
		    
	    }
    }
    
	public void EnvDown()
	{
		transform.position = new Vector3(0f,transform.position.y - increment, 0f);
	}
	public void EnvUP()
	{
		transform.position = new Vector3(0f,transform.position.y + increment, 0f);
	}
}
