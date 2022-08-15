using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Quest/Quest")]
public class Quest : DescriptionBaseSO
{
    [HideInInspector]
    [Tooltip("Condition for failing the quest")]
    [SerializeField] private Condition failCondition;

    [Tooltip("Operation that will be carried out once the quest is activated")]
    [SerializeField] private OperationReference operation;

    [Tooltip("Operation that will be carried out once the quest is fufilled")]
    [SerializeField] private OperationReference termination;

    [Tooltip("Condition for completing the quest")]
    [SerializeField] private Condition completeCondition;

    [HideInInspector]
    [Tooltip("Operation that will be carried out once the quest has failed")]
    [SerializeField] private OperationReference failOperation;

    [Header("Settings")]
    [SerializeField] private bool useFailCondition;

    private bool isEnabled = false;
    private Coroutine coroutine = null;

    public IEnumerator Operate()
    {
        operation.Operate();
        bool isCompleted = false;
        bool failed = false;
        yield return new WaitUntil(() => {
            isCompleted = completeCondition.Eval();
            if (useFailCondition) {
                failed = failCondition.Eval();
                return isCompleted || failed;
            }
            return isCompleted;
            
            
        });
        if (isCompleted)
        {
            termination.Operate();
        }
        else { 
            failOperation.Operate();
        }
        Disable();
    }

    public bool Enable() {
        if (isEnabled) {
            return false;
        }
        coroutine = GameManager.BeginCoroutine(Operate());
        return true;
    }

    public void Disable()
    {
        if (!isEnabled) {
            return;
        }
        isEnabled = false;
        GameManager.EndCoroutine(coroutine);
        coroutine = null;
    }

    public bool Enabled {
        get { return isEnabled; }
    }
}
