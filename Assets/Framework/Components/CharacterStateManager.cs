using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;
using UnityEngine;
using Animancer;
using System;

namespace LobsterFramework
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
        public readonly BaseOr suppression = new(false);

        // Movement block keys
        private BufferedValueAccessor<bool> postureMoveLock;
        private BufferedValueAccessor<bool> poiseMoveLock;
        private BufferedValueAccessor<bool> poiseAbilityLock;
        private BufferedValueAccessor<bool> suppressMoveLock;
        private BufferedValueAccessor<bool> suppressAbilityLock;

        void Start()
        {
            poise = GetComponent<Poise>();
            entity = GetComponent<Entity>();
            moveControl = GetComponent<MovementController>();

            postureMoveLock = moveControl.movementLock.GetAccessor();
            poiseMoveLock = moveControl.movementLock.GetAccessor();
            suppressMoveLock = moveControl.movementLock.GetAccessor();

            suppressAbilityLock = abilityRunner.actionLock.GetAccessor();
            poiseAbilityLock = abilityRunner.actionLock.GetAccessor();

            characterState = CharacterState.Normal;
            poise.onPoiseStatusChange += OnPoiseStatusChanged;
            entity.onPostureStatusChange += OnPostureStatusChanged;
            abilityRunner.onAbilityAnimation += OnAbilityAnimation;

            PlayAnimation(CharacterState.Normal);
            stateMap = new bool[enumStates.Length];
        }

        private void OnEnable()
        {
            suppression.onValueChanged += OnSuppressionStatusChanged;
        }

        private void OnDisable()
        {
            suppression.onValueChanged -= OnSuppressionStatusChanged;
        }

        #region StatusListeners
        private void OnSuppressionStatusChanged(bool suppressed) {
            if (suppressed)
            {
                suppressMoveLock.Acquire(true);
                suppressAbilityLock.Acquire(true);
                moveControl.SetVelocityImmediate(Vector2.zero);
            }
            else {
                stateMap[(int)CharacterState.Suppressed] = false;
                suppressMoveLock.Release();
                suppressAbilityLock.Release();
                ComputeStateAndPlayAnimation();
            }
        }
        

        private void OnPoiseStatusChanged(bool poiseBroken)
        {
            stateMap[(int)CharacterState.PoiseBroken] = poiseBroken;
            if (poiseBroken)
            {
                poiseMoveLock.Acquire(true);
                poiseAbilityLock.Acquire(true);
            }
            else
            {
                poiseMoveLock.Release();
                poiseAbilityLock.Release();
            }
            ComputeStateAndPlayAnimation();
        }

        private void OnPostureStatusChanged(bool postureBroken)
        {
            stateMap[(int)CharacterState.PostureBroken] = postureBroken;
            if (postureBroken)
            {
                postureMoveLock.Acquire(true);
            }
            else
            {
                postureMoveLock.Release();
            }
            ComputeStateAndPlayAnimation();
        }

        private void OnAbilityAnimation(bool casting)
        {
            stateMap[(int)CharacterState.AbilityCasting] = casting;
            ComputeStateAndPlayAnimation();
        }
        #endregion

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
        private void PlayPostureBrokenClip(float fadeTime = 0.15f)
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
