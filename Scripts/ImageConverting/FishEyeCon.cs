using UnityEngine;

public class FishEyeCon : MonoBehaviour
{
	private ComputeShader shader;
	private RenderTexture output;
	private int kernel;
    
	void Awake()
	{
		// Load shader from Resources - each instance shares the shader but has its own RenderTexture
		ComputeShader shaderTemplate = Resources.Load<ComputeShader>("FishEyeToPerspective");
		if (shaderTemplate == null)
		{
			Debug.LogError("FishEyeToPerspective shader not found in Resources!");
			return;
		}
		shader = Instantiate(shaderTemplate);
		kernel = shader.FindKernel("FishEyeToPerspective");
	}
    
	public Texture2D Convert(Texture2D input, Texture2D existingOutput = null, int width = 640, int height = 480)
	{
		if (shader == null)
		{
			Debug.LogError("Shader not initialized!");
			return existingOutput;
		}
        
		// Create or recreate RenderTexture if needed
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
        
		// Set shader parameters
		shader.SetTexture(kernel, "Source", input);
		shader.SetTexture(kernel, "Result", output);
		shader.SetVector("sourceSize", new Vector2(input.width, input.height));
		shader.SetVector("resultSize", new Vector2(width, height));
		shader.SetFloat("radius", Mathf.Min(input.width, input.height) * 0.5f);
        
		// Dispatch compute shader
		int threadGroupsX = Mathf.CeilToInt(width / 8.0f);
		int threadGroupsY = Mathf.CeilToInt(height / 8.0f);
		shader.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);
        
		// Reuse existing texture or create new one
		Texture2D tex = existingOutput;
		if (tex == null || tex.width != width || tex.height != height)
		{
			if (tex != null)
				DestroyImmediate(tex);
			tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		}
        
		// Read pixels from RenderTexture - save and restore active RT to avoid conflicts
		RenderTexture previousActive = RenderTexture.active;
		RenderTexture.active = output;
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		RenderTexture.active = previousActive; // CRITICAL: Restore previous state
        
		return tex;
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
    
	public Texture2D[] SplitInto3(Texture2D source)
	{
		int partWidth = source.width / 3;
		int height = source.height;
        
		Texture2D left = new Texture2D(partWidth, height, TextureFormat.RGB24, false);
		Texture2D mid = new Texture2D(partWidth, height, TextureFormat.RGB24, false);
		Texture2D right = new Texture2D(partWidth, height, TextureFormat.RGB24, false);
        
		left.SetPixels(source.GetPixels(0, 0, partWidth, height));
		mid.SetPixels(source.GetPixels(partWidth, 0, partWidth, height));
		right.SetPixels(source.GetPixels(partWidth * 2, 0, partWidth, height));
        
		left.Apply();
		mid.Apply();
		right.Apply();
        
		return new Texture2D[] { left, mid, right };
	}
}