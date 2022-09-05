using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannel/String Channel")]
public class StringEventChannel : ScriptableObject
{
	public UnityAction<string> OnEventRaised;
	public void RaiseEvent(string arg)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(arg);
	}
}
