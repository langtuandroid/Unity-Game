using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannel/FloatEventChannel")]

public class FloatEventChannel : DescriptionBaseSO
{
	public UnityAction<float> OnEventRaised;
	public void RaiseEvent(float arg)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(arg);
	}
}
