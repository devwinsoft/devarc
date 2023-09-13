using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


namespace Devarc
{
    public class ParticleEffect : BaseEffect
    {
        public float playTime;
        public float fadeOutTime;

        public ParticleSystem particle
        {
            get
            {
                if (mParticle == null)
                    mParticle = GetComponent<ParticleSystem>();
                return mParticle;
            }
        }
        ParticleSystem mParticle = null;

        public override void Clear()
        {
            particle.Stop(true);
            particle.Clear(true);
            base.Clear();
        }

        public override void onAwake()
        {
            mParticle = GetComponent<ParticleSystem>();
        }

        protected override void onPause()
        {
            if (particle.isPlaying)
            {
                particle.Pause(true);
            }
        }

        protected override void onResume()
        {
            if (particle.isPaused)
            {
                particle.Play(true);
            }
        }

        protected override void onPlay()
        {
            particle.Play(true);
            if (playTime > 0f)
            {
                Invoke("Stop", playTime);
            }
        }

        protected override void onStop()
        {
            if (particle.isPlaying)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                transform.SetParent(EffectManager.Instance.transform, true);
                if (fadeOutTime > 0f)
                {
                    Invoke("Complete", fadeOutTime);
                }
            }
            else
            {
                Complete();
            }
        }

        public override void onLateUpdate()
        {
            if (playTime == 0 && particle != null)
            {
                if (particle.IsAlive(true) == false)
                {
                    EffectManager.Instance.Push(this);
                }
            }
        }
    }
}

