using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PanelBigAssetScript : MonoBehaviour
{
	private RectTransform thisRectTranform;

	private RectTransform backButton;

	private Vector2 dirVec;

	private Vector2 fileVec1;

	private Vector2 fileVec2;

	private Vector2 text1Vec;

	private Vector2 text2VecOffsetmin;

	private Vector2 text2VecOffsetmax;

	private Vector2 vec01;

	private Vector2 text2VecOffsetminForDir;

	private Vector2 text2VecOffsetmaxForDir;

	private Vector3 forBackButtonScale;

	[HideInInspector]
	public string curDir;

	[HideInInspector]
	public List<string> preDirs;

	private Button.ButtonClickedEvent backClickedEvent;

	private void Awake()
	{
		thisRectTranform = base.transform as RectTransform;
		backButton = GameObject.Find("BackButton").transform as RectTransform;
		dirVec.x = 64f;
		dirVec.y = 56f;
		fileVec1.x = 8f;
		fileVec1.y = 0f;
		fileVec2.x = 48f;
		fileVec2.y = 64f;
		text1Vec.x = 24f;
		text1Vec.y = 32f;
		text2VecOffsetmin.x = -12f;
		text2VecOffsetmin.y = -15f;
		text2VecOffsetmax.x = 64f;
		text2VecOffsetmax.y = 1f;
		vec01.x = 0f;
		vec01.y = 1f;
		text2VecOffsetminForDir.x = -6f;
		text2VecOffsetminForDir.y = -15f;
		text2VecOffsetmaxForDir.x = 70f;
		text2VecOffsetmaxForDir.y = 1f;
		forBackButtonScale = Vector3.one;
		forBackButtonScale.x = -1f;
		curDir = "D:/";
		preDirs.Add(curDir);
		backClickedEvent = backButton.GetComponent<Button>().onClick;
		backClickedEvent.AddListener(BackButtonClicked);
		GameObject.Find("EventSystem").GetComponent<TextLogScript>().AddLog("LOG: Root loading path: " + curDir);
		ShowAllSubs(curDir);
	}

	private Vector2 IthButtonOffsetmin(int i)
	{
		Vector2 result = default(Vector2);
		if (i % 2 == 1)
		{
			result.x = 10f;
		}
		else
		{
			result.x = 90f;
		}
		i = (i + 1) / 2;
		result.y = 2 - 80 * i;
		return result;
	}

	private void ShowAllSubs(string dir)
	{
		if (Directory.Exists(dir))
		{
			backButton.localScale = Vector3.one;
			backClickedEvent.RemoveListener(BackButtonClicked);
			DirectoryInfo directoryInfo = new DirectoryInfo(dir);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			FileInfo[] files = directoryInfo.GetFiles();
			int num = (directories.Length + files.Length + 2) / 2;
			num = Mathf.Max(506, 80 * num + 12);
			thisRectTranform.offsetMin = new Vector2(0f, -num);
			num = directories.Length;
			for (int i = 1; i <= num; i++)
			{
				RectTransform rectTransform = Object.Instantiate(backButton);
				rectTransform.SetParent(base.transform, worldPositionStays: false);
				rectTransform.anchorMin = vec01;
				rectTransform.anchorMax = vec01;
				rectTransform.offsetMin = IthButtonOffsetmin(i + 1);
				rectTransform.offsetMax = rectTransform.offsetMin + dirVec;
				rectTransform.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Sprites/FolderIcon");
				RectTransform obj = rectTransform.GetChild(1) as RectTransform;
				obj.anchorMin = Vector2.zero;
				obj.anchorMax = obj.anchorMin;
				obj.offsetMin = text2VecOffsetminForDir;
				obj.offsetMax = text2VecOffsetmaxForDir;
				Text component = obj.GetComponent<Text>();
				component.fontSize = 14;
				component.text = directories[i - 1].Name;
				rectTransform.gameObject.AddComponent<AssetButtonScript>().dir = directories[i - 1].FullName;
			}
			num = files.Length;
			for (int j = 1; j <= num; j++)
			{
				RectTransform rectTransform2 = Object.Instantiate(backButton);
				rectTransform2.SetParent(base.transform, worldPositionStays: false);
				rectTransform2.anchorMin = vec01;
				rectTransform2.anchorMax = vec01;
				rectTransform2.offsetMin = IthButtonOffsetmin(directories.Length + j + 1) + fileVec1;
				rectTransform2.offsetMax = rectTransform2.offsetMin + fileVec2;
				rectTransform2.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Sprites/DefaultFileIcon");
				string name = files[j - 1].Name;
				RectTransform obj2 = rectTransform2.GetChild(0) as RectTransform;
				obj2.anchorMin = Vector2.one / 2f;
				obj2.anchorMax = obj2.anchorMin;
				obj2.offsetMin = -text1Vec;
				obj2.offsetMax = text1Vec;
				Text component2 = obj2.GetComponent<Text>();
				component2.fontSize = 18;
				component2.text = Path.GetExtension(name);
				RectTransform obj3 = rectTransform2.GetChild(1) as RectTransform;
				obj3.anchorMin = Vector2.zero;
				obj3.anchorMax = obj3.anchorMin;
				obj3.offsetMin = text2VecOffsetmin;
				obj3.offsetMax = text2VecOffsetmax;
				Text component3 = obj3.GetComponent<Text>();
				component3.fontSize = 14;
				component3.text = Path.GetFileNameWithoutExtension(name);
				rectTransform2.gameObject.AddComponent<AssetFileScript>().dir = files[j - 1].FullName;
			}
			backClickedEvent.AddListener(BackButtonClicked);
			backButton.localScale = forBackButtonScale;
		}
	}

	private void DestroyButtons()
	{
		int childCount = base.transform.childCount;
		for (int i = 1; i < childCount; i++)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
	}

	public void DrawAllButtons()
	{
		DestroyButtons();
		ShowAllSubs(curDir);
	}

	public void BackButtonClicked()
	{
		int num = preDirs.Count - 1;
		if (num > 0)
		{
			curDir = preDirs[num];
			preDirs.RemoveAt(num);
			DrawAllButtons();
		}
	}
}
