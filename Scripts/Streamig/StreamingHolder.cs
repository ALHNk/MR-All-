using UnityEngine;

public class StreamingHolder : MonoBehaviour
{
	public Transform player;
	private Vector3 offset, holderOffset;
	public float limit = 5f;
	public float smoothTime = 0.3f; 
	
	private float lastX, lastZ, y;
	private Vector3 targetPosition;
	private Vector3 velocity = Vector3.zero;
	private bool isMoving = false;
	public bool isLocked = false;
	
	public GameObject holderPositionChanger;
	
	private Vector3 initialChangerScale;
	private Vector3 initialHolderScale;
	
	void Start()
	{
		offset = transform.position - player.transform.position;
		lastX = player.position.x;
		lastZ = player.position.z;
		y = transform.position.y;
		targetPosition = transform.position;
		holderPositionChanger.SetActive(false);
		holderOffset = transform.position - holderPositionChanger.transform.position;
		
		initialHolderScale = transform.localScale;
		initialChangerScale = holderPositionChanger.transform.localScale;
	}
	
	void Update()
	{
		if(isLocked)
		{
			MotionNotLocked();
			holderPositionChanger.transform.position = transform.position - holderOffset;
		}
		else
		{
			MotionLocked();
		}
	
	}
	
	private void MotionNotLocked()
	{
		if (Mathf.Abs(player.transform.position.x - lastX) > limit || 
			Mathf.Abs(player.transform.position.z - lastZ) > limit)
		{
			// Set new target position
			targetPosition = new Vector3(
				player.transform.position.x + offset.x, 
				y, 
				player.transform.position.z + offset.z
			);
			
			lastX = player.position.x;
			lastZ = player.position.z;
			isMoving = true;
		}
		
		// Smoothly move towards target
		if (isMoving)
		{
			transform.position = Vector3.SmoothDamp(
				transform.position, 
				targetPosition, 
				ref velocity,
				smoothTime
			);
			
			if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
			{
				transform.position = targetPosition;
				velocity = Vector3.zero;
				isMoving = false;
			}
		}
	}
	
	private void ApplyRelativeScale()
	{
		Vector3 changerScale = holderPositionChanger.transform.localScale;

		Vector3 scaleMultiplier = new Vector3(
			changerScale.x / initialChangerScale.x,
			changerScale.y / initialChangerScale.y,
			changerScale.z / initialChangerScale.z
		);

		transform.localScale = Vector3.Scale(initialHolderScale, scaleMultiplier);
	}
	private void MotionLocked()
	{
		transform.position = holderPositionChanger.transform.position + holderOffset;
		ApplyRelativeScale();
	}
}