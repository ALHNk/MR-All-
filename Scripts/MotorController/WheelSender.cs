using UnityEngine;

public class WheelSender : MonoBehaviour
{
	float prevAngle = 0f;
	public float decreaseAngel;
	public string what;
	public MotorSending sender;
	public int motor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(sender == null)
	    {
	    	sender = FindObjectOfType<MotorSending>();
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    float angle1 = transform.rotation.eulerAngles.y;
	    
	    //Debug.LogError("angleY: " + angle1);
	    if(prevAngle != angle1 && sender.isConnected)
	    {
	    	//SendValues(angle1, "angle", motor);
	    	float sendingAngle = angle1 - decreaseAngel;
	    	sender.SendValues(sendingAngle, what, motor);
	    	prevAngle = angle1;
	    }
	    
    }
}
