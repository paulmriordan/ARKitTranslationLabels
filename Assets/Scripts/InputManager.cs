using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.iOS
{
	public class InputManager : MonoSingleton<InputManager> 
	{
		public event System.Action<ISelectable> OnItemSelected;

		public ISelectable m_SelectedObject;
		public bool RemoveInWorldUIOnRelease = false;
		public bool WorldUIInFrontOfCam = true;
		public Vector3 WorldUIOffsetFromCam = new Vector3(0,0,1.0f);
		public bool TapOnTapOffWorldUI = false;

		private HashSet<int> m_fingerDownCanvas = new HashSet<int>();
		private IHoldActivatedUI m_holdActivatedUI;
		private const int DUMMY_TOUCH_ID = 1;

		void Start()
		{
//			ArLabel.OnItemEditEnded += () => { SelectItem(null);};
		}

		public void RegisterHoldActivatedUI(IHoldActivatedUI holdUI)
		{
			m_holdActivatedUI = holdUI;
		}

		public void SelectItem(ISelectable item)
		{
			if (m_SelectedObject == item)
				return;

			if (m_SelectedObject != null)
				m_SelectedObject.Deselect();
			
			if (item != null)
				item.Select();

			m_SelectedObject = item;
			OnItemSelected(m_SelectedObject);
		}

		bool IsConsumedByCanvas()
		{
#if UNITY_EDITOR
			return EventSystem.current.IsPointerOverGameObject();
#else

			return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
		}

		void Update () 
		{
			if (Input.GetMouseButtonDown(0))
			{
				FingerDown();
			}
			if (Input.GetMouseButton(0))
			{
				FingerHeld();
			}
			if (Input.GetMouseButtonUp(0))
			{
				FingerUp();
			}
		}

		void FingerDown()
		{
			if (IsConsumedByCanvas())
			{
				m_fingerDownCanvas.Add(DUMMY_TOUCH_ID);
				return;
			}

			if (!WorldUIInFrontOfCam)
				ShowWorldUIAtRaycastHitPoint();
		}

		void ShowWorldUIInFrontOfCamera()
		{
			var camTrans = Camera.main.transform;
			var fwd = camTrans.forward.normalized;
			var up = camTrans.up;
			var pos = camTrans.position + fwd * WorldUIOffsetFromCam.z + up * WorldUIOffsetFromCam.y;
			var rot = Quaternion.LookRotation(camTrans.position - pos);
			if (m_holdActivatedUI != null)
				m_holdActivatedUI.ShowRequest(pos, rot);
		}

		void ShowWorldUIAtRaycastHitPoint()
		{
			if (m_holdActivatedUI == null)
				return;
			
			#if UNITY_EDITOR
			Plane p = new Plane(new Vector3(0,0,1.0f), new Vector3(0,0,-3.21f));
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float enter;
			if (p.Raycast(ray, out enter))
			{
				m_holdActivatedUI.ShowRequest(ray.GetPoint(enter), Quaternion.identity);
			}
			#else
			var viewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			ARHitTestResult hitResult = default(ARHitTestResult);
			if (ARHitTestUtils.GetBestARHitResult(viewportPoint, ref hitResult))
			{
			m_holdActivatedUI.ShowRequest(UnityARMatrixOps.GetPosition (hitResult.worldTransform), 
			UnityARMatrixOps.GetRotation (hitResult.worldTransform));

			}
			#endif
		}

		void FingerHeld()
		{
			if (RemoveInWorldUIOnRelease && m_holdActivatedUI != null && m_holdActivatedUI.IsActive)
			{
				SelectItem(null);
			}	
		}

		void FingerUp()
		{
			if (RemoveInWorldUIOnRelease && m_holdActivatedUI != null)
				m_holdActivatedUI.Remove();

			if (IsConsumedByCanvas() || m_fingerDownCanvas.Contains(DUMMY_TOUCH_ID))
			{
				m_fingerDownCanvas.Remove(DUMMY_TOUCH_ID);
				return;
			}
			
			// Try select existing label
			RaycastHit hitInfo;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000.0f))
			{
				var selectable = hitInfo.collider.gameObject.GetComponent<ISelectable>();
				if (m_SelectedObject == selectable && !m_SelectedObject.IsNew)
					SelectItem(null);
				else
					SelectItem(selectable);
			}
			else
			{
				if (m_SelectedObject != null && m_SelectedObject.IsNew)
					LabelManager.Instance.RemoveLabel(m_SelectedObject.GetGameObject());
				SelectItem(null);
				if (WorldUIInFrontOfCam)
					ShowWorldUIInFrontOfCamera();
			}
		}
	}
}
