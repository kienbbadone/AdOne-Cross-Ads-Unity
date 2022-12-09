using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AdOne
{
    public class AdOneDemoCharacter : MonoBehaviour
    {
        public Joystick joystick;
        public Transform cam_RotateY;

        public float moveSpeed = 12f;
        public float rotateSpeed = 500f;

        public Animator anim;
        public AnimationClip clip_idle, clip_run;
        private int AnimHashBoolRunning = Animator.StringToHash("IsRunning");
        protected void Awake()
        {
            if (anim == null)
                anim = GetComponentInChildren<Animator>();
            if (anim)
            {
                AnimatorOverrideController OverrideAnim = new AnimatorOverrideController(anim.runtimeAnimatorController);
                anim.runtimeAnimatorController = OverrideAnim;

                if (clip_idle)
                    OverrideAnim["Idle"] = clip_idle;
                if (clip_run)
                    OverrideAnim["Run"] = clip_run;
            }
        }

        private void Update()
        {
            bool IsRunning = false;
            if (joystick)
            {
                var dir = joystick.Vertical * cam_RotateY.forward + joystick.Horizontal * cam_RotateY.right;
                dir.Normalize();
                IsRunning = dir.sqrMagnitude >= 0.0025f;
                if (IsRunning)
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
                }
            }
            if (anim)
                anim.SetBool(AnimHashBoolRunning, IsRunning);
        }
    }
}
