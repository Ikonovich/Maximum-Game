
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Candlelight;
using MaxGame.Units.Control;



namespace MaxGame {

    [RequireComponent(typeof(SensorSphere))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]


    public class Unit : GameItem {
       
        [SerializeField, PropertyBackingField(nameof(IsPlayer))]
        private bool isPlayer = false;
        public bool IsPlayer { get => isPlayer; set => isPlayer = value; } 
        
        [Header("Unit Objects")]
        [SerializeField, PropertyBackingField(nameof(FPSCameraObject))]
        private GameObject cameraObject;
        public GameObject FPSCameraObject { get => cameraObject; set => cameraObject = value; }

        [HideInInspector]
        public GameObject TargetMarker;


        [HideInInspector]
        public GameItem FollowTarget;

        protected Weapon[] Weapons;

        protected Weapon CurrentWeapon;

        protected int ActiveWeapon = 0;
        

        protected UnitController UnitController;

        protected Vector3 InputVector = Vector3.zero;

        [HideInInspector]
        public Vector3 Velocity = Vector3.zero;
        
        protected PlayerInput PlayerInput;
        protected AudioListener Listener;

        protected Camera FirstPersonCamera;

        protected float ScanTime = 0.2f;

        protected float ScanCountdown = 0.0f;

        protected CollisionDetector Detector;

        protected NavMeshAgent Agent;
    
        protected Formation Formation;

        public string Stance = "Aggressive";

        public int Range = 50;


        void Awake() {

            Initialize();


        }

        void Start() {

            Register();
        }


        protected override void Initialize() {
            
            

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            UnitController = GetComponent<UnitController>();

            TargetMarker = GetComponentInChildren<TargetMarker>(true).gameObject;

            Agent = GetComponent<NavMeshAgent>();

            //FirstPersonCamera = FPSCameraObject.GetComponent<Camera>();
            //Listener = FPSCameraObject.GetComponent<AudioListener>();

            Transform tempTransform = transform.Find("SelectionEffect");
            SelectionEffect = tempTransform.gameObject;

            StatusEffects = new List<StatusEffect>();

            TeamState = GameController.GetTeamState(TeamID);

            MapIcon = GetComponentInChildren<MapIcon>(true).gameObject;

            if (TeamID != 1) {

                MapIcon.SetActive(false);
            }

        }

        protected void Update() {

            if (IsRegistered == false) {
                Register();
            }
            

            if (ScanCountdown > 0) {

                ScanCountdown -= Time.deltaTime;
            }

            if (CurrentHealth <= 0) {

                Destroyed();
            }

            if (IsActive == true) {
                
                //Debug.Log($"IsActive check passed");

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
                        TargetMarker.SetActive(true);
                        TargetMarker.transform.position = TargetPoint;
                        TargetMarker.transform.rotation = Quaternion.identity;

                    }
                }
                
                if (IsPlayer == true) {
                    //PlayerInput = GetComponent<PlayerInput>();
                    //PlayerInput.ActivateInput();

                }
                else {
                    //PlayerInput = GetComponent<PlayerInput>();
                    //PlayerInput.DeactivateInput();
                }

                this.ProcessEffects();
                this.ProcessMovement();
            }
        }

        void LateUpdate() {

            
            if (IsActive == true) {
                this.ProcessTargeting();
            }
        }

        public override void CursorInteract() {

            ShowMenu();
        }

        public override void Selected() {

            
            //GameController.EnterUnit(this);

            if ((IsSelectable == true) && (IsActive == true)) {

                IsSelected = true;

                SelectionEffect.SetActive(true);
                TargetMarker.SetActive(true);
            }
            
        }

        
        public override void Deselected() {

            if (IsSelectable == true) {

                IsSelected = false;

                SelectionEffect.SetActive(false);
                TargetMarker.SetActive(false);


                //GameController.EnterUnit(this);
            }
        }

        protected virtual void ProcessMovement() {

        }

        protected virtual void ProcessTargeting() {

            // Debug.Log($"Scan countdown: " + ScanCountdown);

            // if ((AttackTarget == null) && (AutoTarget == null) && (ScanCountdown <= 0)) {
                
            //     ScanCountdown = ScanTime;

            //     List<GameItem> tempList = ScanTree.StartSearch(this.transform.position, Range);
            //     for (int i = 0; i < tempList.Count; i++) {
            //         GameItem item = tempList[i];


            //         Debug.Log($"Sarch returned item with Game ID: " + item.GameID);

            //         if ((item.TeamID != TeamID) && ((item is Turret) || (item is Unit))) {

            //             Debug.Log($"Setting unit target");

            //             AutoTarget = item.gameObject;
            //             break;
            //         }
                
            //     }

            // }
        }

        public void SetPlayer(bool active) {

            //IsPlayer = active;

            //Listener.enabled = active;
            //FirstPersonCamera.enabled = active;
            //PlayerInput.enabled = active;
        
            
        }

        // public Camera GetCamera() {

        //     return FirstPersonCamera;
        // }

        // public AudioListener GetListener() {

        //     return Listener;
        // }


        /// <summary>
        /// This method exposes the NavMeshAgent's Warp function to outside entities.
        /// </summary>
        public void Warp(Vector3 destination) {

            Agent.Warp(destination);
        }

        public override void SetTarget(GameItem target) {

            if (target != this) {

                Debug.Log($"Target object set in unit");
                if (target.TeamID != TeamID) {
                    AttackTarget = target.gameObject;
                    FollowTarget = null;
                }
                else {

                    FollowTarget = target;
                    AttackTarget = null;
                }

                TargetPoint = target.transform.position;
            
                UnitController.TargetAssigned(target);

                TargetMarker TargetMarkerScript = TargetMarker.GetComponent<TargetMarker>();
                TargetMarkerScript.SetTarget(target);
                TargetMarker.SetActive(true);
            }
        }

        public override void SetTarget(Vector3 target) {

            Path = new NavMeshPath();


            Agent.CalculatePath(target, Path);

            Debug.Log($"Is agent on mesh?: " + Agent.isOnNavMesh);
            TargetPoint = target;
            AttackTarget = null;
            UnitController.TargetAssigned(target);

            TargetMarker TargetMarkerScript = TargetMarker.GetComponent<TargetMarker>();
            TargetMarkerScript.SetTarget(target);
            TargetMarker.SetActive(true);

        }

        /// <summary>
        /// This method returns the next path point of the unit's nav agent.
        /// </summary>

        public Vector3 GetNextPoint() {

            // if (Agent.path == null) {

            //     return Agent.steeringTarget;
            // }
            // else {

            //     return Vector3.zero;
            // }

            return this.transform.position;
        }

        protected void OnRevertToOverhead(InputAction action) {

            GameController.EnterOverhead();
        }

        public void SetFormation(Formation formationIn) {

            if (Formation != null) {

                Formation.Remove(this);
            }
            Formation = formationIn;
        }

        public Formation GetFormation() {

            return Formation;
        }
    }
}