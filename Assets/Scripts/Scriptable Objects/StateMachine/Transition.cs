using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Transition")]
public class Transition : DescriptionBaseSO
{
    [SerializeField] private State fromState;

}
