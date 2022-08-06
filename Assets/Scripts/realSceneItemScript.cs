using UnityEngine;
using UnityEngine.UI;

public class realSceneItemScript : MonoBehaviour
{
	private Image thisImage;

	private static Color normalColor;

	private static Color highlightedColor;

	private static Color pressedColor;

	private static Color dragHighlightedColor;

	static realSceneItemScript()
	{
		normalColor.r = 40f / 51f;
		normalColor.g = 40f / 51f;
		normalColor.b = 40f / 51f;
		normalColor.a = 0f;
		highlightedColor.r = 2f / 3f;
		highlightedColor.g = 2f / 3f;
		highlightedColor.b = 2f / 3f;
		highlightedColor.a = 1f;
		pressedColor.r = 121f / 255f;
		pressedColor.g = 121f / 255f;
		pressedColor.b = 121f / 255f;
		pressedColor.a = 1f;
		dragHighlightedColor.r = 8f / 51f;
		dragHighlightedColor.g = 133f / 255f;
		dragHighlightedColor.b = 1f;
		dragHighlightedColor.a = 1f;
	}

	private void Awake()
	{
		thisImage = base.gameObject.GetComponent<Image>();
		thisImage.color = normalColor;
	}

	public void PointerEnter()
	{
		thisImage.color = highlightedColor;
	}

	public void PointerExit()
	{
		thisImage.color = normalColor;
	}

	public void PointerDown()
	{
		thisImage.color = pressedColor;
	}

	public void PointerUp()
	{
		thisImage.color = highlightedColor;
	}

	public void DragPointerEnter()
	{
		thisImage.color = dragHighlightedColor;
	}
}
