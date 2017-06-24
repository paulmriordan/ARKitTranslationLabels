using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ContextMenuUIEvents : MonoBehaviour {

		public ContextMenuController m_MenuController;
		public event System.Action EditingBegun;

		public void EditClicked()
		{
			if (InputManager.Instance.m_SelectedObject == null || 
				InputManager.Instance.m_SelectedObject.GetGameObject() == null)
				return;
			
			var inputField = InputManager.Instance.m_SelectedObject.GetGameObject().GetComponentInChildren<InputField>();
			inputField.Select();
			inputField.ActivateInputField();
			EditingBegun();
		}

		public void DeleteClicked()
		{
			LabelManager.Instance.RemoveLabel(InputManager.Instance.m_SelectedObject.GetGameObject());
			InputManager.Instance.m_SelectedObject = null;
		}
	}
}
