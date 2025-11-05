using UnityEngine;
using UnityEngine.UI;

public class VelocityManagement : MonoBehaviour
{
	public Scrollbar velocityScrollbar;
	public Text velocityValue;
	public MotorSending sender;
	
	public int motor;
	
	private float velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void ChangedVelocity()
	{
		velocity = velocityScrollbar.value * 29f;
		velocityValue.text = velocity.ToString("F2");
	}
	
	public void SetVelocity()
	{
		sender.SendValues((velocity), "velocity", motor);
	}
	public void SetVelocityCome(float velocity)
	{
		velocityValue.text = velocity.ToString("F2");
		velocityScrollbar.value = velocity/29f;
	}
}
