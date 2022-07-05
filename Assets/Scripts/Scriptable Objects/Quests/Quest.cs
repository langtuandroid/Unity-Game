using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Quest/Quest")]
public class Quest : DescriptionBaseSO
{
    [Tooltip("Quest manager responsible for handling this quest")]
    [SerializeField] private QuestManager questManager;
    [Tooltip("Priority of the quest, determines the order of evaluation")]
    [SerializeField] private VarInt priority;
    [Tooltip("Condition for failing the quest")]
    [SerializeField] private Condition failCondition;
    [Tooltip("Operation that will be carried out once the quest is activated")]
    [SerializeField] private Operation operation;
    [Tooltip("Operation that will be carried out once the quest is fufilled")]
    [SerializeField] private Operation termination;
    [Tooltip("Operation that will be carried out once the quest has failed")]
    [SerializeField] private Operation failOperation;
    [Tooltip("Whether this quest is enabled or not")]
    [SerializeField] private bool isEnabled;
    private bool isOperating;
    private bool isFailing;

    public void Operate()
    {
        if (!isEnabled)
        {
            return;
        }
        if (isOperating && !isFailing)
        {
            isOperating = operation.Start();
            isFailing = failCondition.Eval();
        } else if (isFailing) {
            isOperating = false;
            isFailing = failOperation.Start();
        }
        else {
            isEnabled = termination.Start();
        }
    }

    public bool Enable() {
        if (isEnabled) {
            return false;
        }
        isOperating = true;
        questManager.Add(this);
        return true;
    }

    public void Disable()
    {
        isEnabled = false;
        isOperating = false;
    }

    public bool Operating{
        get { return isOperating; }
    }

    public bool Failing { 
        get { return isFailing; }
    }

    public bool Enabled {
        get { return isEnabled; }
    }

    public int Priority
    {
        get { return priority.value; }
    }
}
