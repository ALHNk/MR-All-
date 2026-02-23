using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
	private Vector3 originalPosition;
	private bool isPressed;
	private bool hasEntered;
	private bool hasTriggered;

	public float pressDistance = 0.1f;
	public float pressSpeed = 5f;
	public float pressThreshold = 0.005f;

	[Header("Button Event")]
	public UnityEvent OnButtonPressed;   

	void Start()
	{
		originalPosition = transform.localPosition;
	}

	void Update()
	{
		Vector3 targetPosition = isPressed
			? originalPosition - new Vector3(0, pressDistance, 0)
			: originalPosition;

		transform.localPosition = Vector3.Lerp(
			transform.localPosition,
			targetPosition,
			Time.deltaTime * pressSpeed
		);

		// Fully pressed check
		if (isPressed && !hasTriggered &&
			Vector3.Distance(transform.localPosition, targetPosition) < pressThreshold)
		{
			hasTriggered = true;
			OnButtonPressed?.Invoke();   
		}

		if (!isPressed)
		{
			hasTriggered = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		//if ((other.CompareTag("FingerRight1") || other.CompareTag("FingerLeft1")) && !hasEntered)
		if (other.CompareTag("Palm"))
		{
			isPressed = true;
			hasEntered = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Palm"))
		{
			isPressed = false;
			hasEntered = false;
		}
	}
}
