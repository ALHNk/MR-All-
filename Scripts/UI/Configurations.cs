using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Configurations : MonoBehaviour
{
	[Header("Scrollbars")]
	public Scrollbar leverSensBar;
	public Scrollbar leverScaleBar;
	public Scrollbar tableScaleBar;
	[Header("Scripts")]
	public TimeManagedLeverControl lever;
	public LeverTransformCon leverBase;
	[Header("Objects")]
	//public GameObject leverBase;
	public GameObject table;
	[Header("Panels")]
	public GameObject SettingsPanel;
	public GameObject LeverSettingsPanel, SteerSettingsPanel, TableSettingsPanel;
	[Header("Buttons")]
	public GameObject SettingsButton;
	[Header("Texts")]
	public TMP_Text leverSensText;
	public TMP_Text leverScaleText;
	public TMP_Text tableScaleText;
	
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
		if(PlayerPrefs.HasKey("LeverScale"))
		{
			float savedScale = PlayerPrefs.GetFloat("LeverScale");
			leverScaleBar.value = savedScale;
			float actualScale = savedScale * 0.4f + 0.95f;
			leverBase.ChangeScale(actualScale);		
		}
		else
		{
			leverScaleBar.value = leverBase.GetInitialScale();
		}

		if(PlayerPrefs.HasKey("TableScale"))
		{
			float savedScale = PlayerPrefs.GetFloat("TableScale");
			tableScaleBar.value = savedScale;
			savedScale = 0.4f*savedScale + 0.6f;
			table.transform.localScale = new Vector3(savedScale, savedScale, savedScale);
		}
		else
		{
			tableScaleBar.value = 1;
		}
		
		LeverSettingsPanel.SetActive(false);
		SteerSettingsPanel.SetActive(false);
		TableSettingsPanel.SetActive(false);
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
	
	public void TableScaleTextChange()
	{
		tableScaleText.text = tableScaleBar.value.ToString("F2");
	}
	
	public void ChangeLeverSencitivity()
	{
		lever.SetMaxAllowedHandSpeed(leverSensBar.value);
	}
	
	public void ChangeLeverScale()
	{
		float scaleValue = leverScaleBar.value;
		PlayerPrefs.SetFloat("LeverScale", scaleValue);
		leverBase.ChangeScale(scaleValue*0.4f + 0.95f);
		
	}
	
	public void ChangeTableScale()
	{
		PlayerPrefs.SetFloat("TableScale", tableScaleBar.value);
		float scaleValue = 0.4f*tableScaleBar.value + 0.6f;
		table.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
		
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
	public void OpenTableSettings()
	{
		SettingsPanel.SetActive(false);
		TableSettingsPanel.SetActive(true);
	}
	public void CloseTableSettings()
	{
		SettingsPanel.SetActive(true);
		TableSettingsPanel.SetActive(false);
	}
	public void OpenSteerSettings()
	{
		SettingsPanel.SetActive(false);
		SteerSettingsPanel.SetActive(true);
	}
	public void CloseSteerSettings()
	{
		SettingsPanel.SetActive(true);
		SteerSettingsPanel.SetActive(false);
	}
	public void CloseSettings()
	{
		SettingsPanel.SetActive(false);
		SettingsButton.SetActive(true);
	}
	
	public void ClearCache()
	{
		PlayerPrefs.DeleteAll();
	}
}
