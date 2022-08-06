using UnityEngine;
using UnityEngine.UI;

public class AssetButtonScript : MonoBehaviour
{
	[HideInInspector]
	public string dir;

	private PanelBigAssetScript panelBigAssetScript;

	private float lastClickTime;

	private void Awake()
	{
		panelBigAssetScript = GameObject.Find("PanelBigAsset").GetComponent<PanelBigAssetScript>();
		lastClickTime = Time.time;
		base.transform.GetComponent<Button>().onClick.AddListener(OnClick);
	}

	public void OnClick()
	{
		if (Time.time - lastClickTime < 0.5f)
		{
			panelBigAssetScript.preDirs.Add(panelBigAssetScript.curDir);
			panelBigAssetScript.curDir = dir;
			panelBigAssetScript.DrawAllButtons();
		}
		lastClickTime = Time.time;
	}
}
