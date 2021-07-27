using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;


namespace MaxGame {


    public class FlyingUnit : Unit {


        protected float RollScalar = 0.0f;

        protected float RollSpeed = 0.2f;

        void Start() {

            Initialize();
        }

        protected override void ProcessMovement() {

            if (InputVector != Vector3.zero) {
                VelocityVector += InputVector * Time.deltaTime * Acceleration;
            }

            if (VelocityDecayOn == true) {

                VelocityVector -= (VelocityVector * VelocityDecay * Time.deltaTime) - InputVector * Time.deltaTime;
            }

            if (VelocityVector.magnitude > MaximumVelocity) {

                VelocityVector = VelocityVector.normalized * MaximumVelocity;
            }

            transform.Translate(VelocityVector);

            transform.Rotate(0.0f, 0.0f, RollScalar * RollSpeed);
        }

        
        /// <summary>
        /// This method handles mouse-based rotational movement in the X and Y axis. Z axis rotation is
        /// handled in OnRoll.
        /// </summmary>

        protected void OnLook(InputValue input) {

            Vector2 lookVector = input.Get<Vector2>() * LookSpeed;

            Debug.Log($"OnLook: {lookVector}");


            float zAxis = transform.rotation.eulerAngles.z;

            transform.Rotate(-lookVector.y, lookVector.x, 0.0f);

            //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, zAxis);
        }

        
        /// <summary>
        /// This method handles taking keyboard based directional input in the x and z axis.
        /// It updates the InputVector x and z axis while leaving the y axis the same.
        /// The y axis is updated in OnElevate.
        /// </summmary>
        protected void OnMove(InputValue input) {

            Vector2 tempVector = input.Get<Vector2>();

            InputVector = new Vector3(tempVector.x, InputVector.y, tempVector.y);
        }

        /// <summary>
        /// This method handles taking keyboard based directional input in the y axis.
        /// It updates the InputVector y axis while leaving the x and z axis the same.
        /// The x and z axis are updated in OnMove.
        /// </summmary>
        protected void OnElevate(InputValue input) {

            float elevationScalar = input.Get<float>();

            Debug.Log($"OnElevate: {elevationScalar}");


            InputVector = new Vector3(InputVector.x, elevationScalar, InputVector.z);
        }

        /// <summary>
        /// This method handles keyboard based rotational movement in the x and z axis.
        /// It updates the InputVector x and z axis while leaving the y axis the same.
        /// The y axis is updated in OnElevate.
        /// </summmary>
        protected void OnRoll(InputValue input) {

            RollScalar = -(input.Get<float>());
        }
    }
}