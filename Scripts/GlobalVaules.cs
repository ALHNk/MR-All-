using UnityEngine;

public class GlobalVaules : MonoBehaviour
{
	public bool isDifferentialSteerMode = true;
	
	public void SetDifferentialMode()
	{
		isDifferentialSteerMode = !isDifferentialSteerMode;
	}
}
