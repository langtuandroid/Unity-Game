using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannel/VoidEventChannel")]

public class VoidEventChannel : DescriptionBaseSO
{
	public UnityAction OnEventRaised;
	public void RaiseEvent()
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke();
	}
}
