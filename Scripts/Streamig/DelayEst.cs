using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class DelayEst : MonoBehaviour
{
	public MotorSending sender;
	private StreamWriter logWriter;
	private static int commandCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(sender == null)
	    {
	    	sender = FindObjectOfType<MotorSending>();
	    }
	    string logPath = Path.Combine(Application.persistentDataPath,
		    $"command_log_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
	    logWriter = new StreamWriter(logPath, append: false);
	    logWriter.WriteLine("delay_estimate,cmd_id,timestamp_ms,event,value,motor_id");
	    logWriter.Flush();
	    Debug.Log($"Command log: {logPath}");
	    
	    StartCoroutine(SendRoutine());
    }
    
	IEnumerator SendRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			send();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void send()
	{
		int cmdId = ++commandCounter;
		long tsMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		// Log before send
		logWriter?.WriteLine($"delay_estimate,{cmdId},{tsMs},SEND");
		logWriter?.Flush();
		int value = UnityEngine.Random.Range(0, 1000);
		sender.SendValues(value, "estimate", 0);
	}
}
