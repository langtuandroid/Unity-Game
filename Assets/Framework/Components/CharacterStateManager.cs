using LobsterFramework.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

namespace LobsterFramework.EntitySystem
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent (typeof(Poise))]
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
        [ReadOnly] [SerializeField] private CharacterState characterState;

        private Entity entity;
        private Poise poise;
        private MovementController moveControl;

        // Movement block keys
        private int postureBlockKey;
        private int poiseBlockKey;
        private int poiseAbilityBlockKey;

        private bool postureBroken;
        private bool poiseBroken;
        private bool dashing;
        private bool casting;

        void Start()
        {
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
        }

        private void OnPoiseChange(bool poiseBroken) {
            this.poiseBroken = poiseBroken;
            if (poiseBroken)
            {
                if (poiseBlockKey == -1) {
                    poiseBlockKey = moveControl.BlockMovement();
                }
                if (poiseAbilityBlockKey == -1) {
                    poiseAbilityBlockKey = abilityRunner.BlockAction();
                }
            }
            else {
                if (poiseBlockKey != -1) {
                    moveControl.UnblockMovement(poiseBlockKey);
                    poiseBlockKey = -1;
                }
                if (poiseAbilityBlockKey != -1) {
                    abilityRunner.UnblockAction(poiseAbilityBlockKey);
                    poiseAbilityBlockKey = -1;
                }
            }
            ComputeStateAndPlayAnimation();
        }

        private void OnPostureChange(bool postureBroken) {
            this.postureBroken = postureBroken;
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

        private void OnAbilityAnimation(bool casting) { 
            this.casting = casting;
            ComputeStateAndPlayAnimation();
        }

        private void ComputeStateAndPlayAnimation() {
            CharacterState prev = characterState;
            ComputeState();
            if (prev != characterState)
            {
                PlayAnimation(prev);
            }
        }

        private void ComputeState() {
            
            if (postureBroken) {
                characterState = CharacterState.PostureBroken;
                return;
            }
            if (poiseBroken) {
                characterState = CharacterState.PoiseBroken;
                return;
            }
            if (dashing) {
                characterState = CharacterState.Dashing;
                return;
            }
            if (casting) {
                characterState = CharacterState.AbilityCasting;
                return;
            }
            characterState = CharacterState.Normal;
        }

        private void PlayAnimation(CharacterState prevState) { 
            switch(characterState)
            {
                case CharacterState.Normal:
                    if (prevState == CharacterState.AbilityCasting)
                    {
                        weaponWielder.PlayWeaponAnimation();
                    }
                    else {
                        weaponWielder.PlayWeaponAnimation(true);
                    }
                     break;
                case CharacterState.PostureBroken:
                    animancer.Play(onPostureBroken, 0.25f, FadeMode.FromStart);
                    break;
                case CharacterState.Dashing:
                    break;
                case CharacterState.PoiseBroken:
                    animancer.Play(onPostureBroken, 0.4f, FadeMode.FromStart);
                    break;
                default: break;
            }
        }
    }

    public enum CharacterState { 
        Normal,
        PostureBroken,
        PoiseBroken,
        Dashing,
        AbilityCasting
    }
}
