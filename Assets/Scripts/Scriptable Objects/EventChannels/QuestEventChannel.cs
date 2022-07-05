using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestEventChannel : DescriptionBaseSO
{
    public UnityAction<Quest> OnEventRaised;

	public void RaiseEvent(Quest arg)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(arg);
	}
}
