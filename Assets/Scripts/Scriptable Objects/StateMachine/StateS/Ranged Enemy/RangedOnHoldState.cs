using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/States/AI/Ranged-OnHoldState")]
public class RangedOnHoldState : State
{
    [SerializeField] private RefFloat onHoldTime;
    private float onHoldCounter;
    private AIController controller;
    private Transform transform;
    private AITrackData trackData;
 
    public override void InitializeFields(GameObject obj)
    {
        onHoldCounter = 0;
        controller = obj.GetComponent<AIController>();
        trackData = controller.GetControllerData<AITrackData>();
        transform = obj.transform;
    }

    public override void OnEnter()
    {
        Debug.Log("OnHold");
        onHoldCounter = 0;
        controller.StopPathing = true;
    }

    public override void OnExit()
    {
        controller.StopPathing = false;
    }

    public override Type Tick()
    {
        onHoldCounter += Time.deltaTime;
        if (onHoldCounter >= onHoldTime.Value) {
            controller.ClearPath();
            return typeof(WanderState);
        }
        if (controller.TargetVisible(transform.position, trackData.engageDistance.Value)) {
            return typeof(RangedChaseState);
        }

        if (controller.TargetVisible(controller.NextWayPoint(), trackData.keepDistance.Value)) {
            return null;
        }
        return typeof(RangedChaseState);
    }
}
