using UnityEngine;

public class StreamingHolder : MonoBehaviour
{
	public Transform player;
	private Vector3 offset;
	public float limit = 5f;
	public float smoothTime = 0.3f; 
	
	private float lastX, lastZ, y;
	private Vector3 targetPosition;
	private Vector3 velocity = Vector3.zero;
	private bool isMoving = false;
	
	void Start()
	{
		offset = transform.position - player.transform.position;
		lastX = player.position.x;
		lastZ = player.position.z;
		y = transform.position.y;
		targetPosition = transform.position;
	}
	
	void Update()
	{
		// Check if player moved beyond limit
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
}