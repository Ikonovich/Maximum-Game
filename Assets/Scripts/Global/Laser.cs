
using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class Laser : Weapon { 

        [SerializeField, PropertyBackingField(nameof(StatusEffect))]
        private StatusEffect statusEffect;
        public StatusEffect StatusEffect { get => statusEffect; set => statusEffect = value; }
        
        void Awake() {
            
            FiringEffect = GetComponentInChildren<FiringEffect>();
            
        }

        public override void Fire(GameItem targetItem) {

            FiringEffect = GetComponentInChildren<FiringEffect>();

            GameObject target;

            if (targetItem.AimingPoint != null) {

                target = targetItem.AimingPoint;
            }
            else {
                target = targetItem.gameObject;
            }


            if (ReloadCountdown <= 0) { 

                ReloadCountdown = ReloadTime;

                Debug.Log($"Firing muh laser");

                targetItem.AddEffect(StatusEffect);
                FiringEffect.Play(this.transform.position + FiringOffset, target.transform.position);


                // if (Physics.Raycast(this.transform.position + FiringOffset, target.transform.position, out hit, Range)) {

                //     GameObject obj = hit.collider.gameObject;
                    
                //     FiringEffect.Play(this.transform.position + FiringOffset, hit.point);

                //     if (obj.GetComponent<GameItem>() == true) {

                //         Debug.Log($"Laser hit game item");
                //         GameItem item = obj.GetComponent<GameItem>();

                //         item.AddEffect(StatusEffect);
                //     }
                // }
                // else {
                    
                //     FiringEffect.Play(this.transform.position, this .transform.position + (transform.forward * 500));

                // }



            }
        }
        
        public override Vector2 GetFiringVector(Vector3 targetPoint) {


            TargetHelper.transform.LookAt(targetPoint, Vector3.up);
        
            return new Vector2(TargetHelper.transform.rotation.eulerAngles.x, TargetHelper.transform.rotation.eulerAngles.y);

        }

        public override bool OnTarget(GameItem target) {

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(this.transform.position + FiringOffset, transform.forward, out hit, Range, 0)) {

                GameObject obj = hit.transform.gameObject;

                if (obj.GetComponent<GameItem>() == true) {
                    GameItem item = obj.GetComponent<GameItem>();

                    if (item = target) {

                        return true;
                    }
                }
            }
            
            return false;
            
        }

        public override bool IsSafe(int teamID) {
            RaycastHit hit;
            
            if (Physics.Raycast(this.transform.position + FiringOffset, transform.forward, out hit, Range)) {

                GameObject obj = hit.collider.gameObject;
                    
                if (obj.GetComponent<GameItem>() == true) {

                    Debug.Log($"Weapon hit game item");
                    GameItem item = obj.GetComponent<GameItem>();

                    if (item.TeamID == teamID) {
                            
                        Debug.Log($"Weapon can't fire without hitting ally");
                        return false;
                    }
                }
            }
            return true;
        }




    }

}