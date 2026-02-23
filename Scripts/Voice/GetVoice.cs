using UnityEngine;
using UnityEngine.UI;
using Oculus.Voice;
public class GetVoice : MonoBehaviour
{
	public AppVoiceExperience appVoice;
	public Text test;
	public MotorSending motor;
	
	[Header("Values for the motor")]
	public float speed;
	public float speedAngel;
	public int motorid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(motor == null)
	    {
	    	motor = FindObjectOfType<MotorSending>();
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    //test.text = appVoice.
    }
    
	public void ActivateVoiceControl()
	{
		appVoice.Activate();
		test.text = "Clicked";
	}
	
	public void TriggerSpeed(string[] input)
	{
		string action = input[0].ToLower();
		if(input.Length > 1) 
		{
			string direction = input[1].ToLower();
			switch(action)
			{
				case "move":
					switch(direction)
					{
						case "forward":
							test.text = "Forward";
							motor.SendValues(speed, "speed", motorid);
							break;
						case "back":
							test.text = "Back";
							motor.SendValues(-speed, "speed", motorid);
							break;
						default:
							break;
					}
					break;
				case "turn":
					switch(direction)
					{
						case "left":
							test.text = "Left";
							motor.SendValues(speedAngel, "san", motorid);
							break;
						case "right":
							test.text = "Right";
							motor.SendValues(-speedAngel, "san", motorid);
							break;
						default:
							break;
					}
					break;
				default:
					break;
			}
		}
		else 
		{
			if(action.Equals("stop"))
				{
					test.text = "Stop";
					motor.SendValues(0, "san", motorid);
					motor.SendValues(0, "speed", motorid);
				}
		}
	}
		
}
