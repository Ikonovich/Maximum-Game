using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class MissileLauncher : MunitionDeployer { 

        public List<GameObject> FiringPoints;

        public int CurrentFiringPoint = 0;
        

        public override Vector2 GetFiringVector(Vector3 targetPoint) {


            TargetHelper.transform.LookAt(targetPoint, Vector3.up);
        

            Debug.Log($"Returning missile launch angle");
            return new Vector2(345f, TargetHelper.transform.rotation.eulerAngles.y);

        }

        public override void Fire(GameItem targetItem) {

            if (ReloadCountdown <= 0) { 

                ReloadCountdown = ReloadTime;

                
                GameObject newMunition = Instantiate(Munition);
                Missile missile = newMunition.GetComponent<Missile>();
                GameItem parent = ParentObj.GetComponent<GameItem>();

                

                Debug.Log($"Deploying munition");

                newMunition.transform.position = FiringPoints[CurrentFiringPoint].transform.position;
                newMunition.transform.rotation = Barrel.transform.rotation;


                newMunition.transform.Translate(FiringOffset, Space.Self);
                missile.SetTarget(targetItem);
                missile.Launch(LaunchImpulse);

                CurrentFiringPoint += 1;
                
                if (CurrentFiringPoint >= FiringPoints.Count) {

                    CurrentFiringPoint = 0;
                }

            }
            else {

                ReloadCountdown -= Time.deltaTime;
            }

        }

        public override bool IsSafe(int teamID) {
            RaycastHit hit;
            
            if (Physics.Raycast(this.transform.position + FiringOffset, transform.forward, out hit, 10.0f)) {

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