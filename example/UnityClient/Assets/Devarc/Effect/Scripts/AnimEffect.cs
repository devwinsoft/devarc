using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public class AnimEffect : BaseEffect
    {
        public SimpleAnimList animDatas;

        protected SimpleAnimator Controller
        {
            get
            {
                if (mController == null)
                    mController = GetComponentInChildren<SimpleAnimator>();
                if (mController == null)
                    Debug.LogError("[AnimEffect] SimpleAnimator is not attached.");
                return mController;
            }
        }
        SimpleAnimator mController;
        float mPlaySpeed = 1f;


        public override void Clear()
        {
            Controller.Clear();
            base.Clear();
        }


        public override void SetSortingOrder(int _order)
        {
            var list = GetComponentsInChildren<SpriteRenderer>();
            foreach (var obj in list)
            {
                obj.sortingOrder = _order;
            }
        }


        protected override void onPause()
        {
            Controller.animator.speed = 0f;
        }

        protected override void onResume()
        {
            Controller.animator.speed = 1f;
        }


        protected override void onPlay()
        {
            Controller.animator.speed = mPlaySpeed;
            Controller.PlaySpeed = mPlaySpeed;
            Controller.PlayAnimation(animDatas.list, () =>
            {
                EffectManager.Instance.Remove(this);
            });
        }


        protected override void onStop()
        {
            EffectManager.Instance.Remove(this);
        }


        public override void onAwake()
        {
            mPlaySpeed = Controller.animator.speed;
        }

        public override void onLateUpdate()
        {
        }
    }
}
