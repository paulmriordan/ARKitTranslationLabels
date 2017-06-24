using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.iOS
{

	public interface ISelectable
	{
		void Select();
		void Deselect();
		GameObject GetGameObject();
	}

	public interface IEditable
	{
		void StartEditing();
	}

	public class ArLabel : MonoBehaviour, ISelectable, IEditable 
	{
		public static event System.Action OnItemEditBegun;
		public static event System.Action OnItemEditEnded;

		public Image Image;
		public InputField m_InputField;
		public Color SelectedColor = Color.blue;
		public Color UnSelectedColor = Color.white;
		public float SmoothPosTime = 0.2f;
		public float SmoothPosSpeedupRate = 1.0f;
		public float RotSmooth = 1.0f;
		public Vector3 SelectedOffset = new Vector3(0, 1.0f, 1.0f);

		private bool m_selected = false;
		private Vector3 m_posSmoothVel;
		private Quaternion m_animDeriv;
		private GameObject m_gObj;
		private Vector3 m_position;
		private LookAtTarget m_lookAtTarget;
		private float m_selectedTime = float.MaxValue;

		void Awake()
		{
			m_gObj = gameObject;
			m_position = m_gObj.transform.position;
			m_lookAtTarget = GetComponent<LookAtTarget>();
			m_InputField.onEndEdit.AddListener(EditingEnded);
		}

		public GameObject GetGameObject() 
		{
			return m_gObj;
		}

		public void Select()
		{
			Image.color = SelectedColor;
			m_selected = true;
			m_lookAtTarget.enabled = false; // animation takes over
			m_lookAtTarget.SetTargetOffset(new Vector3(SelectedOffset.x, SelectedOffset.y, 0));
			m_selectedTime = Time.time;
		}

		public void Deselect()
		{
			transform.SetParent(LabelManager.Instance.transform); //anim in world
			m_lookAtTarget.enabled = true; // no longer fixed towards camera
			m_lookAtTarget.SetTargetOffset(Vector3.zero);
			Image.color = UnSelectedColor;
			m_selected = false;
			m_selectedTime = float.MaxValue;
		}

		public void StartEditing()
		{
			m_InputField.Select();
			m_InputField.ActivateInputField();
			OnItemEditBegun();
		}

		void EditingEnded(string userInput)
		{
			OnItemEditEnded();
		}

		void Update()
		{
			UpdatePosition();
		}

		void UpdatePosition()
		{
			Vector3 target = transform.position;
			var camTrans = Camera.main.transform;
			Quaternion targetRot = camTrans.rotation * Quaternion.Euler(0, 180.0f, 0);

			if (m_selected)
			{
				var fwd = camTrans.forward.normalized;
				var up = camTrans.up;
				target = camTrans.position + fwd * SelectedOffset.z + up * SelectedOffset.y;
			}
			else
				target = m_position;

			var animTimeFac = 1.0f;
			//get anim speed up factor (no speedup if deselected)
			{
				if (m_selected)
					animTimeFac = 1.0f/(1.0f + SmoothPosSpeedupRate * (Time.time - m_selectedTime));
			}

			if (m_selected)
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(RotSmooth * Time.deltaTime));
//				transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation, camTrans.rotation, ref m_animDeriv, SmoothPosTime * animTimeFac);
			transform.position = Vector3.Lerp(transform.position, target, Mathf.Clamp01(SmoothPosTime * Time.deltaTime));

			//Fix to camera once reached
			{
				if (m_selected && (transform.position - target).sqrMagnitude < 0.0001f)
				{
					transform.SetParent(camTrans);
					transform.localRotation = Quaternion.Euler(0, 180.0f, 0);
				}
			}
		}
	}
}