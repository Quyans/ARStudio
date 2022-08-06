using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace rc_hololens
{
	public class OBJLoader : MonoBehaviour
	{
		[HideInInspector]
		public Transform currentMachineModelTransform;

		private List<GameObject> ggameObjects = new List<GameObject>();

		private void Awake()
		{
			LoadObjFile(Path.Combine(Application.persistentDataPath, "model.obj"));
			currentMachineModelTransform = GameObject.Find("MachineModel").transform;
			foreach (GameObject ggameObject in ggameObjects)
			{
				ggameObject.transform.Rotate(90f, 0f, 0f);
				ggameObject.transform.parent = currentMachineModelTransform;
			}
		}

		private  void LoadObjFile(string filename)
		{
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<int> list3 = new List<int>();
			Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
			Material material = null;
			Material material2 = new Material(Shader.Find("Standard"));
			SortedList<int, List<int>> sortedList = new SortedList<int, List<int>>();
			string[] array = Encoding.ASCII.GetString(File.ReadAllBytes(filename)).Split('\n');
			foreach (string text in array)
			{
				if (text.Length <= 0 || text[0] == '#')
				{
					continue;
				}
				string[] array2 = text.Split(' ', '/');
				if (array2[0] == "mtllib")
				{
					dictionary = LoadMtlFile(Path.Combine(Application.persistentDataPath, "model.obj.mtl"));
				}
				else if (array2[0] == "vn")
				{
					list2.Add(GetVector3(array2));
				}
				else if (array2[0] == "v")
				{
					list.Add(GetVector3(array2));
				}
				else if (array2[0] == "usemtl")
				{
					if (list3.Count != 0)
					{
						int num = 0;
						List<Vector3> list4 = new List<Vector3>();
						List<Vector3> list5 = new List<Vector3>();
						foreach (KeyValuePair<int, List<int>> item in sortedList)
						{
							list4.Add(list[item.Key]);
							list5.Add(list2[item.Key]);
							foreach (int item2 in item.Value)
							{
								list3[item2] = num;
							}
							num++;
						}
						Mesh mesh = new Mesh();
						mesh.vertices = list4.ToArray();
						mesh.normals = list5.ToArray();
						mesh.triangles = list3.ToArray();
						GameObject gameObject = new GameObject();
						MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
						MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
						meshFilter.mesh = mesh;
						meshRenderer.material = material;
						ggameObjects.Add(gameObject);
						list3 = new List<int>();
						sortedList = new SortedList<int, List<int>>();
					}
					material = ((!dictionary.ContainsKey(array2[1])) ? material2 : dictionary[array2[1]]);
				}
				else if (array2[0] == "f")
				{
					int num2 = int.Parse(array2[1]) - 1;
					list3.Add(num2);
					if (!sortedList.ContainsKey(num2))
					{
						sortedList.Add(num2, new List<int>());
					}
					sortedList[num2].Add(list3.Count - 1);
					num2 = int.Parse(array2[4]) - 1;
					list3.Add(num2);
					if (!sortedList.ContainsKey(num2))
					{
						sortedList.Add(num2, new List<int>());
					}
					sortedList[num2].Add(list3.Count - 1);
					num2 = int.Parse(array2[7]) - 1;
					list3.Add(num2);
					if (!sortedList.ContainsKey(num2))
					{
						sortedList.Add(num2, new List<int>());
					}
					sortedList[num2].Add(list3.Count - 1);
				}
			}
			if (list3.Count == 0)
			{
				return;
			}
			int num3 = 0;
			List<Vector3> list6 = new List<Vector3>();
			List<Vector3> list7 = new List<Vector3>();
			foreach (KeyValuePair<int, List<int>> item3 in sortedList)
			{
				list6.Add(list[item3.Key]);
				list7.Add(list2[item3.Key]);
				foreach (int item4 in item3.Value)
				{
					list3[item4] = num3;
				}
				num3++;
			}
			Mesh mesh2 = new Mesh();
			mesh2.vertices = list6.ToArray();
			mesh2.normals = list7.ToArray();
			mesh2.triangles = list3.ToArray();
			GameObject gameObject2 = new GameObject();
			MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
			meshFilter2.mesh = mesh2;
			meshRenderer2.material = material;
			ggameObjects.Add(gameObject2);
		}

		private Dictionary<string, Material> LoadMtlFile(string filename)
		{
			Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
			string key = null;
			Material material = null;
			string[] array = Encoding.ASCII.GetString(File.ReadAllBytes(filename)).Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(' ');
				if (array2[0] == "newmtl")
				{
					if (material != null)
					{
						dictionary.Add(key, material);
					}
					material = new Material(Shader.Find("Standard"));
					key = array2[1];
				}
				else if (array2[0] == "Ka")
				{
					material.SetColor("_EmissionColor", GetColors(array2));
					material.EnableKeyword("_EMISSION");
				}
				else if (array2[0] == "Kd")
				{
					material.SetColor("_Color", GetColors(array2));
				}
				else if (array2[0] == "Ks")
				{
					material.SetColor("_SpecColor", GetColors(array2));
				}
			}
			if (material != null)
			{
				dictionary.Add(key, material);
			}
			return dictionary;
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
	}
}
