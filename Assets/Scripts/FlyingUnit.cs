using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;
using MaxGame.Units.Control;


namespace MaxGame {


    public class FlyingUnit : Unit {


        protected float RollScalar = 0.0f;

        protected float RollSpeed = 0.2f;

        void Start() {

            Initialize();
        }

        void Update() {

            if (InteractionCountdown > 0) {

                InteractionCountdown -= Time.deltaTime;

                if (InteractionCountdown <= 0) {
                    
                    // Do nothing
                }
            }

            if (IsSelected == true) {
            
                if (AttackTarget != null) {

                    TargetMarker.transform.position = AttackTarget.transform.position;
                    TargetMarker.SetActive(true);
                }
                else if (TargetPoint != Vector3.zero) {
                    TargetMarker.transform.position = TargetPoint;
                    TargetMarker.SetActive(true);
                    TargetMarker.transform.position = TargetPoint;

                }
            }
            
            if (IsPlayer == true) {
                PlayerInput = GetComponent<PlayerInput>();
                PlayerInput.ActivateInput();

            }
            else {
                PlayerInput = GetComponent<PlayerInput>();
                PlayerInput.DeactivateInput();
            }

            ProcessMovement();
            ProcessTargeting();
        }

        protected override void ProcessTargeting() {

            GameItem targetItem = AttackTarget.GetComponent<GameItem>();

            if ((AttackTarget != null) && (targetItem.TeamID != TeamID)) {

                Debug.Log($"Checking OnTarget");


                if (Weapons[ActiveWeapon].OnTarget(AttackTarget.transform.position) == true) {

                    Weapons[ActiveWeapon].Fire(targetItem);
                }
            }
        }

        // /// <summary>
        // /// This method handles mouse-based rotational movement in the X and Y axis. Z axis rotation is
        // /// handled in OnRoll.
        // /// </summmary>

        // protected void OnLook(InputValue input) {

        //     Vector2 lookVector = input.Get<Vector2>() * LookSpeed;

        //     Debug.Log($"OnLook: {lookVector}");


        //     float zAxis = transform.rotation.eulerAngles.z;

        //     transform.Rotate(-lookVector.y, lookVector.x, 0.0f);

        // }

        
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