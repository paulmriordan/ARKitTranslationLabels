using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ContextMenuController : MonoBehaviour 
	{
		public Button m_DeleteButton;
		public Button m_EditButton;
		public ContextMenuUIEvents m_UIEvents;

		private enum E_State
		{
			Unselected,
			Selected,
			Editing,
		}

		void Start () 
		{
			InputManager.Instance.OnItemSelected += ObjectSelected;
			m_UIEvents.EditingBegun += () => { ChangeState(E_State.Editing);};
		}

		void ObjectSelected(ISelectable obj)
		{
			if (obj != null)
				ChangeState(E_State.Selected);
			else
				ChangeState(E_State.Unselected);
		}

		void ChangeState(E_State state)
		{
			switch(state)
			{
			case E_State.Selected:
				m_DeleteButton.gameObject.SetActive(true);
				m_EditButton.gameObject.SetActive(true);
				break;
			case E_State.Unselected:
				m_DeleteButton.gameObject.SetActive(false);
				m_EditButton.gameObject.SetActive(false);
				break;
			case E_State.Editing:
				m_DeleteButton.gameObject.SetActive(true);
				m_EditButton.gameObject.SetActive(false);
				break;
			}
		}
	}
}
