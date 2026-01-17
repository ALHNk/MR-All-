using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Configurations : MonoBehaviour
{
	[Header("Scrollbars")]
	public Scrollbar leverSensBar;
	public Scrollbar leverScaleBar;
	[Header("Scripts")]
	public TimeManagedLeverControl lever;
	public LeverTransformCon leverBase;
	[Header("Objects")]
	//public GameObject leverBase;
	[Header("UI")]
	public GameObject SettingsPanel;
	public GameObject LeverSettingsPanel;
	public GameObject SettingsButton;
	public TMP_Text leverSensText;
	public TMP_Text leverScaleText;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		if(lever == null)
		{
			lever = FindObjectOfType<TimeManagedLeverControl>();
		}
		if(leverBase == null)
		{
			leverBase = FindObjectOfType<LeverTransformCon>();
		}
		leverSensBar.value = lever.GetMaxAllowedHandSpeed();
		leverScaleBar.value = (leverBase.GetInitialScale() / 2);
		LeverSettingsPanel.SetActive(false);
		SettingsPanel.SetActive(false);
		
		LeverSensTextChange();
		LeverScaleTextChange();
	}
	
	public void LeverSensTextChange()
	{
		leverSensText.text = leverSensBar.value.ToString("F2");
	}
	public void LeverScaleTextChange()
	{
		leverScaleText.text = leverScaleBar.value.ToString("F2");
	}
	
	public void ChangeLeverSencitivity()
	{
		lever.SetMaxAllowedHandSpeed(leverSensBar.value);
	}
	
	public void ChangeLeverScale()
	{
		leverBase.ChangeScale(leverScaleBar.value*2);
	}
	
	public void OpenSettings()
	{
		SettingsPanel.SetActive(true);
		SettingsButton.SetActive(false);
	}
	public void OpenLeverSettings()
	{
		SettingsPanel.SetActive(false);
		LeverSettingsPanel.SetActive(true);
	}
	public void CloseLeverSettings()
	{
		SettingsPanel.SetActive(true);
		LeverSettingsPanel.SetActive(false);
	}
	public void CloseSettings()
	{
		SettingsPanel.SetActive(false);
		SettingsButton.SetActive(true);
	}
}
