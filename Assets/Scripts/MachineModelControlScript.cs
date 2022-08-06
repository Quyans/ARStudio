using UnityEngine;

namespace rc_hololens
{
	public class MachineModelControlScript : MonoBehaviour
	{
		[HideInInspector]
		public Vector3 sliderOldDisplacements;

		private void Awake()
		{
			sliderOldDisplacements = Vector3.zero;
		}

		private void Start()
		{
			Vector3 vector = new Vector3(10000f, 10000f, 10000f);
			Vector3 vector2 = new Vector3(-10000f, -10000f, -10000f);
			Component[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
			componentsInChildren = componentsInChildren;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				MeshRenderer meshRenderer = (MeshRenderer)componentsInChildren[i];
				vector = Vector3.Min(vector, meshRenderer.bounds.min);
				vector2 = Vector3.Max(vector2, meshRenderer.bounds.max);
			}
			vector = base.gameObject.transform.InverseTransformPoint(vector);
			vector2 = base.gameObject.transform.InverseTransformPoint(vector2);
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			component.center = (vector + vector2) / 2f;
			component.size = (vector2 - vector) / 2f;
		}

		private void Update()
		{
		}
	}
}
