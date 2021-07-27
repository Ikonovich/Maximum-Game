using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MaxGame {

    public class RollingUnit : Unit {

        TankController TankController;


        void Start() {
            Initialize();
        }

        protected override void Initialize() {

            TankController = GetComponent<TankController>();

        }

        void Update() {

            ProcessMovement();
        }


        protected void OnLook(InputValue input) {

            Vector2 lookVector = input.Get<Vector2>() * LookSpeed;

            Debug.Log($"OnLook: {lookVector}");


            float zAxis = transform.rotation.eulerAngles.z;

            TankController.BarrelRotation = lookVector.x;

            TankController.TurretRotation = lookVector.y;
            //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, zAxis);
        }

    }
}