using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectEvent : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public UnityEvent onSelect, onDeselect;

	public void OnDeselect(BaseEventData eventData)
	{
		onDeselect.Invoke();
	}

	//Do this when the selectable UI object is selected.
	public void OnSelect(BaseEventData eventData)
	{
		onSelect.Invoke();
	}

	public void SelectButton(GameObject defaultSelectableObject)
	{
		Selectable defaultButton = defaultSelectableObject.GetComponent<Slider>();
		defaultButton.Select();
		defaultButton.OnSelect(null);
	}
	
}
