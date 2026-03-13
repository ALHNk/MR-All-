using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	private bool initial = true;
	private Renderer renderer;
	
	public void Start()
	{
		renderer = GetComponent<Renderer>();
	}
	
	public void ChangeOwnColor()
	{
		if(initial)
		{
			renderer.material.color = Color.yellow;
		}
		else
		{
			renderer.material.color = Color.blue;
		}
		
		initial = !initial;
	}
}
