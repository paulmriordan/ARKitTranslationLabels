using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ContextMenuUIEvents : MonoBehaviour {

		public ContextMenuController m_MenuController;

		public void EditClicked()
		{
			var editable = InputManager.Instance.m_SelectedObject as IEditable;
			if (editable == null)
				return;
			editable.StartEditing();
		}

		public void DeleteClicked()
		{
			LabelManager.Instance.RemoveLabel(InputManager.Instance.m_SelectedObject.GetGameObject());
			InputManager.Instance.m_SelectedObject = null;
		}

		public void DoneClicked()
		{
		}

		public void CancelClicked()
		{
		}
	}
}
