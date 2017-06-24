
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public class HoldContextUIEvents : MonoBehaviour {

		public void CreateNewLabel()
		{
			LabelManager.Instance.AddLabel(transform.position, transform.rotation);
		}

		public void MoveLabel()
		{
		}


	}
}
