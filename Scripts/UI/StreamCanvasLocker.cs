using UnityEngine;

public class StreamCanvasLocker : MonoBehaviour
{
	public StreamingHolder holder;
	public GameObject holderPositionChanger;
	private bool isLocked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(holder == null)
	    {
	    	holder =  FindObjectOfType<StreamingHolder>();
	    }
    }

	public void LockClicked()
	{
		holderPositionChanger.SetActive(isLocked);
		holder.isLocked = !isLocked;
		isLocked = !isLocked;
	}

}
