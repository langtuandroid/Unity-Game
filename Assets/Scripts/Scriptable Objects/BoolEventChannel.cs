using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannel/BoolEventChannel")]

public class BoolEventChannel : DescriptionBaseSO
{
	public UnityAction<bool> OnEventRaised;
	public void RaiseEvent(bool arg)
	{
		if (OnEventRaised != null) { OnEventRaised.Invoke(arg); }
	}
}
