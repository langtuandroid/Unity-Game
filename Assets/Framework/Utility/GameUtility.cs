using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.Utility { 
    public static class GameUtility
    {
        public static void SetAbsoluteScale(GameObject obj, Vector2 size) {
            Transform trans = obj.transform;
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            Vector3 scale = trans.lossyScale;
            trans.localScale = new Vector3(size.x / scale.x, size.y / scale.y, 1);
        }

        public static Entity FindEntity(GameObject obj)
        {
            Entity entity = obj.gameObject.GetComponent<Entity>();
            if (entity == null && obj.layer == LayerMask.NameToLayer("Entities"))
            {
                entity = obj.GetComponentInParent<Entity>();
            }
            return entity;
        }

        public static void CrossFade(Animator animator, string animation, float transitionDuration, float normalizedTime) {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(info.fullPathHash, 0, info.normalizedTime);
            animator.CrossFadeInFixedTime(animation, transitionDuration, 0, normalizedTime);
        }
    }
}
