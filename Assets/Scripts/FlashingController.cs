using UnityEngine;

public class FlashingController : MonoBehaviour
{
	public Color flashingStartColor = Color.blue;
	public Color flashingEndColor = Color.cyan;
	public float flashingFrequency = 2f;

	protected HighlightableObject Ho;

	void Start()
	{
		Ho = gameObject.AddComponent<HighlightableObject>();
		Ho.FlashingOn(flashingStartColor, flashingEndColor, flashingFrequency);
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.E))
			Ho.FlashingOff();
		if(Input.GetKeyDown(KeyCode.D))
			Ho.FlashingOn();
	}
}
