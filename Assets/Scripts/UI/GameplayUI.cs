using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.QuestSystem;

namespace LobsterFramework.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Quest quest;

        public void EnableQuest()
        {
            quest.Enable();
        }
    }
}
