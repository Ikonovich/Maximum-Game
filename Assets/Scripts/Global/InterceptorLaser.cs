
using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class InterceptorLaser : Weapon {

        protected void Awake() {

            //FiringEffect = FiringEffect.GetComponent<FiringEffect>();
        }


        public override void Fire(GameItem targetItem) {

            Debug.Log($"Interceptor laser fire called");


            if (ReloadCountdown <= 0) { 

                GameObject target;

                if (targetItem.AimingPoint != null) {

                    target = targetItem.AimingPoint;
                }
                else {
                    target = targetItem.gameObject;
                }



                Debug.Log($"Firing interceptor laser");

                RaycastHit hit;

                //if (Physics.Raycast(this.transform.position + FiringOffset, target.transform.position, out hit, Range)) {

                    //GameObject obj = hit.collider.gameObject;
                    
                FiringEffect.Play(this.transform.position, target.transform.position);

                Munition munition = target.GetComponent<Munition>();

                
                if (munition != null) {

                    Debug.Log($"Interceptor hit munition");
                    
                    
                    if (munition.Intercept(false) == true) {

                        ReloadCountdown = ReloadTime;
                    }
                }
                // }
                // else {
                    
                //     FiringEffect.Play(this.transform.position, this.transform.position + (this.transform.forward * 500));

                // }



            }
        }


        public override Vector2 GetFiringVector(Vector3 targetPoint) {


            TargetHelper.transform.LookAt(targetPoint, Vector3.up);
        
            return new Vector2(TargetHelper.transform.rotation.eulerAngles.x, TargetHelper.transform.rotation.eulerAngles.y);

        }
    }
} 