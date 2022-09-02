using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
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

    [Header("Settings")]
    [SerializeField] private bool useFailCondition;

    private Coroutine coroutine = null;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator Operate()
    {
        operation.Begin();
        bool isCompleted = false;
        bool failed = false;
        yield return new WaitUntil(() => {
            isCompleted = completeCondition.Eval();
            if (useFailCondition)
            {
                failed = failCondition.Eval();
                return isCompleted || failed;
            }
            return isCompleted;
        });
        if (isCompleted)
        {
            termination.Begin();
        }
        else
        {
            failOperation.Begin();
        }
        Disable();
    }

    public bool Enable()
    {
        if (coroutine != null)
        {
            return false;
        }
        coroutine = GameManager.BeginCoroutine(Operate());
        return true;
    }

    public void Disable()
    {
        if (coroutine == null)
        {
            return;
        }
        GameManager.EndCoroutine(coroutine);
        coroutine = null;
    }

    public bool Enabled
    {
        get { return coroutine == null; }
    }
}
