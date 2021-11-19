using System;
using UnityEngine;

namespace MaxGame {

    /// <summary>
    /// This class tracks the object or point that has been assigned by it's owner.
    /// It also floats up and down. Every time the float Countdown hits 0, IsFloatingUp flips
    /// value.
    /// </summary>

    public class TargetMarker : MonoBehaviour {

        protected Vector3 TargetPoint = Vector3.zero;

        protected GameItem TargetItem;


        protected float CenterHeight = 30.0f;

        protected float FloatOffset = 0.0f;

        protected float FloatTime = 1.7f;

        protected float FloatCountdown = 1.7f;

        protected float FloatStrength = 10.0f;

        protected bool IsFloatingUp = true;

        protected void Update() {

            
            transform.rotation = Quaternion.identity;


            if (TargetItem != null) {

                TargetPoint = TargetItem.gameObject.transform.position;
            }

            if (TargetPoint != Vector3.zero) {

                transform.position = new Vector3(TargetPoint.x, TargetPoint.y + CenterHeight + FloatOffset, TargetPoint.z);
            }

            // This code enables the floating effect.

            FloatCountdown -= Time.deltaTime;

            if (FloatCountdown <= 0) {

                IsFloatingUp = !IsFloatingUp;
                FloatCountdown = FloatTime;
            }

            float floatDir = -1.0f;

            if (IsFloatingUp == true) {
                floatDir = 1.0f;
            }

            FloatOffset = FloatOffset + floatDir * FloatStrength * Time.deltaTime * FloatCountdown / FloatTime;
        }

        public void SetTarget(Vector3 target) {

            TargetPoint = target;
        }

        public void SetTarget(GameItem target) {

            TargetItem = target;
        }

        public void ClearTarget() {

            TargetPoint = Vector3.zero;
            TargetItem = null;
        }

    }


}