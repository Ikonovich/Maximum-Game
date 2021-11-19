using System;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    public class Weapon : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(LayerMask))]
        private LayerMask layerMask = 221;
        public LayerMask LayerMask { get => layerMask; set => layerMask = value; }
        

        [SerializeField, PropertyBackingField(nameof(AimConstraint))]
        private float aimConstraint = 1.0f;
        public float AimConstraint { get => aimConstraint; set => aimConstraint = value; }
        

        [SerializeField, PropertyBackingField(nameof(ParentObj))]
        private GameObject parentObj;
        public GameObject ParentObj { get => parentObj; set => parentObj = value; }
        

        [SerializeField, PropertyBackingField(nameof(TargetHelper))]
        private GameObject targetHelper;
        public GameObject TargetHelper { get => targetHelper; set => targetHelper = value; }
        

        [SerializeField, PropertyBackingField(nameof(FiringOffset))]
        private Vector3 firingOffset = Vector3.zero;
        public Vector3 FiringOffset { get => firingOffset; set => firingOffset = value; }

        [SerializeField, PropertyBackingField(nameof(IsActive))]
        private bool isActive = false;
        public bool IsActive { get => isActive; set => isActive = value; }

        
        [SerializeField, PropertyBackingField(nameof(Range))]
        private float range = 200;
        public float Range { get => range; set => range = value; }

        [SerializeField, PropertyBackingField(nameof(ReloadTime))]
        private float reloadTime = 5.0f;
        public float ReloadTime { get => reloadTime; set => reloadTime = value; }
        
        protected float ReloadCountdown = 0.0f;

        protected FiringEffect FiringEffect;

        [SerializeField, PropertyBackingField(nameof(Barrel))]
        private GameObject barrel;
        public GameObject Barrel { get => barrel; set => barrel = value; }
        


        void Awake() {

            FiringEffect = GetComponent<FiringEffect>();
            
        }

        
        void Update() {

            if (ReloadCountdown > 0) { 
                ReloadCountdown -= Time.deltaTime;
            }
        }

        public virtual void Fire(GameItem target) {

        }
        
        public virtual Vector2 GetFiringVector(Vector3 targetPoint) {

            return Vector2.zero;
        }

        public virtual bool OnTarget(Vector3 targetPoint) {

            return false;
        }

        public virtual bool OnTarget(GameItem item) {

            return false;
        }

        public virtual bool IsSafe(int teamID) {

            return true;
        }

    }

}