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

    [HideInInspector]
    [Tooltip("Condition for failing the quest")]
    [SerializeField] private Condition failCondition;

    [Tooltip("Operation that will be carried out once the quest is activated")]
    [SerializeField] private Operation operation;

    [Tooltip("Operation that will be carried out once the quest is fufilled")]
    [SerializeField] private Operation termination;

    [Tooltip("Condition for completing the quest")]
    [SerializeField] private Condition completeCondition;

    [HideInInspector]
    [Tooltip("Operation that will be carried out once the quest has failed")]
    [SerializeField] private Operation failOperation;

    private bool isEnabled;
    private bool isOperating;
    private bool isFailing;
    private bool isCompleted;

    [Header("Settings")]
    [SerializeField] private bool useFailCondition;

    private void OnEnable()
    {
        isEnabled = false;
        isOperating = false;
        isFailing = false;
        isCompleted = false;
        ReferenceCheck();
    }

    private void ReferenceCheck() {
        if (useFailCondition) {
            if (failCondition == null || failOperation == null) {
                Debug.LogWarning("The quest is using fail condition, but the associated condition/operation reference is not assigned!");
            }
        }
        if (operation == null) {
            Debug.LogWarning("The quest is missing the reference to the operation!");
        }
        if (completeCondition == null) {
            Debug.LogWarning("The quest is missing the reference to the complete condition!");
        }
    }

    public void Operate()
    {
        if (!isEnabled)
        {
            return;
        }
        if (isOperating && !isFailing) 
        {
            isOperating = operation.Start();
        } else if (isFailing) { // The quest will fail if the fail condition is satisfied
            isOperating = false;
            isEnabled = failOperation.Start();
            return;
        } else if (isCompleted) { // Complete the quest
            isEnabled = termination.Start();
            return;
        }
        if (useFailCondition)
        {
            isFailing = failCondition.Eval();
        }
        isCompleted = completeCondition.Eval();
    }

    public bool Enable() {
        if (isEnabled) {
            return false;
        }
        isOperating = true;
        isEnabled = true;
        isFailing = false;
        isCompleted = false;
        questManager.Add(this);
        return true;
    }

    public void Disable()
    {
        isEnabled = false;
        isOperating = false;
        isFailing = false;
        isCompleted = false;
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
        get { return priority.Value; }
    }
}
