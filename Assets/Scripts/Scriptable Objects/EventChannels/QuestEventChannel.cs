using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestEventChannel : DescriptionBaseSO
{
    public UnityAction<QuestSO> OnEventRaised;

	public void RaiseEvent(QuestSO arg)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(arg);
	}
}
