using UnityEngine;
using UnityEngine.UI;

public class ChangeDegrees : MonoBehaviour
{
	public CustomSlider degreeScrollbar;
	public TMPro.TMP_Text valuetext;
	public MotorSending sender;
	public int motorid;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		if(sender == null)
		{
			sender = FindObjectOfType<MotorSending>();
		}
	}
    
	public void SetScrollbarValue(float value)
	{
		degreeScrollbar.SetValue(value);
	}
	public void SetValueText()
	{
		valuetext.text = degreeScrollbar.GetValue().ToString("F2");
	}
	public void ApplyDegree()
	{
		float degree = degreeScrollbar.GetValue();
		if(degree < 0.01f)
		{
			degree = 0.01f;
		}
		sender.SendValues(degree, "twodegree", motorid);
	}
}
