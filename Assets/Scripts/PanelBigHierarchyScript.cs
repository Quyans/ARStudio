using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PanelBigHierarchyScript : MonoBehaviour
{
	[HideInInspector]
	public Transform rootTrans;

	[HideInInspector]
	public Vector2 sceneItemOffsetMin;

	[HideInInspector]
	public Vector2 sceneItemOffsetMinMax;

	[HideInInspector]
	public int nextSceneItemPos;

	[HideInInspector]
	public Vector2 sceneItemSpriteOffsetMin;

	[HideInInspector]
	public Vector2 sceneItemSpriteOffsetMinMax;

	[HideInInspector]
	public bool originalButtonUsed;

	[HideInInspector]
	public Vector2 sceneItemTextOffsetMax;

	private Quaternion rotateZ90;

	[HideInInspector]
	public bool nowDraging;

	[HideInInspector]
	public GameObject dragBegin;

	[HideInInspector]
	public GameObject dragEnd;

	[HideInInspector]
	public List<GameObject> markers;
	public List<GameObject> cubes;
	private void Awake()
	{
		GameObject gameObject = new GameObject();
		rootTrans = gameObject.transform;
		sceneItemOffsetMin.x = 0f;
		sceneItemOffsetMin.y = -30f;
		sceneItemOffsetMinMax.x = 230f;
		sceneItemOffsetMinMax.y = 30f;
		nextSceneItemPos = 1;
		sceneItemSpriteOffsetMin.x = 15f;
		sceneItemSpriteOffsetMin.y = 0f;
		sceneItemSpriteOffsetMinMax.x = 30f;
		sceneItemSpriteOffsetMinMax.y = 30f;
		originalButtonUsed = false;
		sceneItemTextOffsetMax.x = 230f;
		sceneItemTextOffsetMax.y = 30f;
		rotateZ90.x = 0f;
		rotateZ90.y = 0f;
		rotateZ90.z = 1f / Mathf.Sqrt(2f);
		rotateZ90.w = rotateZ90.z;
		nowDraging = false;
		dragBegin = null;
		dragEnd = null;
		markers = new List<GameObject>();
		cubes = new List<GameObject>();
	}

	public void RedrawSceneItemButton()
	{
		nextSceneItemPos = 1;
		DrawSceneItemsRecursively(rootTrans);
	}

	private void DrawSceneItemsRecursively(Transform objTrans)
	{
		int childCount = objTrans.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = objTrans.GetChild(i);
			SceneItemScript component = child.GetComponent<SceneItemScript>();
			RectTransform counterButtonRectTrans = component.counterButtonRectTrans;
			counterButtonRectTrans.offsetMin = sceneItemOffsetMin * nextSceneItemPos;
			counterButtonRectTrans.offsetMax = counterButtonRectTrans.offsetMin + sceneItemOffsetMinMax;
			nextSceneItemPos++;
			RectTransform obj = counterButtonRectTrans.GetChild(1) as RectTransform;
			obj.offsetMin = sceneItemSpriteOffsetMin * (component.spritePos + 2);
			obj.offsetMax = sceneItemTextOffsetMax;
			if (child.childCount != 0)
			{
				RectTransform rectTransform = counterButtonRectTrans.GetChild(0) as RectTransform;
				rectTransform.offsetMin = sceneItemSpriteOffsetMin * component.spritePos;
				rectTransform.offsetMax = rectTransform.offsetMin + sceneItemSpriteOffsetMinMax;
				if (component.drawChildren)
				{
					rectTransform.rotation = Quaternion.identity;
					DrawSceneItemsRecursively(child);
				}
				else
				{
					rectTransform.rotation = rotateZ90;
				}
			}
		}
	}

	public void CreateMaker()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.name = "Marker" + (markers.Count + 1);
		SceneItemScript sceneItemScript = gameObject.AddComponent<SceneItemScript>();
		sceneItemScript.counterButtonImageRectTrans.rotation = rotateZ90;
		sceneItemScript.imageCom.color = Color.clear;
		sceneItemScript.counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = gameObject.name;
		gameObject.GetComponent<SceneItemScript>().isMarker = true;
		sceneItemScript.markerID = markers.Count + 1 + 212;
		gameObject.transform.SetParent(rootTrans);
		markers.Add(gameObject);
	}

	
	public void CreateCube()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Cube" + (cubes.Count + 1);
		//UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath("Assets/Materials/cube.mat");
		UnityEngine.Object obj;

		if (cubes.Count % 3 == 1)
		{
			#if UNITY_EDITOR
				obj = AssetDatabase.LoadMainAssetAtPath("Assets/Materials/cube1.mat");	
			#else
				obj = Resources.Load("Assets/Materials/cube2.mat");
			#endif
		}
		else if (cubes.Count % 3 == 2)
		{
			#if UNITY_EDITOR
				obj = AssetDatabase.LoadMainAssetAtPath("Assets/Materials/cube2.mat");	
			#else
				obj = Resources.Load("Assets/Materials/cube.mat");
			#endif
		}
		else
		{
			#if UNITY_EDITOR
						obj = AssetDatabase.LoadMainAssetAtPath("Assets/Materials/cube3.mat");	
			#else
							obj = Resources.Load("Assets/Materials/cube.mat");
			#endif
		}
		
		Material mat = obj as Material;
		// GetComponent<Renderer>().material = mat;
		gameObject.GetComponent<MeshRenderer>().material = mat;
					
		// cube1.transform.position=new Vector3(边长 * Mathf.Cos(1.0472f), 0, 0);
		// GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		// cube2.transform.position = new Vector3(-1*5 * Mathf.Cos(1.0472f), 0, 0);
		// GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		// cube3.transform.position = new Vector3(0, 5 * Mathf.Sin(1.0472f), 0);
		

		SceneItemScript sceneItemScript = gameObject.AddComponent<SceneItemScript>();
		sceneItemScript.counterButtonImageRectTrans.rotation = rotateZ90;
		sceneItemScript.imageCom.color = Color.clear;
		sceneItemScript.counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = gameObject.name;
		gameObject.GetComponent<SceneItemScript>().isCube = true;
		
		sceneItemScript.cubeID = cubes.Count + 1 ; //从1开始计算
		gameObject.transform.SetParent(rootTrans);
		cubes.Add(gameObject);
	}
	public void ExitGame()
    	{
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
        }
	public void SaveScene()
	{
		string text = "D:/qys/BHU/science/AR/StudioSavedScene";
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Target path: " + text);
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Saving scene ...");
		if (Directory.Exists(text + "/models"))
		{
			Directory.Delete(text + "/models", recursive: true);
		}
		Directory.CreateDirectory(text + "/models");
		FileStream fileStream = File.Create(text + "/scene.xml");
		StreamWriter streamWriter = new StreamWriter(fileStream);
		ForSaveScene(streamWriter, rootTrans, 0, "", text + "/models/", root: true);
		streamWriter.Close();
		fileStream.Close();
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Scene saved.");
	}
	
	private void ForSaveScene(StreamWriter sr, Transform tr, int tabn, string nam, string path, bool root = false)
	{
		if (root)
		{
			sr.WriteLine("<Scene>");
		}
		else
		{
			for (int i = 0; i < tabn; i++)
			{
				sr.Write("\t");
			}
			if (tr.GetComponent<SceneItemScript>().isMarker)
			{
				sr.Write("<Marker id = \"");
				sr.Write(tr.GetComponent<SceneItemScript>().markerID.ToString());
				sr.WriteLine("\">");
			}
			else if (tr.GetComponent<SceneItemScript>().isCube)
			{
				sr.Write("<Cube id = \"");
				sr.Write(tr.GetComponent<SceneItemScript>().cubeID.ToString());
				sr.WriteLine("\">");
				
				Vector3 localPosition = tr.localPosition;
				for (int j = 0; j < tabn + 1; j++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Position x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
				localPosition = tr.localEulerAngles;
				for (int k = 0; k < tabn + 1; k++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Rotation x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
				localPosition = tr.localScale;
				for (int l = 0; l < tabn + 1; l++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Scale x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
			}
			else
			{
				if (tr.GetComponent<MeshFilter>() == null)
				{
					sr.WriteLine("<Model path= \"null\">");
				}
				else
				{
					CreateObj(path + nam + tr.name + ".obj", tr.gameObject);
					sr.WriteLine("<Model path= \"/models/" + nam + tr.name + ".obj\">");
				}
				Vector3 localPosition = tr.localPosition;
				for (int j = 0; j < tabn + 1; j++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Position x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
				localPosition = tr.localEulerAngles;
				for (int k = 0; k < tabn + 1; k++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Rotation x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
				localPosition = tr.localScale;
				for (int l = 0; l < tabn + 1; l++)
				{
					sr.Write("\t");
				}
				sr.WriteLine("<Scale x=\"" + localPosition.x + "\" y=\"" + localPosition.y + "\" z=\"" + localPosition.z + "\"/>");
			}
			sr.Flush();
		}
		int childCount = tr.childCount;
		if (root)
		{
			for (int m = 0; m < childCount; m++)
			{
				ForSaveScene(sr, tr.GetChild(m), tabn + 1, "", path);
			}
		}
		else if (tr.GetComponent<SceneItemScript>().isMarker)
		{
			for (int n = 0; n < childCount; n++)
			{
				ForSaveScene(sr, tr.GetChild(n), tabn + 1, tr.GetComponent<SceneItemScript>().markerID + "_", path);
			}
		}
		else
		{
			for (int num = 0; num < childCount; num++)
			{
				ForSaveScene(sr, tr.GetChild(num), tabn + 1, nam + tr.name + "_", path);
			}
		}
		if (root)
		{
			sr.WriteLine("</Scene>");
			return;
		}
		for (int num2 = 0; num2 < tabn; num2++)
		{
			sr.Write("\t");
		}
		if (tr.GetComponent<SceneItemScript>().isMarker)
		{
			sr.WriteLine("</Marker>");
		}
		else if (tr.GetComponent<SceneItemScript>().isCube)
		{
			sr.WriteLine("</Cube>");
		}
		else
		{
			sr.WriteLine("</Model>");
		}
	}

	private void CreateObj(string path, GameObject gam)
	{
		FileStream fileStream = File.Create(path);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		streamWriter.WriteLine("#717");
		FileInfo fileInfo = new FileInfo(path);
		streamWriter.WriteLine("mtllib " + fileInfo.Name + ".mtl");
		Mesh mesh = gam.GetComponent<MeshFilter>().mesh;
		int i = 0;
		for (int num = mesh.vertices.Length; i < num; i++)
		{
			Vector3 vector = mesh.normals[i];
			streamWriter.WriteLine("vn " + vector.x + " " + vector.y + " " + vector.z);
			vector = mesh.vertices[i];
			streamWriter.WriteLine("v " + vector.x + " " + vector.y + " " + vector.z);
		}
		streamWriter.WriteLine("");
		streamWriter.WriteLine("usemtl material_0");
		int num2 = 0;
		int num3 = mesh.triangles.Length;
		while (num2 < num3)
		{
			streamWriter.Write("f " + (mesh.triangles[num2] + 1) + "//" + (mesh.triangles[num2] + 1) + " ");
			streamWriter.Write(mesh.triangles[num2 + 1] + 1 + "//" + (mesh.triangles[num2 + 1] + 1) + " ");
			streamWriter.WriteLine(mesh.triangles[num2 + 2] + 1 + "//" + (mesh.triangles[num2 + 2] + 1));
			num2 += 2;
			num2++;
		}
		streamWriter.Close();
		fileStream.Close();
		fileStream = File.Create(path + ".mtl");
		streamWriter = new StreamWriter(fileStream);
		streamWriter.WriteLine("newmtl material_0");
		Material material = gam.GetComponent<MeshRenderer>().material;
		Color color = material.GetColor("_EmissionColor");
		streamWriter.WriteLine("Ka " + color.r + " " + color.g + " " + color.b);
		color = material.GetColor("_Color");
		streamWriter.WriteLine("Kd " + color.r + " " + color.g + " " + color.b);
		color = material.GetColor("_SpecColor");
		streamWriter.WriteLine("Ks " + color.r + " " + color.g + " " + color.b);
		streamWriter.Close();
		fileStream.Close();
	}
}
