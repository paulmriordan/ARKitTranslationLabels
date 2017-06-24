using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{
	public class ArLabel : MonoBehaviour, ISelectable 
	{
		public Image Image;
		public Color SelectedColor = Color.blue;
		public Color UnSelectedColor = Color.white;

		private GameObject m_gObj;

		void Start()
		{
			m_gObj = gameObject;
		}

		public GameObject GetGameObject() 
		{
			return m_gObj;
		}

		public void Select()
		{
			Image.color = SelectedColor;
		}

		public void Deselect()
		{
			Image.color = UnSelectedColor;
		}
	}
}