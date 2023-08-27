using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LobsterFramework
{
	[CreateAssetMenu(menuName = "EventChannel/SceneEventChannel")]
	public class SceneEventChannel : DescriptionBaseSO
	{
		public UnityAction<Scene> OnEventRaised;
		public void RaiseEvent(Scene scene)
		{
			OnEventRaised.Invoke(scene);
		}
	}
}
