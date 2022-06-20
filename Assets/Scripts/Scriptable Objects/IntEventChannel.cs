using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannel/IntEventChannel")]

public class IntEventChannel : DescriptionBaseSO
{
	public UnityAction<int> OnEventRaised;
	public void RaiseEvent(int arg)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(arg);
	}
}
