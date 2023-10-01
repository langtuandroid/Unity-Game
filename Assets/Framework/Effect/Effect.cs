using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Pool;

namespace LobsterFramework.Effects
{
    public abstract class Effect : ScriptableObject
    {
        [SerializeField] private RefFloat duration;
        [SerializeField] private Sprite icon;
        [SerializeField] private VarString spritePoolTag;
        protected EffectProcessor processor;

        public Sprite Icon { get { return icon; } }

        public float Counter_t { get; private set; }
        public float Duration { get { return duration.Value; } }

        public void ActivateEffect(EffectProcessor processor)
        {
            Counter_t = 0;
            this.processor = processor;
            // Display effect icon
            if (icon && spritePoolTag)
            {
                GameObject g = ObjectPool.GetObject(spritePoolTag.Value, Vector3.zero, Quaternion.identity);
                Transform gTransform = g.transform;
                Transform pTransform = processor.transform;
                gTransform.SetParent(pTransform);
                gTransform.localPosition = Vector3.zero;
                gTransform.localScale = Vector3.one;
                TemporalObject tmp = g.GetComponent<TemporalObject>();
                tmp.ResetCounter();
                tmp.duration = duration.Value;
                SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = icon;
            }
            OnApply();
        }

        public bool Update()
        {
            if (Counter_t >= duration.Value)
            {
                OnEffectOver();
                return false;
            }
            OnUpdate();
            Counter_t += Time.deltaTime;
            return true;
        }

        protected virtual void OnApply() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnEffectOver() { }

        public void TerminateEffect() {
            OnEffectOver();
        }
    }
}
