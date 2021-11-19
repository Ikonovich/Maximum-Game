using System;
using UnityEngine;
using Candlelight;

namespace MaxGame { 

    public class Missile : Munition {

        [SerializeField, PropertyBackingField(nameof(MaxFlightTime))]
        private float maxFlightTime = 10.0f;
        public float MaxFlightTime { get => maxFlightTime; set => maxFlightTime = value; }
        
        [SerializeField, PropertyBackingField(nameof(FlightForce))]
        private float flightForce = 100.0f;
        public float FlightForce { get => flightForce; set => flightForce = value; }
        
        [SerializeField, PropertyBackingField(nameof(TargetHelper))]
        private GameObject targetHelper;
        public GameObject TargetHelper { get => targetHelper; set => targetHelper = value; }
        

        protected float FlightTime = 0.0f;

        public float NoColliderTime = 0.10f;




        protected GameItem Target;

        void Awake() {

            Body = this.GetComponent<Rigidbody>();

            Collider collider = GetComponent<Collider>();
            collider.enabled = false;

        }

        void Update() {

            if (FlightTime > NoColliderTime) {
                    
                Collider collider = GetComponent<Collider>();
                collider.enabled = true;
                Debug.Log($"Collider reactivated");

            }
        }
        public void SetTarget(GameItem target) {

            Target = target;

            Debug.Log($"Missile target set.");
        }


    }

}