using UnityEngine;

public class Test : MonoBehaviour
{
	public GameObject obj;
	
	public void PrintTest()
	{
		Debug.LogError("TEST");
	}
	
	public void qtest()
	{
		obj.SetActive(false);
	}
}
