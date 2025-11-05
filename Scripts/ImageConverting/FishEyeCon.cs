using UnityEngine;

public class FishEyeCon : MonoBehaviour
{
	public ComputeShader shader;
	
	RenderTexture output;

	public Texture2D Convert(Texture2D input, int width = 640, int height = 480)
	{
		if (output == null || output.width != width || output.height != height)
		{
			output = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			output.enableRandomWrite = true;
			output.Create();
		}

		int kernel = shader.FindKernel("FishEyeToPerspective");

		shader.SetTexture(kernel, "Source", input);
		shader.SetTexture(kernel, "Result", output);

		shader.SetVector("sourceSize", new Vector2(input.width, input.height));
		shader.SetVector("resultSize", new Vector2(width, height));
		shader.SetFloat("radius", Mathf.Min(input.width, input.height) * 0.5f);

		shader.Dispatch(kernel, width / 8, height / 8, 1);

		RenderTexture.active = output;
		Texture2D tex = new Texture2D(width, height);
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();

		return tex;
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
