using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public class LabelManager : MonoSingleton<LabelManager> {

		public GameObject LabelPrefab;

		public ArLabel AddLabel(Vector3 pos, Quaternion rot)
		{
			var item = GameObject.Instantiate(LabelPrefab, pos, rot, transform).GetComponent<ArLabel>();
			InputManager.Instance.SelectItem(item.GetComponent<ISelectable>());
			return item;
		}

		public void RemoveLabel(GameObject toRemove)
		{
			Destroy(toRemove);
			InputManager.Instance.SelectItem(null);
		}
	}
}