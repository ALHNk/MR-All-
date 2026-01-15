using UnityEngine;

public class MainLocker : MonoBehaviour
{
	private bool isLocked;
	[SerializeField] MotorSending sender;
	private string secret;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void LockUnlock()
	{
		if(isLocked)
		{
			isLocked = false;
			//sender.SendValues(0, "lock", secret);
			sender.SendValues(0, "lock");
		}
		else
		{
			isLocked = true;
			sender.SendValues(1, "lock");
			//secret = sender.TakeSecret();
		}
	}
    
    
	public void setIsLocked(bool locker)
	{
		isLocked = locker;
	}
    
	public bool getIsLocked()
	{
		return isLocked;
	}
}
