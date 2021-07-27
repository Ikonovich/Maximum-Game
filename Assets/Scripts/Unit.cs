
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;


namespace MaxGame {

    public class Unit : GameItem {


        [SerializeField, PropertyBackingField(nameof(LookSpeed))]
        private float lookSpeed = 0.1f;
        public float LookSpeed { get => lookSpeed; set => lookSpeed = value; }
        
        [SerializeField, PropertyBackingField(nameof(Acceleration))]
        private float acceleration = 0.1f;
        public float Acceleration { get => acceleration; set => acceleration = value; }
        
        [SerializeField, PropertyBackingField(nameof(MaximumVelocity))]
        private float maximumVelocity = 10.0f;
        public float MaximumVelocity { get => maximumVelocity; set => maximumVelocity = value; }

        [SerializeField, PropertyBackingField(nameof(VelocityDecayOn))]
        private bool velocityDecayOn = true;
        public bool VelocityDecayOn { get => velocityDecayOn; set => velocityDecayOn = value; }

        [SerializeField, PropertyBackingField(nameof(VelocityDecay))]
        private float velocityDecay = 1.0f;
        public float VelocityDecay { get => velocityDecay; set => velocityDecay = value; }

        [SerializeField, PropertyBackingField(nameof(IsPlayer))]
        private bool isPlayer = false;
        public bool IsPlayer { get => isPlayer; set => isPlayer = value; } 

        protected GameController GameController;

        protected Vector3 InputVector = Vector3.zero;

        protected Vector3 VelocityVector = Vector3.zero;

        
        [SerializeField, PropertyBackingField(nameof(FPSCameraObject))]
        private GameObject cameraObject;
        public GameObject FPSCameraObject { get => cameraObject; set => cameraObject = value; }


        protected PlayerInput PlayerInput;
        protected AudioListener Listener;

        protected Camera FirstPersonCamera;

        void Start() {

            Initialize();
        }

        protected override void Initialize() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();


            FirstPersonCamera = FPSCameraObject.GetComponent<Camera>();
            Listener = FPSCameraObject.GetComponent<AudioListener>();
            PlayerInput = GetComponent<PlayerInput>();

        }

        void Update() {

            
            if (IsPlayer == true) {
                PlayerInput = GetComponent<PlayerInput>();
                PlayerInput.ActivateInput();

            }
            else {
                PlayerInput.DeactivateInput();
            }

            ProcessMovement();
        }


        public override void Selected() {

            GameController.EnterUnit(this);
        }

        protected virtual void ProcessMovement() {

        }

        public void SetPlayer(bool active) {

            IsPlayer = active;

            Listener.enabled = active;
            FirstPersonCamera.enabled = active;
        
            
        }

        public Camera GetCamera() {

            return FirstPersonCamera;
        }

        public AudioListener GetListener() {

            return Listener;
        }
    }
}