using UnityEngine;
using UnityEngine.UI;

public class TextLogScript : MonoBehaviour
{
	public Text logText;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void AddLog(string log)
	{
		string[] array = logText.text.Split('\n');
		int num = array.Length;
		if (num > 10)
		{
			string text = "";
			for (int i = 1; i < num - 1; i++)
			{
				text = text + array[i] + "\n";
			}
			logText.text = text;
		}
		logText.text = logText.text + log + "\n";
	}
}
