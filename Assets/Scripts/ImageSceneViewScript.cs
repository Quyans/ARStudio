using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageSceneViewScript : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private RectTransform thisRectTransform;

	private Transform camera_UITransform;

	private Transform camera_init_trans;

	private float depth;

	private Vector2 thetaPhi;

	private Vector2 rightMouseButtonPos;

	private Vector2 degreesPerPixel;

	private float[] timePressed;

	private float displacementPerSecondPerDepth;

	private Vector3 oldCameraPos;

	private Vector3[] finalDisplacements;

	private bool nowOver;

	private void Awake()
	{
		thisRectTransform = base.transform as RectTransform;
		camera_init_trans = GameObject.Find("Camera_UI").transform;
		camera_UITransform = GameObject.Find("Camera_UI").transform;
		depth = 20f;
		thetaPhi = new Vector2(90f, 90f);
		rightMouseButtonPos = Vector2.zero;
		degreesPerPixel = new Vector2(0.8f, 0.8f);
		timePressed = new float[4]
		{
			-1f,
			-1f,
			-1f,
			-1f
		};
		displacementPerSecondPerDepth = 5f;
		oldCameraPos = camera_UITransform.position;
		finalDisplacements = new Vector3[4];
		nowOver = false;
	}

	private void Update()
	{
		if (!nowOver)
		{
			return;
		}
		if (Input.GetMouseButtonDown(1))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRectTransform, Input.mousePosition, null, out rightMouseButtonPos);
		}
		if (Input.GetMouseButton(1))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRectTransform, Input.mousePosition, null, out var localPoint);
			localPoint -= rightMouseButtonPos;
			Vector2 vector = new Vector2(localPoint.y * degreesPerPixel.x, localPoint.x * degreesPerPixel.y);
			vector += thetaPhi;
			if (vector.x < 0f)
			{
				vector.x = 0f;
			}
			if (vector.x > 180f)
			{
				vector.x = 180f;
			}
			vector *= (float)Math.PI / 180f;
			Vector3 b = default(Vector3);
			b.x = depth * Mathf.Sin(vector.x) * Mathf.Cos(vector.y);
			b.y = depth * Mathf.Cos(vector.x);
			b.z = (0f - depth) * Mathf.Sin(vector.x) * Mathf.Sin(vector.y);
			Vector3 vector2 = camera_UITransform.TransformPoint(0f, 0f, depth);
			camera_UITransform.position = vector2 + b;
			camera_UITransform.LookAt(vector2);
		}
		if (Input.GetMouseButtonUp(1))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRectTransform, Input.mousePosition, null, out var localPoint2);
			localPoint2 -= rightMouseButtonPos;
			Vector2 vector3 = new Vector2(localPoint2.y * degreesPerPixel.x, localPoint2.x * degreesPerPixel.y);
			thetaPhi += vector3;
			if (thetaPhi.x < 0f)
			{
				thetaPhi.x = 0f;
			}
			if (thetaPhi.x > 180f)
			{
				thetaPhi.x = 180f;
			}
		}
		if (timePressed[0] >= 0f)
		{
			finalDisplacements[0] = (Time.time - timePressed[0]) * displacementPerSecondPerDepth * depth * camera_UITransform.forward;
		}
		if (timePressed[1] >= 0f)
		{
			finalDisplacements[1] = (Time.time - timePressed[1]) * displacementPerSecondPerDepth * depth * camera_UITransform.forward;
		}
		if (timePressed[2] >= 0f)
		{
			finalDisplacements[2] = (Time.time - timePressed[2]) * displacementPerSecondPerDepth * depth * camera_UITransform.right;
		}
		if (timePressed[3] >= 0f)
		{
			finalDisplacements[3] = (Time.time - timePressed[3]) * displacementPerSecondPerDepth * depth * camera_UITransform.right;
		}
		camera_UITransform.position = oldCameraPos + finalDisplacements[0] - finalDisplacements[1] - finalDisplacements[2] + finalDisplacements[3];
		if (Input.GetKeyDown("w"))
		{
			timePressed[0] = Time.time;
			if (OnlyOneNonNegative())
			{
				oldCameraPos = camera_UITransform.position;
			}
		}
		if (Input.GetKeyUp("w"))
		{
			oldCameraPos += finalDisplacements[0];
			finalDisplacements[0] = Vector3.zero;
			timePressed[0] = -1f;
		}
		if (Input.GetKeyDown("s"))
		{
			timePressed[1] = Time.time;
			if (OnlyOneNonNegative())
			{
				oldCameraPos = camera_UITransform.position;
			}
		}
		if (Input.GetKeyUp("s"))
		{
			oldCameraPos -= finalDisplacements[1];
			finalDisplacements[1] = Vector3.zero;
			timePressed[1] = -1f;
		}
		if (Input.GetKeyDown("a"))
		{
			timePressed[2] = Time.time;
			if (OnlyOneNonNegative())
			{
				oldCameraPos = camera_UITransform.position;
			}
		}
		if (Input.GetKeyUp("a"))
		{
			oldCameraPos -= finalDisplacements[2];
			finalDisplacements[2] = Vector3.zero;
			timePressed[2] = -1f;
		}
		if (Input.GetKeyDown("d"))
		{
			timePressed[3] = Time.time;
			if (OnlyOneNonNegative())
			{
				oldCameraPos = camera_UITransform.position;
			}
		}
		if (Input.GetKeyUp("d"))
		{
			oldCameraPos += finalDisplacements[3];
			finalDisplacements[3] = Vector3.zero;
			timePressed[3] = -1f;
		}
		if (Input.GetKeyDown("r"))
		{
			oldCameraPos = camera_init_trans.position;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			float num = depth;
			depth /= 1.1f;
			camera_UITransform.position = camera_UITransform.TransformPoint(0f, 0f, num - depth);
			oldCameraPos = camera_UITransform.position;
		}
		else if (axis < 0f)
		{
			float num2 = depth;
			depth *= 1.1f;
			camera_UITransform.position = camera_UITransform.TransformPoint(0f, 0f, num2 - depth);
			oldCameraPos = camera_UITransform.position;
		}
	}

	private bool OnlyOneNonNegative()
	{
		int num = 0;
		if (timePressed[0] >= 0f)
		{
			num++;
		}
		if (timePressed[1] >= 0f)
		{
			num++;
		}
		if (timePressed[2] >= 0f)
		{
			num++;
		}
		if (timePressed[3] >= 0f)
		{
			num++;
		}
		if (num == 1)
		{
			return true;
		}
		return false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		nowOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		nowOver = false;
	}
}
