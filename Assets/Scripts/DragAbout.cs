using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAbout : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler
{
	private PanelBigHierarchyScript panelBigHierarchyScript;

	private static ColorBlock colorBlockHigh;

	private static ColorBlock colorBlockHighOnDrag;

	[HideInInspector]
	public realSceneItemScript realSceneItemScrip;

	[HideInInspector]
	public GameObject belongModel;

	private bool nowOver;

	private PropertyScript propertyScript;

	static DragAbout()
	{
		Color white = Color.white;
		white.r = 40f / 51f;
		white.g = 40f / 51f;
		white.b = 40f / 51f;
		colorBlockHigh.normalColor = white;
		white.a = 128f / 255f;
		colorBlockHigh.disabledColor = white;
		white.r = 2f / 3f;
		white.g = 2f / 3f;
		white.b = 2f / 3f;
		white.a = 1f;
		colorBlockHigh.highlightedColor = white;
		white.r = 121f / 255f;
		white.g = 121f / 255f;
		white.b = 121f / 255f;
		colorBlockHigh.pressedColor = white;
		white.r = 8f / 51f;
		white.g = 133f / 255f;
		white.b = 40f / 51f;
		colorBlockHighOnDrag.normalColor = white;
		colorBlockHighOnDrag.highlightedColor = white;
		colorBlockHighOnDrag.pressedColor = white;
		colorBlockHighOnDrag.disabledColor = white;
	}

	private void Awake()
	{
		panelBigHierarchyScript = GameObject.Find("PanelBigHierarchy").GetComponent<PanelBigHierarchyScript>();
		nowOver = false;
		propertyScript = GameObject.Find("PanelProperty").GetComponent<PropertyScript>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		panelBigHierarchyScript.dragBegin = belongModel;
		panelBigHierarchyScript.nowDraging = true;
	}

	public void OnDrag(PointerEventData data)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (panelBigHierarchyScript.dragBegin != null && panelBigHierarchyScript.dragEnd != null)
		{
			if (panelBigHierarchyScript.dragBegin != panelBigHierarchyScript.dragEnd && !panelBigHierarchyScript.dragBegin.GetComponent<SceneItemScript>().isMarker)
			{
				Transform parent = panelBigHierarchyScript.dragBegin.transform.parent;
				if (parent.childCount == 1)
				{
					parent.GetComponent<SceneItemScript>().imageCom.color = Color.clear;
				}
				panelBigHierarchyScript.dragBegin.transform.SetParent(panelBigHierarchyScript.dragEnd.transform);
				SceneItemScript component = panelBigHierarchyScript.dragEnd.GetComponent<SceneItemScript>();
				component.drawChildren = true;
				if (panelBigHierarchyScript.dragEnd.transform.childCount == 1)
				{
					component.imageCom.color = Color.white;
				}
				int spritePos = panelBigHierarchyScript.dragEnd.GetComponent<SceneItemScript>().spritePos;
				SpritePosAdd(panelBigHierarchyScript.dragBegin.transform, spritePos + 1);
				panelBigHierarchyScript.RedrawSceneItemButton();
			}
		}
		else if (panelBigHierarchyScript.dragBegin != null && panelBigHierarchyScript.dragBegin.transform.parent != panelBigHierarchyScript.rootTrans)
		{
			Transform parent2 = panelBigHierarchyScript.dragBegin.transform.parent;
			if (parent2.childCount == 1)
			{
				parent2.GetComponent<SceneItemScript>().imageCom.color = Color.clear;
			}
			panelBigHierarchyScript.dragBegin.transform.SetParent(panelBigHierarchyScript.rootTrans);
			SpritePosAdd(panelBigHierarchyScript.dragBegin.transform, 0);
			panelBigHierarchyScript.RedrawSceneItemButton();
		}
		panelBigHierarchyScript.dragBegin = null;
		panelBigHierarchyScript.dragEnd = null;
		realSceneItemScrip.PointerExit();
		panelBigHierarchyScript.nowDraging = false;
	}

	private void SpritePosAdd(Transform tr, int pos)
	{
		tr.GetComponent<SceneItemScript>().spritePos = pos;
		int childCount = tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			SpritePosAdd(tr.GetChild(i), pos + 1);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (panelBigHierarchyScript.nowDraging)
		{
			realSceneItemScrip.DragPointerEnter();
			return;
		}
		realSceneItemScrip.PointerEnter();
		nowOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (panelBigHierarchyScript.nowDraging)
		{
			if (belongModel != panelBigHierarchyScript.dragBegin)
			{
				realSceneItemScrip.PointerExit();
			}
		}
		else
		{
			realSceneItemScrip.PointerExit();
			nowOver = false;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!panelBigHierarchyScript.nowDraging)
		{
			realSceneItemScrip.PointerDown();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!panelBigHierarchyScript.nowDraging)
		{
			realSceneItemScrip.PointerUp();
			propertyScript.curTrans = belongModel.transform;
			propertyScript.ShowCurData();
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		panelBigHierarchyScript.dragEnd = belongModel;
		realSceneItemScrip.PointerExit();
	}

	private void Update()
	{
		if (nowOver && Input.GetKeyUp(KeyCode.Delete))
		{
			if (belongModel.transform.parent.childCount == 1 && belongModel.transform.parent != panelBigHierarchyScript.rootTrans)
			{
				belongModel.transform.parent.GetComponent<SceneItemScript>().imageCom.color = Color.clear;
			}
			belongModel.transform.SetParent(null);
			ForDelete(belongModel.transform);
			Object.Destroy(belongModel);
			panelBigHierarchyScript.RedrawSceneItemButton();
		}
	}

	private void ForDelete(Transform tr)
	{
		Object.Destroy(tr.GetComponent<SceneItemScript>().counterButtonRectTrans.gameObject);
		int childCount = tr.childCount;
		for (int i = 0; i < childCount; i++)
		{
			ForDelete(tr.GetChild(i));
		}
	}
}
