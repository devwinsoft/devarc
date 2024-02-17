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

        float mReaminTime = 0f;

        public ParticleSystem particle
        {
            get
            {
                if (mParticle == null)
                    mParticle = GetComponentInChildren<ParticleSystem>(true);
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
            mReaminTime = playTime;
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
                    Invoke("Remove", fadeOutTime);
                }
                else
                {
                    Remove();
                }
            }
            else
            {
                Remove();
            }
        }

        public override void onLateUpdate()
        {
            if (playTime > 0f)
            {
                mReaminTime -= Time.deltaTime;
                if (mReaminTime <= 0f)
                    Remove();
            }
            //else if (particle.IsAlive(true) == false)
            //{
            //    Remove();
            //}
        }
    }
}

