using UnityEngine;
using UnityEngine.UI;

public class TestForHand : MonoBehaviour
{
	public OVRHand rightHand, leftHand;
	private OVRHand hand;
	
	public Text handPositionText;
	public GameObject moveableCube;
	public GameObject movingArea;
	private Collider movingAreaCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		movingAreaCollider = movingArea.GetComponent<Collider>();
		if(PlayerPrefs.HasKey("IsDexter"))
		{
			if(PlayerPrefs.GetInt("IsDexter") == 1)
			{
				hand = rightHand;
			}
			else
			{
				hand = leftHand;
			}
		}
		hand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
	    Vector3 cubePosition = moveableCube.transform.localPosition;
	    handPositionText.text = $"Hand position X: {cubePosition.x} Y: {cubePosition.y} Z: {cubePosition.z}";
	    
	    if (hand != null && movingAreaCollider != null)
	    {
		    Vector3 handWorldPosition = hand.transform.position;

		    if (IsHandInsideMovingArea(handWorldPosition))
		    {
			    OnHandStayInArea();  
		    }
	    }
    }
    
	bool IsHandInsideMovingArea(Vector3 handPosition)
	{
		return movingAreaCollider.bounds.Contains(handPosition);
	}

	void OnHandStayInArea()
	{
		moveableCube.transform.position = hand.transform.position;
	}
    
    
	public void SetDexter(bool dexter)
	{
		if(dexter)
		{
			hand = rightHand;
		}
		else
		{
			hand = leftHand;
		}
	}
	
}
