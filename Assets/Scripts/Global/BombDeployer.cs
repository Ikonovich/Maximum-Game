using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class BombDeployer : MunitionDeployer { 


        public override bool OnTarget(Vector3 targetPoint) {
            
            float distance = (targetPoint - transform.position).magnitude;

            if (distance < Range) {
                
                
                Debug.Log($"Bomb deployer on target");

                return true;
            }
            Debug.Log($"Bomb deployer off target with distance " + distance.ToString());


            return false;

        }
    }
}



