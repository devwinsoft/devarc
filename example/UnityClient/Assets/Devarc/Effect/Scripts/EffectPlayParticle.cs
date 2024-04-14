using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;


namespace Devarc
{
    public class EffectPlayParticle : BaseEffectPlay
    {
        public float playTime;
        public float fadeOutTime;

        float mReaminTime = 0f;
        ParticleSystem[] mParticles = null;

        public override void Clear()
        {
            foreach (var particle in mParticles)
            {
                particle.Stop(true);
                particle.Clear(true);
            }
            base.Clear();
        }

        public override void onAwake()
        {
            mParticles = GetComponentsInChildren<ParticleSystem>();
        }

        protected override void onPause()
        {
            foreach (var particle in mParticles)
            {
                if (particle.isPlaying)
                {
                    particle.Pause();
                }
            }
        }

        protected override void onResume()
        {
            foreach (var particle in mParticles)
            {
                if (particle.isPaused)
                {
                    particle.Play();
                }
            }
        }

        protected override void onPlay()
        {
            mReaminTime = playTime;
            foreach (var particle in mParticles)
            {
                particle.Play();
            }
            if (playTime > 0f)
            {
                Invoke("Stop", playTime);
            }
        }

        protected override void onStop()
        {
            foreach (var particle in mParticles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

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

        public override void onLateUpdate()
        {
            if (playTime > 0f)
            {
                mReaminTime -= Time.deltaTime;
                if (mReaminTime <= 0f)
                    Remove();
            }
            else
            {
                bool isAlive = false;
                foreach (var particle in mParticles)
                {
                    if (particle.IsAlive(true))
                    {
                        isAlive = true;
                    }
                }

                if (isAlive == false)
                {
                    Remove();
                }
            }
        }
    }
}
