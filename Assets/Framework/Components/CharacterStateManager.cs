using LobsterFramework.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using System;
using LobsterFramework.Utility.BufferedStats;
using UnityEngine.UIElements;

namespace LobsterFramework.EntitySystem
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(Poise))]
    [RequireComponent(typeof(MovementController))]
    public class CharacterStateManager : MonoBehaviour
    {
        [Header("Component Reference")]
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private WeaponWielder weaponWielder;
        [SerializeField] private AbilityRunner abilityRunner;

        [Header("Animations")]
        [SerializeField] private AnimationClip onPostureBroken;

        [Header("Status")]
        [ReadOnly][SerializeField] private CharacterState characterState;
        private readonly static Array enumStates = Enum.GetValues(typeof(CharacterState));
        private bool[] stateMap;

        // Requried Components
        private Entity entity;
        private Poise poise;
        private MovementController moveControl;

        // Suppression
        private BaseOr suppression;

        // Movement block keys
        private int postureBlockKey;
        private int poiseBlockKey;
        private int poiseAbilityBlockKey;
        private int suppressBlockKey;
        private int suppressAbilityBlockKey;

        void Start()
        {
            suppression = new(false);

            poise = GetComponent<Poise>();
            entity = GetComponent<Entity>();
            moveControl = GetComponent<MovementController>();
            poiseBlockKey = -1;
            postureBlockKey = -1;
            poiseAbilityBlockKey = -1;

            characterState = CharacterState.Normal;
            poise.onPoiseStatusChange += OnPoiseChange;
            entity.onPostureStatusChange += OnPostureChange;
            abilityRunner.onAbilityAnimation += OnAbilityAnimation;
            PlayAnimation(CharacterState.Normal);

            stateMap = new bool[enumStates.Length];
        }

        public int Suppress()
        {
            int key = suppression.AddEffector(true);
            stateMap[(int)CharacterState.Suppressed] = true;
            if (suppression.EffectorCount == 1)
            {
                suppressAbilityBlockKey = abilityRunner.BlockAction();
                suppressBlockKey = moveControl.BlockMovement();
                moveControl.SetVelocityImmediate(Vector2.zero);
            }
            ComputeStateAndPlayAnimation();
            return key;
        }

        public bool Release(int key)
        {
            if (suppression.RemoveEffector(key))
            {
                if (suppression.EffectorCount == 0)
                {
                    stateMap[(int)CharacterState.Suppressed] = false;
                    abilityRunner.UnblockAction(suppressAbilityBlockKey);
                    moveControl.UnblockMovement(suppressBlockKey);
                    ComputeStateAndPlayAnimation();
                }
            }
            return false;
        }

        private void OnPoiseChange(bool poiseBroken)
        {
            stateMap[(int)CharacterState.PoiseBroken] = poiseBroken;
            if (poiseBroken)
            {
                if (poiseBlockKey == -1)
                {
                    poiseBlockKey = moveControl.BlockMovement();
                }
                if (poiseAbilityBlockKey == -1)
                {
                    poiseAbilityBlockKey = abilityRunner.BlockAction();
                }
            }
            else
            {
                if (poiseBlockKey != -1)
                {
                    moveControl.UnblockMovement(poiseBlockKey);
                    poiseBlockKey = -1;
                }
                if (poiseAbilityBlockKey != -1)
                {
                    abilityRunner.UnblockAction(poiseAbilityBlockKey);
                    poiseAbilityBlockKey = -1;
                }
            }
            ComputeStateAndPlayAnimation();
        }

        private void OnPostureChange(bool postureBroken)
        {
            stateMap[(int)CharacterState.PostureBroken] = postureBroken;
            if (postureBroken)
            {
                if (postureBlockKey == -1)
                {
                    postureBlockKey = moveControl.BlockMovement();
                }
            }
            else
            {
                if (postureBlockKey != -1)
                {
                    moveControl.UnblockMovement(postureBlockKey);
                    postureBlockKey = -1;
                }
            }
            ComputeStateAndPlayAnimation();
        }

        private void OnAbilityAnimation(bool casting)
        {
            stateMap[(int)CharacterState.AbilityCasting] = casting;
            ComputeStateAndPlayAnimation();
        }

        private void ComputeStateAndPlayAnimation()
        {
            CharacterState prev = characterState;
            ComputeState();
            if (prev != characterState)
            {
                PlayAnimation(prev);
            }
        }

        private void ComputeState()
        {
            foreach (CharacterState state in enumStates)
            {
                if (stateMap[(int)state])
                {
                    characterState = state;
                    return;
                }
            }
            characterState = CharacterState.Normal;
        }

        private void PlayAnimation(CharacterState prevState)
        {
            switch (characterState)
            {
                case CharacterState.Normal:
                    if (prevState == CharacterState.AbilityCasting)
                    {
                        weaponWielder.PlayWeaponAnimation();
                    }
                    else
                    {
                        weaponWielder.PlayTransitionAnimation();
                    }
                    break;
                case CharacterState.PostureBroken:
                    PlayPostureBrokenClip();
                    break;
                case CharacterState.Dashing:
                    break;
                case CharacterState.PoiseBroken:
                    PlayPostureBrokenClip();
                    break;
                case CharacterState.Suppressed:
                    PlayPostureBrokenClip();
                    break;
                default: break;
            }
        }
        private void PlayPostureBrokenClip(float fadeTime = 0.25f)
        {
            foreach (AnimancerState state in animancer.States)
            {
                state.IsPlaying = false;
            }
            animancer.Play(onPostureBroken, fadeTime, FadeMode.FromStart);
        }

        /// <summary>
        /// The state of the character, ordered by their priorities. If the conditions for multiple character states are met, only the one with the highest
        /// priority will take place.
        /// </summary>
        public enum CharacterState
        {
            PostureBroken,
            PoiseBroken,
            Suppressed,
            Dashing,
            AbilityCasting,
            Normal,
        }
    }
}
