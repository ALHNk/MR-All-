using UnityEngine;

public class LeverTransformCon : MonoBehaviour
{
	private float initialScale;
	protected void Start()
	{
		initialScale = transform.localScale.x;
	}
	public void ChangeScale(float scaler)
	{
		transform.localScale = new Vector3(initialScale*scaler, initialScale*scaler, initialScale*scaler);
	}
	public float GetInitialScale()
	{
		return initialScale;
	}
}
