using UnityEngine;
using UnityEngine.SceneManagement;

public class Menumanagement : MonoBehaviour
{
	public void OpenHandConrolLevel()
	{
		SceneManager.LoadScene("HandControl");
	}
	public void OpenMotorControlLevel()
	{
		SceneManager.LoadScene("MotorControl");
	}
	public void ExitApp()
	{
		Application.Quit();
	}
}
