using UnityEngine;

public class FishEyeCon : MonoBehaviour
{
	private ComputeShader shader;
	private RenderTexture output;
	private int kernel;
	
	[Header("Fisheye Parameters")]
	[Range(0.5f, 2.5f)]
	public float strength = 1.5f;  
	
	[Range(60f, 180f)]
	public float fov = 140f;  
    
	void Awake()
	{
		ComputeShader shaderTemplate = Resources.Load<ComputeShader>("FishEyeToPerspective");
		if (shaderTemplate == null)
		{
			Debug.LogError("FishEyeToPerspective shader not found in Resources!");
			return;
		}
		shader = Instantiate(shaderTemplate);
		kernel = shader.FindKernel("FishEyeToPerspective");
	}
    
	public Texture Convert(Texture2D input, int width = 640, int height = 480)
	{
		if (shader == null)
		{
			Debug.LogError("Shader not initialized!");
			return input;
		}
        
		if (output == null || output.width != width || output.height != height)
		{
			if (output != null)
			{
				output.Release();
				DestroyImmediate(output);
			}
                
			output = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			output.enableRandomWrite = true;
			output.filterMode = FilterMode.Bilinear;
			output.Create();
		}
        
		shader.SetTexture(kernel, "Source", input);
		shader.SetTexture(kernel, "Result", output);
		shader.SetVector("sourceSize", new Vector2(input.width, input.height));
		shader.SetVector("resultSize", new Vector2(width, height));
		shader.SetFloat("radius", Mathf.Min(input.width, input.height) * 0.5f);
		shader.SetFloat("fov", fov * Mathf.Deg2Rad);
		shader.SetFloat("strength", strength);
        
		int threadGroupsX = Mathf.CeilToInt(width / 8.0f);
		int threadGroupsY = Mathf.CeilToInt(height / 8.0f);
		shader.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);
        
		return output;
	}
    
	void OnDestroy()
	{
		if (output != null)
		{
			output.Release();
			DestroyImmediate(output);
		}
		if (shader != null)
		{
			DestroyImmediate(shader);
		}
	}
}