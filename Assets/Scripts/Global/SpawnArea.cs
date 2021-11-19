using System;
using UnityEngine;


namespace MaxGame {

    public class SpawnArea : MonoBehaviour{


        public bool IsOccupied = false;
        protected int ColliderCount = 0;

        void Update() {

            if (ColliderCount == 0) {
                IsOccupied = false;
            }
            else {
                IsOccupied = true;
            }
        }

        protected void OnTriggerEnter(Collider collider) {

            Debug.Log($"Spawn Area entered");
            ColliderCount += 1;     
        }

        protected void OnTriggerExit(Collider collider) {

            Debug.Log($"Spawn Area exited");
            ColliderCount -= 1;     

        }

    }
}