
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public class HoldContextUIEvents : MonoBehaviour {

		public void CreateNewLabel()
		{
			Debug.Log("Create new label at" + transform.position);
			var newLabel = LabelManager.Instance.AddLabel(transform.position, transform.rotation);
			newLabel.StartEditing();
		}

		public void MoveLabel()
		{
		}
	}
}
