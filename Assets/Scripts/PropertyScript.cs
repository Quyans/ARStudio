using UnityEngine;
using UnityEngine.UI;

public class PropertyScript : MonoBehaviour
{
	private bool nowWorld;

	[HideInInspector]
	public Transform curTrans;

	private InputField InputFieldX1;

	private InputField InputFieldX2;

	private InputField InputFieldX3;

	private InputField InputFieldY1;

	private InputField InputFieldY2;

	private InputField InputFieldY3;

	private InputField InputFieldZ1;

	private InputField InputFieldZ2;

	private InputField InputFieldZ3;

	private Text worldText;

	private void Awake()
	{
		nowWorld = false;
		curTrans = null;
		InputFieldX1 = base.transform.GetChild(1).GetComponent<InputField>();
		InputFieldX2 = base.transform.GetChild(2).GetComponent<InputField>();
		InputFieldX3 = base.transform.GetChild(3).GetComponent<InputField>();
		InputFieldY1 = base.transform.GetChild(4).GetComponent<InputField>();
		InputFieldY2 = base.transform.GetChild(5).GetComponent<InputField>();
		InputFieldY3 = base.transform.GetChild(6).GetComponent<InputField>();
		InputFieldZ1 = base.transform.GetChild(7).GetComponent<InputField>();
		InputFieldZ2 = base.transform.GetChild(8).GetComponent<InputField>();
		InputFieldZ3 = base.transform.GetChild(9).GetComponent<InputField>();
		worldText = base.transform.GetChild(0).GetChild(0).GetComponent<Text>();
	}

	public void ShowCurData()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				InputFieldX1.text = curTrans.position.x.ToString();
				InputFieldY1.text = curTrans.position.y.ToString();
				InputFieldZ1.text = curTrans.position.z.ToString();
				InputFieldX2.text = curTrans.eulerAngles.x.ToString();
				InputFieldY2.text = curTrans.eulerAngles.y.ToString();
				InputFieldZ2.text = curTrans.eulerAngles.z.ToString();
				InputFieldX3.text = curTrans.lossyScale.x.ToString();
				InputFieldY3.text = curTrans.lossyScale.y.ToString();
				InputFieldZ3.text = curTrans.lossyScale.z.ToString();
			}
			else
			{
				InputFieldX1.text = curTrans.localPosition.x.ToString();
				InputFieldY1.text = curTrans.localPosition.y.ToString();
				InputFieldZ1.text = curTrans.localPosition.z.ToString();
				InputFieldX2.text = curTrans.localEulerAngles.x.ToString();
				InputFieldY2.text = curTrans.localEulerAngles.y.ToString();
				InputFieldZ2.text = curTrans.localEulerAngles.z.ToString();
				InputFieldX3.text = curTrans.localScale.x.ToString();
				InputFieldY3.text = curTrans.localScale.y.ToString();
				InputFieldZ3.text = curTrans.localScale.z.ToString();
			}
		}
		else
		{
			InputFieldX1.text = string.Empty;
			InputFieldY1.text = string.Empty;
			InputFieldZ1.text = string.Empty;
			InputFieldX2.text = string.Empty;
			InputFieldY2.text = string.Empty;
			InputFieldZ2.text = string.Empty;
			InputFieldX3.text = string.Empty;
			InputFieldY3.text = string.Empty;
			InputFieldZ3.text = string.Empty;
		}
	}

	public void OnButtonClick()
	{
		nowWorld = !nowWorld;
		if (nowWorld)
		{
			worldText.text = "World";
		}
		else
		{
			worldText.text = "Local";
		}
		ShowCurData();
	}

	public void InputFieldX1End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 position = curTrans.position;
				position.x = float.Parse(InputFieldX1.text);
				curTrans.position = position;
			}
			else
			{
				Vector3 localPosition = curTrans.localPosition;
				localPosition.x = float.Parse(InputFieldX1.text);
				curTrans.localPosition = localPosition;
			}
		}
		else
		{
			InputFieldX1.text = string.Empty;
		}
	}

	public void InputFieldY1End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 position = curTrans.position;
				position.y = float.Parse(InputFieldY1.text);
				curTrans.position = position;
			}
			else
			{
				Vector3 localPosition = curTrans.localPosition;
				localPosition.y = float.Parse(InputFieldY1.text);
				curTrans.localPosition = localPosition;
			}
		}
		else
		{
			InputFieldY1.text = string.Empty;
		}
	}

	public void InputFieldZ1End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 position = curTrans.position;
				position.z = float.Parse(InputFieldZ1.text);
				curTrans.position = position;
			}
			else
			{
				Vector3 localPosition = curTrans.localPosition;
				localPosition.z = float.Parse(InputFieldZ1.text);
				curTrans.localPosition = localPosition;
			}
		}
		else
		{
			InputFieldZ1.text = string.Empty;
		}
	}

	public void InputFieldX2End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 eulerAngles = curTrans.eulerAngles;
				eulerAngles.x = float.Parse(InputFieldX2.text);
				curTrans.eulerAngles = eulerAngles;
			}
			else
			{
				Vector3 localEulerAngles = curTrans.localEulerAngles;
				localEulerAngles.x = float.Parse(InputFieldX2.text);
				curTrans.localEulerAngles = localEulerAngles;
			}
		}
		else
		{
			InputFieldX2.text = string.Empty;
		}
	}

	public void InputFieldY2End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 eulerAngles = curTrans.eulerAngles;
				eulerAngles.y = float.Parse(InputFieldY2.text);
				curTrans.eulerAngles = eulerAngles;
			}
			else
			{
				Vector3 localEulerAngles = curTrans.localEulerAngles;
				localEulerAngles.y = float.Parse(InputFieldY2.text);
				curTrans.localEulerAngles = localEulerAngles;
			}
		}
		else
		{
			InputFieldY2.text = string.Empty;
		}
	}

	public void InputFieldZ2End()
	{
		if (curTrans != null)
		{
			if (nowWorld)
			{
				Vector3 eulerAngles = curTrans.eulerAngles;
				eulerAngles.z = float.Parse(InputFieldZ2.text);
				curTrans.eulerAngles = eulerAngles;
			}
			else
			{
				Vector3 localEulerAngles = curTrans.localEulerAngles;
				localEulerAngles.z = float.Parse(InputFieldZ2.text);
				curTrans.localEulerAngles = localEulerAngles;
			}
		}
		else
		{
			InputFieldZ2.text = string.Empty;
		}
	}

	public void InputFieldX3End()
	{
		if (curTrans != null)
		{
			if (!nowWorld)
			{
				Vector3 localScale = curTrans.localScale;
				localScale.x = float.Parse(InputFieldX3.text);
				curTrans.localScale = localScale;
			}
		}
		else
		{
			InputFieldX3.text = string.Empty;
		}
	}

	public void InputFieldY3End()
	{
		if (curTrans != null)
		{
			if (!nowWorld)
			{
				Vector3 localScale = curTrans.localScale;
				localScale.y = float.Parse(InputFieldY3.text);
				curTrans.localScale = localScale;
			}
		}
		else
		{
			InputFieldY3.text = string.Empty;
		}
	}

	public void InputFieldZ3End()
	{
		if (curTrans != null)
		{
			if (!nowWorld)
			{
				Vector3 localScale = curTrans.localScale;
				localScale.z = float.Parse(InputFieldZ3.text);
				curTrans.localScale = localScale;
			}
		}
		else
		{
			InputFieldZ3.text = string.Empty;
		}
	}
}
