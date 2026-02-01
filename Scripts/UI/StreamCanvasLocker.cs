using UnityEngine;
using TMPro;
public class StreamCanvasLocker : MonoBehaviour
{
	public StreamingHolder holder;
	public GameObject holderPositionChanger;
	private bool isLocked = false;
	
	private TMP_Text lockerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(holder == null)
	    {
	    	holder =  FindObjectOfType<StreamingHolder>();
	    }
	    lockerText = GetComponentInChildren<TMP_Text>();
    }

	public void LockClicked()
	{
		holder.isLocked = !isLocked;
		isLocked = !isLocked;
		holder.SetOffset();
		holderPositionChanger.SetActive(isLocked);
		if(isLocked)
		{
			lockerText.text = "Unlock";
		}
		else
		{
			lockerText.text = "Lock";
		}
	}

}
