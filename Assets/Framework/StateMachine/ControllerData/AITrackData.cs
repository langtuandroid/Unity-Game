using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ControllerData/AITrackData")]
public class AITrackData : ControllerData
{
    public RefFloat sightRange;
    public RefFloat engageDistance;
    public RefFloat chaseDistance;
    public RefFloat keepDistance;
}
