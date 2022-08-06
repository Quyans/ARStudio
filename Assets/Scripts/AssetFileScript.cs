using System.IO;
using rc_hololens;
using UnityEngine;
using UnityEngine.UI;

public class AssetFileScript : MonoBehaviour
{
	[HideInInspector]
	public string dir;

	private float lastClickTime;

	private ReadObj readObjScript;
	private ReadFbx readFbxScripy;

	private void Awake()
	{
		lastClickTime = Time.time;
		readObjScript = GameObject.Find("UtilityObject").GetComponent<ReadObj>();
		readFbxScripy = GameObject.Find("UtilityObject").GetComponent<ReadFbx>();
		base.transform.GetComponent<Button>().onClick.AddListener(OnClick);
	}

	public void OnClick()
	{
		if (Time.time - lastClickTime < 0.5f && Path.GetExtension(dir) == ".obj")
		{
			readObjScript.LoadObjFile(dir);
		}
		else if (Time.time - lastClickTime < 0.5f && Path.GetExtension(dir) == ".fbx")
		{
			readFbxScripy.LoadFbxFile(dir);
		}
		lastClickTime = Time.time;
	}
}
