using UnityEngine;

public class ModePlane : MonoBehaviour
{
	public GlobalVaules globalValues;
	private bool isMaterialChanged;
	private Renderer renderer;
	public TMPro.TMP_Text modeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		renderer = GetComponent<Renderer>();
		renderer.material.color = Color.yellow;
		isMaterialChanged = true;
		modeText.text = "DS";
    }

    // Update is called once per frame
    void Update()
    {
	    if(globalValues.isDifferentialSteerMode != isMaterialChanged)
	    {
	    	if(globalValues.isDifferentialSteerMode)
	    	{
	    		renderer.material.color = Color.yellow;
	    		modeText.text = "DS";
	    	}
		    else	    	
		    {
	    		renderer.material.color = Color.blue;
	    		modeText.text = "WB";
	    	}
	    	isMaterialChanged = globalValues.isDifferentialSteerMode;
	    	
	    }

	    
    }
}
