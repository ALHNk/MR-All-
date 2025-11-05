using UnityEngine;
using UnityEngine.UI;

public class TakeFromArm : MonoBehaviour
{
	public Text info;
	public float minDifference = 3f;
	public Animator anim;
	public Send send;
	
	//public Transform reference; // сюда кидаешь Cube

	//private Transform shoulder, elbow;

	//void Start()
	//{
	//	shoulder = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
	//	elbow = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
	//}

	//void Update()
	//{
	//	// Плечо относительно куба
	//	Quaternion shoulderLocal = Quaternion.Inverse(reference.rotation) * shoulder.rotation;
	//	// Локоть относительно куба
	//	Quaternion elbowLocal = Quaternion.Inverse(reference.rotation) * elbow.rotation;

	//	Vector3 shoulderAngles = shoulderLocal.eulerAngles;
	//	Vector3 elbowAngles = elbowLocal.eulerAngles;

	//	info.text =
	//		$"Shoulder (ref cube) → X:{shoulderAngles.x:F1}° Y:{shoulderAngles.y:F1}° Z:{shoulderAngles.z:F1}°\n" +
	//		$"Elbow (ref cube) → X:{elbowAngles.x:F1}° Y:{elbowAngles.y:F1}° Z:{elbowAngles.z:F1}°";
	//}
	
	
	
	
	
	
	
	
	
	
	
	private Transform plecho, lokot, chest;
	private float xPrevPlecho, yPrevPlecho, zPrevPlecho, xPrevLokot, yPrevLokot, zPrevLokot;
	Vector3 prevChest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    //plecho = GameObject.Find("Right_UpperArm-tPose");
	    //lokot = GameObject.Find("Right_LowerArm-tPose");
	    
	    plecho = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
	    lokot = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
	    chest = anim.GetBoneTransform(HumanBodyBones.Chest);
	    prevChest = chest.position;
	    xPrevPlecho = yPrevPlecho = zPrevPlecho = 0f;
	    xPrevLokot = yPrevLokot = zPrevLokot = 0f;
    }

    // Update is called once per frame
    void Update()
    {
	    RotationAbsolutely();
    }
	public void RotationAbsolutely()
	{
		float xPlechoDelta = plecho.rotation.eulerAngles.x - xPrevPlecho;
		float yPlechoDelta = plecho.rotation.eulerAngles.y - yPrevPlecho;
		float zPlechoDelta = plecho.rotation.eulerAngles.z - zPrevPlecho;

		float xLokotDelta = lokot.rotation.eulerAngles.x - xPrevLokot;
		float yLokotDelta = lokot.rotation.eulerAngles.y - yPrevLokot;
		float zLokotDelta = lokot.rotation.eulerAngles.z - zPrevLokot;
	
		//if(chest.position == prevChest)
		//{
			
			
			if (Mathf.Abs(xPlechoDelta) >= minDifference ||
				Mathf.Abs(yPlechoDelta) >= minDifference ||
				Mathf.Abs(zPlechoDelta) >= minDifference ||
				Mathf.Abs(xLokotDelta) >= minDifference ||
				Mathf.Abs(yLokotDelta) >= minDifference ||
				Mathf.Abs(zLokotDelta) >= minDifference)
			{
				info.text =
					"Plecho → X: " + plecho.rotation.eulerAngles.x.ToString("F2") +
					" | Y: " + plecho.rotation.eulerAngles.y.ToString("F2") +
					" | Z: " + plecho.rotation.eulerAngles.z.ToString("F2") +
					"\nLokot → X: " + lokot.rotation.eulerAngles.x.ToString("F2") +
					" | Y: " + lokot.rotation.eulerAngles.y.ToString("F2") +
					" | Z: " + lokot.rotation.eulerAngles.z.ToString("F2") +
					" | Chest: " + chest.position.x + " " + chest.position.y +" "+ chest.position.z;
	
				xPrevPlecho = plecho.rotation.eulerAngles.x;
				yPrevPlecho = plecho.rotation.eulerAngles.y;
				zPrevPlecho = plecho.rotation.eulerAngles.z;
	
				xPrevLokot = lokot.rotation.eulerAngles.x;
				yPrevLokot = lokot.rotation.eulerAngles.y;
				zPrevLokot = lokot.rotation.eulerAngles.z;
				
				send.SendValuesTCP(plecho.rotation.eulerAngles.y, 0);
				send.SendValuesTCP(plecho.rotation.eulerAngles.z, 1);
				
			}
		//}
		//else
		//{
		//	prevChest = chest.position;
		//}
		
	}
}
