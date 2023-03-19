using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.QuestSystem
{
    public class LogOperation : Operation
    {
        [SerializeField] private string str;
        public override void Begin()
        {
            Debug.Log(str);
        }
    }
}
