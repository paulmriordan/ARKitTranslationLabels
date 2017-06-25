using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButtonRelease : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent onReleasedOver;
	bool m_entered = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		m_entered = true;
		Debug.Log("Down");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Input.GetMouseButton(0))
			m_entered = true;
		Debug.Log("Enter");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (m_entered && !Input.GetMouseButton(0))
			onReleasedOver.Invoke();
		m_entered = false;
		Debug.Log("Exit");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (m_entered && !Input.GetMouseButton(0))
			onReleasedOver.Invoke();
		m_entered = false;
		Debug.Log("Up");
	}

	void Update()
	{
		if (!Input.GetMouseButton(0) && m_entered)
		{
			Debug.Log("Released over");
			onReleasedOver.Invoke();
			m_entered = false;
		}
	}
}
