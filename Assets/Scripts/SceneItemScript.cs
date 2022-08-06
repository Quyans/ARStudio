using UnityEngine;
using UnityEngine.UI;

public class SceneItemScript : MonoBehaviour
{
	private PanelBigHierarchyScript panelBigHierarchyScript;

	[HideInInspector]
	public bool drawChildren;

	[HideInInspector]
	public int spritePos;

	[HideInInspector]
	public RectTransform counterButtonRectTrans;

	[HideInInspector]
	public RectTransform counterButtonImageRectTrans;

	[HideInInspector]
	public Image imageCom;

	[HideInInspector]
	public RectTransform counterButtonTextRectTrans;

	[HideInInspector]
	public CanvasGroup canvasGroup;

	private Color color194;

	[HideInInspector]
	public realSceneItemScript realSceneItemScrip;

	[HideInInspector]
	public bool isMarker;
	
	[HideInInspector]
	public int markerID;

	[HideInInspector] 
	public bool isCube;

	[HideInInspector] 
	public int cubeID;

	private void Awake()
	{
		panelBigHierarchyScript = GameObject.Find("PanelBigHierarchy").GetComponent<PanelBigHierarchyScript>();
		drawChildren = false;
		spritePos = 0;
		isMarker = false;
		markerID = 0;
		cubeID = 0;
		if (panelBigHierarchyScript.originalButtonUsed)
		{
			counterButtonRectTrans = Object.Instantiate(GameObject.Find("SceneItem").transform as RectTransform);
			counterButtonRectTrans.SetParent(panelBigHierarchyScript.transform);
		}
		else
		{
			counterButtonRectTrans = GameObject.Find("SceneItem").transform as RectTransform;
		}
		counterButtonRectTrans.offsetMin = panelBigHierarchyScript.sceneItemOffsetMin * panelBigHierarchyScript.nextSceneItemPos;
		counterButtonRectTrans.offsetMax = counterButtonRectTrans.offsetMin + panelBigHierarchyScript.sceneItemOffsetMinMax;
		panelBigHierarchyScript.nextSceneItemPos++;
		counterButtonImageRectTrans = counterButtonRectTrans.GetChild(0) as RectTransform;
		counterButtonImageRectTrans.offsetMin = Vector2.zero;
		counterButtonImageRectTrans.offsetMax = panelBigHierarchyScript.sceneItemSpriteOffsetMinMax;
		imageCom = counterButtonImageRectTrans.GetComponent<Image>();
		counterButtonTextRectTrans = counterButtonRectTrans.GetChild(1) as RectTransform;
		counterButtonTextRectTrans.offsetMin = panelBigHierarchyScript.sceneItemSpriteOffsetMin * 2f;
		counterButtonTextRectTrans.offsetMax = panelBigHierarchyScript.sceneItemTextOffsetMax;
		realSceneItemScrip = counterButtonRectTrans.GetComponent<realSceneItemScript>();
		if (panelBigHierarchyScript.originalButtonUsed)
		{
			DragAbout component = counterButtonTextRectTrans.gameObject.GetComponent<DragAbout>();
			component.realSceneItemScrip = realSceneItemScrip;
			component.belongModel = base.gameObject;
		}
		else
		{
			DragAbout dragAbout = counterButtonTextRectTrans.gameObject.AddComponent<DragAbout>();
			dragAbout.realSceneItemScrip = realSceneItemScrip;
			dragAbout.belongModel = base.gameObject;
			panelBigHierarchyScript.originalButtonUsed = true;
		}
		canvasGroup = counterButtonRectTrans.GetComponent<CanvasGroup>();
		ShowCanvasGroup();
		counterButtonImageRectTrans.GetComponent<Button>().onClick.AddListener(ClickImageButton);
	}

	public void ShowCanvasGroup()
	{
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	public void HideCanvasGroup()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	private void ClickImageButton()
	{
		if (base.transform.childCount != 0)
		{
			if (drawChildren)
			{
				drawChildren = false;
				HideChildrens(base.transform);
			}
			else
			{
				drawChildren = true;
				ShowChildrens(base.transform);
			}
			panelBigHierarchyScript.RedrawSceneItemButton();
		}
	}

	private void HideChildrens(Transform tr)
	{
		int childCount = tr.childCount;
		if (childCount != 0)
		{
			for (int i = 0; i < childCount; i++)
			{
				Transform child = tr.GetChild(i);
				child.GetComponent<SceneItemScript>().HideCanvasGroup();
				HideChildrens(child);
			}
		}
	}

	private void ShowChildrens(Transform tr, bool firstTime = true)
	{
		int childCount = tr.childCount;
		if (childCount == 0)
		{
			return;
		}
		if (firstTime)
		{
			for (int i = 0; i < childCount; i++)
			{
				Transform child = tr.GetChild(i);
				child.GetComponent<SceneItemScript>().ShowCanvasGroup();
				ShowChildrens(child, firstTime: false);
			}
		}
		else if (tr.GetComponent<SceneItemScript>().drawChildren)
		{
			for (int j = 0; j < childCount; j++)
			{
				Transform child2 = tr.GetChild(j);
				child2.GetComponent<SceneItemScript>().ShowCanvasGroup();
				ShowChildrens(child2, firstTime: false);
			}
		}
	}
}
