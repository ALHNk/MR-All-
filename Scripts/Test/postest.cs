using UnityEngine;

public class postest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    Debug.LogError(transform.rotation.eulerAngles.y - 45f);
    }
}
