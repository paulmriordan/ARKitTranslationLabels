using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ContextMenuUIEvents : MonoBehaviour 
	{
		public GameObject[] DebugContainers;

		public event System.Action OnItemMoveRequested = () => {};
		public event System.Action OnItemMoveFinished = () => {};

		public ContextMenuController m_MenuController;

		public void CreateClicked()
		{
			var camTrans = Camera.main.transform;
			var newLabel = LabelManager.Instance.AddLabel(camTrans.position, camTrans.rotation);
			newLabel.StartEditing();
		}

		public void LanguageClicked()
		{
			m_MenuController.ToggleLanguage();
			LabelManager.Instance.TranslateAll(ContextMenuController.GetActiveLanguageCode());
		}

		public void EditClicked()
		{
			var editable = InputManager.Instance.m_SelectedObject as IEditable;
			if (editable == null)
				return;
			editable.StartEditing();
		}

		public void MoveClicked()
		{
			OnItemMoveRequested();
		}

		public void DeleteClicked()
		{
			LabelManager.Instance.RemoveLabel(InputManager.Instance.m_SelectedObject.GetGameObject());
			InputManager.Instance.m_SelectedObject = null;
		}

		public void DoneClicked()
		{
			if (m_MenuController.ActiveState ==  ContextMenuController.E_State.Positioning)
			{
				var positionable = InputManager.Instance.m_SelectedObject as IPositionable;
				if (positionable != null)
					positionable.SaveNewPosition(InputManager.Instance.m_SelectedObject.GetGameObject().transform.position);
				OnItemMoveFinished();
			}

			InputManager.Instance.SelectItem(null);
		}

		public void CancelClicked()
		{
			switch (m_MenuController.ActiveState)
			{
			case ContextMenuController.E_State.Positioning:
				if (InputManager.Instance.m_SelectedObject.IsNew)
				{
					LabelManager.Instance.RemoveLabel(InputManager.Instance.m_SelectedObject.GetGameObject());
					InputManager.Instance.m_SelectedObject = null;
				}
				else
					OnItemMoveFinished();
				break;
			case ContextMenuController.E_State.Selected:
				InputManager.Instance.SelectItem(null);
				break;
			}
		}

		public void DebugClicked()
		{
			foreach (var d in DebugContainers)
				d.SetActive(!d.activeSelf);
		}

	}
}
