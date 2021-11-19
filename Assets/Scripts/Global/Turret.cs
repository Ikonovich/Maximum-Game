using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {



    /// </summary>
    /// This class provides provides a foundation for static turrets.
    /// The building handles target marking, but the actual turret (or turrets) operates independently,
    /// controlled by a TurretCountroller, only using the building as the source of it's target. 
    /// </summary>

    public class Turret : Structure {

        [SerializeField, PropertyBackingField(nameof(Range))]
        private float range;
        public float Range { get => range; set => range = value; }

        protected float ScanTime = 0.1f;
        
        protected float ScanCountdown = 0.0f;

        protected ScanTree ScanTree;
        

        protected int ActiveLauncher = 0;
        

        void Awake() {

            Initialize();

        }

        void Start() {

            Register();
        }

        protected void Update() {

            if (ScanCountdown > 0) {
                ScanCountdown -= Time.deltaTime;
            }

            ProcessEffects();

        }

        protected override void Initialize() {
            

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            ScanTree = controllerObject.GetComponent<ScanTree>();

            TargetMarker = GetComponentInChildren<TargetMarker>(true).gameObject;


            Transform tempTransform = transform.Find("SelectionEffect");
            SelectionEffect = tempTransform.gameObject;

            StatusEffects = new List<StatusEffect>();

            MapIcon = GetComponentInChildren<MapIcon>(true).gameObject;
            if (TeamID != 1) {

                MapIcon.SetActive(false);
            }
            // Gathers the materials for setting transparency.
            
            List<Material> Materials = new List<Material>();

            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in renderers) {

                Materials.Add(renderer.material);

            }
            
        }

        
        public override void Selected() {

            Debug.Log($"Turret selected");


            if ((IsSelectable == true) && (IsActive == true)) {

                IsSelected = true;

                SelectionEffect.SetActive(true);

                // if ((AttackTarget != null) && (TargetPoint != Vector3.zero)) {
                 
                //     TargetMarker.SetActive(true);
                // }
            }

        }

        public override void Deselected() {

            if (IsSelectable == true) {

                IsSelected = false;

                SelectionEffect.SetActive(false);
                //TargetMarker.SetActive(false);


                //GameController.EnterUnit(this);
            }
        }


        public override void SetTarget(GameItem target) {

            Debug.Log($"Target object set in turret");
            AttackTarget = target.gameObject;


            TargetMarker TargetMarkerScript = TargetMarker.GetComponent<TargetMarker>();
            TargetMarkerScript.SetTarget(target);
            TargetMarker.SetActive(true);
        }

        public override void SetTarget(Vector3 target) {

            Debug.Log($"Target point set in turret: " + target.ToString());
            TargetPoint = target;


            TargetMarker TargetMarkerScript = TargetMarker.GetComponent<TargetMarker>();
            TargetMarkerScript.SetTarget(target);
            TargetMarker.SetActive(true);

        }


    }
}