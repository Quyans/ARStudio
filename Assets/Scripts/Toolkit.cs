using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class Toolkit
{
	public static string Clean(this string str)
	{
		string text = str.Replace('\t', ' ');
		while (text.Contains("  "))
		{
			text = text.Replace("  ", " ");
		}
		return text.Trim();
	}

	public static float FastFloatParse(string input)
	{
		if (input.Contains("e") || input.Contains("E"))
		{
			return float.Parse(input, CultureInfo.InvariantCulture);
		}
		float num = 0f;
		int num2 = 0;
		int length = input.Length;
		if (length == 0)
		{
			return float.NaN;
		}
		char c = input[0];
		float num3 = 1f;
		if (c == '-')
		{
			num3 = -1f;
			num2++;
			if (num2 >= length)
			{
				return float.NaN;
			}
		}
		while (true)
		{
			if (num2 >= length)
			{
				return num3 * num;
			}
			c = input[num2++];
			if (c < '0' || c > '9')
			{
				break;
			}
			num = num * 10f + (float)(c - 48);
		}
		if (c != '.' && c != ',')
		{
			return float.NaN;
		}
		float num4 = 0.1f;
		while (num2 < length)
		{
			c = input[num2++];
			if (c < '0' || c > '9')
			{
				return float.NaN;
			}
			num += (float)(c - 48) * num4;
			num4 *= 0.1f;
		}
		return num3 * num;
	}

	public static int FastIntParse(string input)
	{
		if (input.Length == 0) return 1;
		int num = 0;
		bool flag = input[0] == '-';
		for (int i = (flag ? 1 : 0); i < input.Length; i++)
		{
			num = num * 10 + (input[i] - 48);
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public static Vector3 Vector3FromStrArray(string[] cmps, float coef = 1f)
	{
		float x = FastFloatParse(cmps[1]) * coef;
		float y = FastFloatParse(cmps[2]);
		float z = FastFloatParse(cmps[3]);
		return new Vector3(x, y, z);
	}

	public static Vector2 Vector2FromStrArray(string[] cmps)
	{
		float x = FastFloatParse(cmps[1]);
		float y = FastFloatParse(cmps[2]);
		return new Vector2(x, y);
	}

	public static Color ColorFromStrArray(string[] cmps, float scalar = 1f)
	{
		float r = FastFloatParse(cmps[1]) * scalar;
		float g = FastFloatParse(cmps[2]) * scalar;
		float b = FastFloatParse(cmps[3]) * scalar;
		return new Color(r, g, b);
	}

	public static Texture2D LoadTextureFromImage(string path)
	{
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			string text = Path.GetExtension(path).ToLower();
			Path.GetFileName(path);
			switch (text)
			{
			case ".png":
			case ".jpg":
			case ".jpeg":
			{
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(data);
				return texture2D;
			}
			}
		}
		return null;
	}

	public static void EnableMtlTrans(Material mtl)
	{
		mtl.SetFloat("_Mode", 3f);
		mtl.SetInt("_SrcBlend", 5);
		mtl.SetInt("_DstBlend", 10);
		mtl.SetInt("_ZWrite", 0);
		mtl.DisableKeyword("_ALPHATEST_ON");
		mtl.EnableKeyword("_ALPHABLEND_ON");
		mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		mtl.renderQueue = 3000;
	}

	public static Dictionary<string, Material> LoadMtlFromFile(string filename)
	{
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Material file path: " + filename);
		Stream stream = new FileStream(filename, FileMode.Open);
		StringReader stringReader = new StringReader(new StreamReader(stream).ReadToEnd());
		Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
		Material material = null;
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = text.Clean();
				string[] array = text.Split(' ');
				if (text2[0] != '#' && array.Length >= 2)
				{
					if (array[0] == "newmtl")
					{
						string text3 = array[1];
						// Material material3 = (dictionary[text3] = new Material(Shader.Find("Custom/StudioStandardShader"))
						Material material3 = (dictionary[text3] = new Material(Shader.Find("Standard"))
						{
							name = text3
						});
						material = material3;
					}
					else if (!(material == null))
					{
						if (array[0] == "Kd" || array[0] == "kd")
						{
							material.SetColor("_Color", ColorFromStrArray(array));
						}
						else if (array[0] == "map_Kd" || array[0] == "map_kd")
						{
							string text4 = array[1];
							Texture2D texture2D = LoadTextureFromImage(text4);
							GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Using image: " + text4);
							material.SetTexture("_MainTex", texture2D);
							if (texture2D != null && texture2D.format == TextureFormat.ARGB32)
							{
								EnableMtlTrans(material);
							}
						}
						else if (array[0] == "Ks" || array[0] == "ks")
						{
							material.SetColor("_SpecColor", ColorFromStrArray(array));
						}
						else if (array[0] == "Ka" || array[0] == "ka")
						{
							material.SetColor("_EmissionColor", ColorFromStrArray(array, 0.05f));
							material.EnableKeyword("_EMISSION");
						}
						else if (array[0] == "Ns" || array[0] == "ns")
						{
							float num = FastFloatParse(array[1]);
							num /= 1000f;
							material.SetFloat("_Glossiness", num);
						}
					}
				}
			}
		}
		stream.Close();
		return dictionary;
	}
}
