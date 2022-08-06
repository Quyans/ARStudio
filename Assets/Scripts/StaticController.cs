using System;
using UnityEngine;

public class StaticController : MonoBehaviour
{
    public Color flashingStartColor = Color.blue;
    protected HighlightableObject Ho;
	
    void Start()
    {
        Ho = gameObject.AddComponent<HighlightableObject>();
        Ho.ConstantOn(flashingStartColor);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
            Ho.ConstantOff();
        if(Input.GetKeyDown(KeyCode.D))
            Ho.ConstantOn();
    }
}
