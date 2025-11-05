using UnityEngine;
using TMPro;
using Oculus.Interaction;

public class LeverController : MonoBehaviour
{
	public TMP_Text speedText;
	public float coef;
	private float startPosition;
	private float speedValue;
	private float prevPosition;
	
	public GrabInteractable grab;
	
	public int motor;
	public MotorSending sender;
	private bool isStopped;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    startPosition = transform.position.x;
	    prevPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
	{
		//if(grab.State == InteractableState.Disabled && isStopped)
	    //{
		//	speedValue = 0;
		//	SendSpeed();
		//	isStopped = true;
	    //}
	    speedValue = (transform.position.x - startPosition) * coef;
	    speedText.text = speedValue.ToString("F2");
	    if(transform.position.x != prevPosition)
	    {
	    	SendSpeed();
	    	prevPosition = transform.position.x;
	    	isStopped = false;
	    }
	   
    }
    
	public void SendSpeed()
	{
		sender.SendValues(speedValue, "speed", motor);
	}
}
