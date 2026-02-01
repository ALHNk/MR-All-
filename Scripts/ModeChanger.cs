using UnityEngine;
using TMPro;

public class ModeChanger : MonoBehaviour
{
	public GameObject WheelModeEnv;
	public GameObject SpeedModeEnv;
	
	public TMP_Text modeText;
	private bool isSpeedMode = true;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		WheelModeEnv.SetActive(false);
		SpeedModeEnv.SetActive(true);
	}
	
	public void Change()
	{
		WheelModeEnv.SetActive(isSpeedMode);
		SpeedModeEnv.SetActive(!isSpeedMode);
		if(!isSpeedMode)
		{
			modeText.text = "Speed Mode";
		}
		else
		{
			modeText.text = "Wheel Mode";
		}
		isSpeedMode = !isSpeedMode;
	}
	
	public bool GetIsSpeedMode()
	{
		return isSpeedMode;
	}
}
