
using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class MunitionDeployer : Weapon { 

        [SerializeField, PropertyBackingField(nameof(Munition))]
        private GameObject munition;
        public GameObject Munition { get => munition; set => munition = value; }

        [SerializeField, PropertyBackingField(nameof(LaunchImpulse))]
        private Vector3 launchImpulse = Vector3.zero;
        public Vector3 LaunchImpulse { get => launchImpulse; set => launchImpulse = value; }

        
        void Awake() {

            FiringEffect = GetComponent<FiringEffect>();
        }

        
        public override void Fire(GameItem target) {

            if (ReloadCountdown <= 0) { 

                
                ReloadCountdown = ReloadTime;

                Debug.Log($"Deploying munition");
                GameObject newMunition = Instantiate(Munition);
                Munition munition = newMunition.GetComponent<Munition>();

                if (ParentObj == null) {
                    Debug.Log($"Parent object is null");
                }
                if (ParentObj.GetComponent<GameItem>().TeamID == null) {

                    Debug.Log($"Parent game item null");

                }
                munition.TeamID = ParentObj.GetComponent<GameItem>().TeamID;
                newMunition.transform.position = this.transform.position;
                newMunition.transform.rotation = this.transform.rotation;


                newMunition.transform.Translate(FiringOffset);

                munition.ApplyImpulse(LaunchImpulse);

            }
            else {

                ReloadCountdown -= Time.deltaTime;
            }

        }

        public override Vector2 GetFiringVector(Vector3 targetPoint) {


            TargetHelper.transform.LookAt(targetPoint, Vector3.up);
        
            return new Vector2(TargetHelper.transform.rotation.eulerAngles.x, TargetHelper.transform.rotation.eulerAngles.y);

        }

        public override bool OnTarget(Vector3 targetPoint) {

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



