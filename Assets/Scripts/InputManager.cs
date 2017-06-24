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

		private HashSet<int> m_fingerDownCanvas = new HashSet<int>();
		private IHoldActivatedUI m_holdActivatedUI;
		private const int DUMMY_TOUCH_ID = 1;

		void Start()
		{
			ArLabel.OnItemEditEnded += () => { SelectItem(null);};
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

		// Update is called once per frame
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

#if UNITY_EDITOR
			Plane p = new Plane(new Vector3(0,0,1.0f), new Vector3(0,0,-3.21f));
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float enter;
			if (p.Raycast(ray, out enter))
			{
				m_holdActivatedUI.HoldBegun(ray.GetPoint(enter), Quaternion.identity);
			}
#else
			var viewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			ARHitTestResult hitResult = default(ARHitTestResult);
			if (ARHitTestUtils.GetBestARHitResult(viewportPoint, ref hitResult))
			{
				m_holdActivatedUI.HoldBegun(UnityARMatrixOps.GetPosition (hitResult.worldTransform), 
										UnityARMatrixOps.GetRotation (hitResult.worldTransform));

			}
#endif
		}

		void FingerHeld()
		{
			if (m_holdActivatedUI.IsActive)
			{
				SelectItem(null);
			}	
		}

		void FingerUp()
		{
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
				SelectItem(hitInfo.collider.gameObject.GetComponent<ISelectable>());
			}
			else
			{
				SelectItem(null);
			}
		}
	}
}
