using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ReadObj : MonoBehaviour
{
	private PanelBigHierarchyScript panelBigHierarchyScript;

	private Quaternion rotateZ90;

	private RectTransform panelBigHierarchyRectTrans;

	private void Awake()
	
	{
		panelBigHierarchyScript = GameObject.Find("PanelBigHierarchy").GetComponent<PanelBigHierarchyScript>();
		rotateZ90.x = 0f;
		rotateZ90.y = 0f;
		rotateZ90.z = 1f / Mathf.Sqrt(2f);
		rotateZ90.w = rotateZ90.z;
		panelBigHierarchyRectTrans = panelBigHierarchyScript.transform as RectTransform;
	}

	public void LoadObjFile(string filename)
	{
		int num = 0;
		if (panelBigHierarchyScript.markers.Count == 0)
		{
			panelBigHierarchyScript.CreateMaker();
		}
		GameObject gameObject = new GameObject();
		gameObject.name = Path.GetFileNameWithoutExtension(filename);
		SceneItemScript sceneItemScript = gameObject.AddComponent<SceneItemScript>();
		sceneItemScript.counterButtonImageRectTrans.rotation = rotateZ90;
		sceneItemScript.imageCom.color = Color.white;
		sceneItemScript.spritePos = 1;
		sceneItemScript.counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = gameObject.name;
		GameObject gameObject2 = panelBigHierarchyScript.markers[panelBigHierarchyScript.markers.Count - 1];
		gameObject2.GetComponent<SceneItemScript>().imageCom.color = Color.white;
		gameObject2.GetComponent<SceneItemScript>().drawChildren = true;
		gameObject.transform.SetParent(gameObject2.transform);
		List<GameObject> gameObjectsList = new List<GameObject>();
        Dictionary<string, Material> materialDict = new Dictionary<string, Material>();

/*		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector2> list4 = new List<Vector2>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector3> list6 = new List<Vector3>();
		List<Vector2> list7 = new List<Vector2>();
		List<int> list8 = new List<int>();
*/

		string[] value = filename.Split('\\');
		filename = string.Join("/", value);
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: OBJ file path: " + filename);
		Stream stream = new FileStream(filename, FileMode.Open);
		StreamReader streamReader = new StreamReader(stream);
        StringReader stringReader = new StringReader(streamReader.ReadToEnd());
        //a = Resources.Load("Assets/Materials/defaultMat.mat");
		//Material defaultMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/defaultMat.mat", typeof(Material));
		Material defaultMaterial =  (Material)Resources.Load("Assets/Materials/defaultMat.mat");
		// 0th iteration, set material dictionary
		for (string line = stringReader.ReadLine(); line != null; line = stringReader.ReadLine())
		{
			string[] splitLine = line.Split(' ');
			if (splitLine[0] == "mtllib")
			{
				materialDict = Toolkit.LoadMtlFromFile(filename.Replace(".obj", ".mtl"));
				break;
			}
		}

		// first iteration
		stream.Position = 0L;
		stringReader = new StringReader(streamReader.ReadToEnd());
		List<Vector3> Normals = new List<Vector3>();
		List<Vector3> Vertices = new List<Vector3>();
		List<Vector2> UVs = new List<Vector2>();
        for (string line = stringReader.ReadLine(); line != null; line = stringReader.ReadLine())
        {
            string[] splitLine = line.Split(' ');
            if (splitLine[0] == "vn") { Normals.Add(Toolkit.Vector3FromStrArray(splitLine)); continue; }
            if (splitLine[0] == "v") { Vertices.Add(Toolkit.Vector3FromStrArray(splitLine, -1.0f)); continue; }
            if (splitLine[0] == "vt") { UVs.Add(Toolkit.Vector2FromStrArray(splitLine)); }
        }
		// Debug.Log("after first iteration...");
		// Debug.Log("v: " + Vertices.Count);
		// Debug.Log("vn: " + Normals.Count);
		// Debug.Log("vt: " + UVs.Count);

		// second iteration
		List<Vector3> NewNormals = new List<Vector3>();
		List<Vector3> NewVertices = new List<Vector3>();
		List<Vector2> NewUVs = new List<Vector2>();
        List<int> Triangles = new List<int>();

        Vector2Int noneUV = new Vector2Int(0, 0);
        if (UVs.Count == 0) UVs.Add(noneUV);

		stream.Position = 0L;
		stringReader = new StringReader(streamReader.ReadToEnd());
		int index = 0;
		Material curMaterial = defaultMaterial;
        for (string line = stringReader.ReadLine(); line != null; line = stringReader.ReadLine())
        {
			// usemtl line
            string[] splitLine = line.Split(' ');
            if (splitLine[0] == "usemtl")
            {
                // deal with last mesh
                if (Triangles.Count != 0)
                {
                    Mesh mesh = new Mesh();
                    mesh.vertices = NewVertices.ToArray();
                    mesh.uv = NewUVs.ToArray();
                    mesh.normals = NewNormals.ToArray();
                    mesh.triangles = Triangles.ToArray();
                    GameObject newGameObject = new GameObject();
                    MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();
                    meshFilter.mesh = mesh;
                    meshRenderer.material = curMaterial;
                    gameObjectsList.Add(newGameObject);
                }
                // set new mesh material
                if (materialDict.ContainsKey(splitLine[1])) curMaterial = materialDict[splitLine[1]];
                else curMaterial = defaultMaterial;
                NewNormals.Clear();
                NewUVs.Clear();
                NewVertices.Clear();
                Triangles.Clear();
				index = 0;
                continue;
            }

			// f line
            if (splitLine[0] == "f" && splitLine.Length == 4)
            {
                // three vertices info in this face/line
                var v1 = splitLine[1].Split('/');       // [1,1,1] v, uv, vn for v1
                var v2 = splitLine[2].Split('/');       // [2,2,2] v, uv, vn for v2
                var v3 = splitLine[3].Split('/');       // [4,3,4] v, uv, vn for v3

                // add v
                NewVertices.Add(Vertices[Toolkit.FastIntParse(v1[0]) - 1]);
                NewVertices.Add(Vertices[Toolkit.FastIntParse(v2[0]) - 1]);
                NewVertices.Add(Vertices[Toolkit.FastIntParse(v3[0]) - 1]);

                // add uv
                NewUVs.Add(UVs[Toolkit.FastIntParse(v1[1]) - 1]);
                NewUVs.Add(UVs[Toolkit.FastIntParse(v2[1]) - 1]);
                NewUVs.Add(UVs[Toolkit.FastIntParse(v3[1]) - 1]);

                // add vn
                NewNormals.Add(Normals[Toolkit.FastIntParse(v1[2]) - 1]);
                NewNormals.Add(Normals[Toolkit.FastIntParse(v2[2]) - 1]);
                NewNormals.Add(Normals[Toolkit.FastIntParse(v3[2]) - 1]);

                Triangles.Add(index);
                Triangles.Add(index + 1);
                Triangles.Add(index + 2);
                index += 3;
            }
        }

        // last mesh
        if (Triangles.Count != 0)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = NewVertices.ToArray();
            mesh.uv = NewUVs.ToArray();
            mesh.normals = NewNormals.ToArray();
            mesh.triangles = Triangles.ToArray();
            GameObject newGameObject = new GameObject();
            MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = curMaterial;
            gameObjectsList.Add(newGameObject);
        }

		Console.WriteLine("finished");
		streamReader.Close();
		stringReader.Close();
		stream.Close();
		foreach (GameObject item in gameObjectsList)
		{
			item.name = "Child" + num;
			num++;
			SceneItemScript sceneItemScript2 = item.AddComponent<SceneItemScript>();
			sceneItemScript2.spritePos = 2;
			sceneItemScript2.imageCom.color = Color.clear;
			RectTransform counterButtonImageRectTrans = sceneItemScript2.counterButtonImageRectTrans;
			counterButtonImageRectTrans.offsetMin = panelBigHierarchyScript.sceneItemSpriteOffsetMin;
			counterButtonImageRectTrans.offsetMax = panelBigHierarchyScript.sceneItemSpriteOffsetMin + panelBigHierarchyScript.sceneItemSpriteOffsetMinMax;
			RectTransform counterButtonTextRectTrans = sceneItemScript2.counterButtonTextRectTrans;
			counterButtonTextRectTrans.offsetMin = panelBigHierarchyScript.sceneItemSpriteOffsetMin * 2f;
			counterButtonTextRectTrans.offsetMax = panelBigHierarchyScript.sceneItemTextOffsetMax;
			counterButtonTextRectTrans.GetChild(0).GetComponent<Text>().text = item.name;
			sceneItemScript2.HideCanvasGroup();
			item.transform.parent = gameObject.transform;
		}
		Vector2 offsetMin = panelBigHierarchyRectTrans.offsetMin;
		offsetMin.y -= (num + 1) * 30;
		panelBigHierarchyRectTrans.offsetMin = offsetMin;
		panelBigHierarchyScript.RedrawSceneItemButton();
	}

	public static Vector3 GetVector2(string[] substrs)
	{
		float x = float.Parse(substrs[1]);
		float y = float.Parse(substrs[2]);
		return new Vector2(x, y);
	}

	public static Vector3 GetVector3(string[] substrs)
	{
		float x = float.Parse(substrs[1]);
		float y = float.Parse(substrs[2]);
		float z = float.Parse(substrs[3]);
		return new Vector3(x, y, z);
	}

	public static Color GetColors(string[] substrs)
	{
		float r = float.Parse(substrs[1]);
		float g = float.Parse(substrs[2]);
		float b = float.Parse(substrs[3]);
		return new Color(r, g, b);
	}

	public static Texture2D GetTexture(string imgpath)
	{
		byte[] data = File.ReadAllBytes(imgpath);
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		return texture2D;
	}
}
