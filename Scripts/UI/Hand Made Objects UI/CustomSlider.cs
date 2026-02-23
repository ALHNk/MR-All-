using UnityEngine;

public class CustomSlider : MonoBehaviour
{
	[SerializeField] private float upLimitValue;
	[SerializeField] private float downLimitValue;
	[SerializeField] private Transform filledZone;
	[SerializeField] private float maxFillSize = 0.05f;
	[SerializeField] private float fillUpLimit = 0;
	[SerializeField] private float fillDownLimit = -0.25f;

	private float value;
	private float coef;
	
	public ChangeDegrees changeDegrees;

	void Start() 
	{ 
		coef = 1 /(upLimitValue + downLimitValue); 
		if(coef < 0) coef *= -1; 
		if(changeDegrees == null)
		{
			changeDegrees = FindObjectOfType<ChangeDegrees>();
		}
	}

	void Update()
	{
		float nowPosition = transform.localPosition.z;

		value = nowPosition * coef;

		ChangeFilledZone();
		changeDegrees.SetValueText();
	}

	private void ChangeFilledZone()
	{
		Vector3 scale = filledZone.localScale;
		scale.z = value * maxFillSize;
		filledZone.localScale = scale;
		
		UpdateFilledZonePosition();
	}
	
	private void UpdateFilledZonePosition()
	{
		float halfFill = value * (fillUpLimit - fillDownLimit);
		Vector3 pos = filledZone.localPosition;
		//Debug.LogError("X: " + pos.x + " Y: " + pos.y + " Z: " + pos.z);
		pos.z = fillDownLimit - halfFill;
		filledZone.localPosition = pos;
	}
	
	public float GetValue()
	{
		return -value;
	}
	
	public void SetValue(float newValue)
	{
		value = newValue;
		float nowPosition = newValue / coef;
		ChangeFilledZone();
	}
}
