using UnityEngine;

public class EnvPos : MonoBehaviour
{
	public float increment;
	public StreamingHolder strHold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    if(strHold.isLocked == false)
	    {
	    	if(OVRInput.GetDown(OVRInput.Button.Three))
		    {
		    	EnvUP();
		    }
		    if(OVRInput.GetDown(OVRInput.Button.One))
		    {
		    	EnvDown();
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
