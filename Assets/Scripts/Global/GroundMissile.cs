using System;
using UnityEngine;
using Candlelight;
    

namespace MaxGame { 

    
    /// <summary>
    /// This class controls a missile intended to be launched from a 
    /// roughly horizontal, ground based platform.
    /// In the initial launch phase, it clears the launcher and turns up,
    /// before reorienting itself to attack the set target.
    /// </summary>


    public class GroundMissile : Missile {


        [Header("First Phase")]
        public float FirstPhaseStartTime = 0.15f;

        public float FirstPhaseForce = 0.0f;

        public float FirstPhaseTorque = -900.0f;

        public float FirstPhaseLift = 0.0f;

        
        [Header("Second Phase")]
        public float SecondPhaseStartTime = 0.25f;

        public float SecondPhaseForce = 50.0f;

        public float SecondPhaseTorque = 0.0f;

        public float SecondPhaseLift = 0.0f;

        [Header("Third Phase")]
        public float ThirdPhaseStartTime = 0.6f;

        public float ThirdPhaseForce = 0.0f;

        public float ThirdPhaseTorque = 700.0f;

        public float ThirdPhaseLift = 0.0f;

        
        [Header("Fourth Phase")]

        public float FourthPhaseStartTime = 1.70f;

        public float FourthPhaseForce = 50.0f;

        public float FourthPhaseTorque = 700.0f;

        public float FourthPhaseLift = 9.81f;





        void Awake() {

            Body = this.GetComponent<Rigidbody>();

        }

        void Update() {

             if (FlightTime > NoColliderTime) {
                    
                Collider collider = GetComponent<Collider>();
                collider.enabled = true;
                Debug.Log($"Collider reactivated");

            }

            if (Target != null) {

                FlightTime += Time.deltaTime;

                if (FlightTime > MaxFlightTime) {

                    Kill();

                }

                else if (FlightTime > FourthPhaseStartTime) {

                    TargetHelper.transform.LookAt(Target.transform.position, Vector3.up);

                    Body.rotation = Quaternion.Lerp(Body.rotation, TargetHelper.transform.rotation, 10.0f * Time.deltaTime);

                    Body.AddRelativeForce(new Vector3(0.0f, 0.0f, ThirdPhaseForce));


                    Body.AddForce(new Vector3(0.0f, FourthPhaseLift, 0.0f));

                    Body.AddRelativeTorque(new Vector3(FourthPhaseTorque, 0.0f, 0.0f));


                }

                else if (FlightTime > ThirdPhaseStartTime) {

                    Body.AddRelativeForce(new Vector3(0.0f, 0.0f, ThirdPhaseForce));

                    Body.AddForce(new Vector3(0.0f, ThirdPhaseLift, 0.0f));

                    Body.AddRelativeTorque(new Vector3(ThirdPhaseTorque, 0.0f, 0.0f));


                }
                else if (FlightTime > SecondPhaseStartTime) {

                    Body.AddRelativeForce(new Vector3(0.0f, 0.0f, SecondPhaseForce));
                    
                    Body.AddForce(new Vector3(0.0f, SecondPhaseLift, 0.0f));

                    Body.AddRelativeTorque(new Vector3(SecondPhaseTorque, 0.0f, 0.0f));


                }
                else if (FlightTime > FirstPhaseStartTime) {

                    Body.AddRelativeForce(new Vector3(0.0f, 0.0f, FirstPhaseForce));
                    
                    Body.AddForce(new Vector3(0.0f, FirstPhaseLift, 0.0f));

                    Body.AddRelativeTorque(new Vector3(FirstPhaseTorque, 0.0f, 0.0f));


                }
            }
            else {

                Kill();
            }
        }
    }
}