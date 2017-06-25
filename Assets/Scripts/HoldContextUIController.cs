using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public interface IHoldActivatedUI
	{
		void ShowRequest(Vector3 pos, Quaternion rot);
		void Remove();
		bool IsActive {get;}
	}

	public class HoldContextUIController : MonoBehaviour, IHoldActivatedUI 
	{
		public float ShowAfterHoldSecs = 0.5f;

		private float m_heldStartTime = float.MaxValue;
		private GameObject m_UIObject;

		public bool IsActive {get {return m_UIObject.activeSelf;}}

		void Start()
		{
			m_UIObject = GetComponentInChildren<Canvas>(true).gameObject;
			InputManager.Instance.RegisterHoldActivatedUI(this);
		}

		public void ShowRequest(Vector3 pos, Quaternion rot)
		{
			m_heldStartTime = Time.time;
			transform.position = pos;
			transform.rotation = rot;
		}

		public void Remove()
		{
			m_heldStartTime = float.MaxValue;
			m_UIObject.SetActive(false);
		}

		void Show()
		{
			m_UIObject.SetActive(true);
		}

		void Update()
		{
			if (Time.time > m_heldStartTime + ShowAfterHoldSecs && !IsActive)
			{
				Show();
			}
		}
	}
}
