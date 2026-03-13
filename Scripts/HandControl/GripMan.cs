using UnityEngine;
using Oculus.Interaction.Input;
public class GripMan : MonoBehaviour
{
	public Hand Hand;
	public TMPro.TMP_Text testText;
	private float gripPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    gripPower = 0;
    }

    // Update is called once per frame
    void Update()
    {
	   gripPower = 
		    Hand.GetFingerPinchStrength(HandFinger.Index) +
		    Hand.GetFingerPinchStrength(HandFinger.Middle);

	    gripPower /= 2f;
	    
	    testText.text = gripPower.ToString("F3");
    }
}
