using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ContextMenuController : MonoBehaviour 
	{
		public enum E_State
		{
			Unselected,
			Selected,
			Editing,
			Positioning,
		}

		public Button m_CreateButton;
		public Button m_LanguageButton;
		public Button m_EditButton;
		public Button m_MoveButton;
		public Button m_DeleteButton;
		public Button m_DoneButton;
		public Button m_CancelButton;
		public string[] Languages = {"English", "Spanish", "French"};

		private int m_currentLanguage = 0;

		public ContextMenuUIEvents m_UIEvents;
		public E_State ActiveState {get; private set;}

		void Start () 
		{
			InputManager.Instance.OnItemSelected += ObjectSelected;
			ArLabel.OnItemEditBegun += () => {ChangeState(E_State.Editing);};
			ArLabel.OnItemEditEnded += () => {ChangeState(E_State.Selected);};
			ArLabel.OnNewItemEditEnded += () => {ChangeState(E_State.Positioning);};
			m_UIEvents.OnItemMoveRequested += () => {ChangeState(E_State.Positioning);};
			m_UIEvents.OnItemMoveFinished += () => {ChangeState(E_State.Selected);};
		}

		public void ToggleLanguage()
		{
			m_currentLanguage = (m_currentLanguage + 1)%Languages.Length;
			var txt = m_LanguageButton.GetComponentInChildren<Text>(true);
			txt.text = Languages[m_currentLanguage];
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
			bool create = state == E_State.Unselected;
			bool language = state == E_State.Unselected;
			bool edit = state == E_State.Selected;
			bool move = state == E_State.Selected;
			bool delete = state == E_State.Selected;
			bool done = state == E_State.Selected || state == E_State.Positioning;
			bool cancel = /*state == E_State.Editing ||*/ state == E_State.Positioning;

			m_CreateButton.gameObject.SetActive(create);
			m_LanguageButton.gameObject.SetActive(language);
			m_EditButton.gameObject.SetActive(edit);
			m_MoveButton.gameObject.SetActive(move);
			m_DeleteButton.gameObject.SetActive(delete);
			m_DoneButton.gameObject.SetActive(done);
			m_CancelButton.gameObject.SetActive(cancel);


			switch(state)
			{
			case E_State.Editing:
				{
				var txt = m_DoneButton.GetComponentInChildren<Text>();
				txt.text = "Confirm Edit";
				break;
				}
			case E_State.Selected:
				{
					var txt = m_DoneButton.GetComponentInChildren<Text>();
					txt.text = "Done";
					break;
				}
			case E_State.Positioning:
				{
				var txt = m_DoneButton.GetComponentInChildren<Text>();
				txt.text = "Place here";
				break;
				}
			}
			ActiveState = state;
		}
	}
}
