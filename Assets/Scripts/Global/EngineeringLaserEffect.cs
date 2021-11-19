using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class EngineeringLaserEffect : FiringEffect {

        [SerializeField, PropertyBackingField(nameof(FirePointObjects))]
        private List<GameObject> firePointObjects;
        public List<GameObject> FirePointObjects { get => firePointObjects; set => firePointObjects = value; }
        
        
        [SerializeField, PropertyBackingField(nameof(StartDiameter))]
        private float startDiameter = 0.2f;
        public float StartDiameter { get => startDiameter; set => startDiameter = value; }

        [SerializeField, PropertyBackingField(nameof(EndDiameter))]
        private float endDiameter = 0.2f;
        public float EndDiameter { get => endDiameter; set => endDiameter = value; }
        

        [SerializeField, PropertyBackingField(nameof(Material))]
        private Material material;
        public Material Material { get => material; set => material = value; }

        [SerializeField, PropertyBackingField(nameof(IsMultiLaser))]
        private bool isMultiLaser;
        public bool IsMultiLaser { get => isMultiLaser; set => isMultiLaser = value; }

        protected AudioSource[] AudioSources;

        protected List<LineRenderer> FirePoints;

        protected List<float> FireTimers;

        protected int CurrentFirePoint = 0;

        protected LineRenderer Line;

        protected Collider TargetCollider;

        protected bool IsPlaying = false;

        protected void Awake() {

            AudioSources = GetComponents<AudioSource>();

            FirePoints = new List<LineRenderer>();
            FireTimers = new List<float>();

            foreach (GameObject firePoint in FirePointObjects) {
                    
                LineRenderer line = firePoint.GetComponent<LineRenderer>();

                FirePoints.Add(line);
                FireTimers.Add(0f);
            
            }
        }

        protected void Update() {
            
            
            if (IsPlaying) {

                LineRenderer line0 = FirePoints[0];

                float xShift = Mathf.Cos(Time.fixedTime);
                float yShift = Mathf.Sin(Time.fixedTime);

                Vector3 endPosition = TargetCollider.ClosestPoint(line0.transform.position); 

                endPosition = TargetCollider.ClosestPoint(endPosition + new Vector3(xShift * TargetCollider.bounds.extents.x, yShift * TargetCollider.bounds.extents.y, 0f)); 

                line0.startWidth = StartDiameter;
                line0.endWidth = EndDiameter;

                line0.SetPosition(0, line0.transform.position);
                line0.SetPosition(1, endPosition);

                LineRenderer line1 = FirePoints[1];

                xShift = -Mathf.Cos(Time.fixedTime);
                yShift = -Mathf.Sin(Time.fixedTime);

                endPosition = TargetCollider.ClosestPoint(line1.transform.position); 
                endPosition = TargetCollider.ClosestPoint(endPosition + new Vector3(xShift * TargetCollider.bounds.extents.x, yShift * TargetCollider.bounds.extents.y, 0f)); 

                line1.startWidth = StartDiameter;
                line1.endWidth = EndDiameter;

                line1.SetPosition(0, line1.transform.position);
                line1.SetPosition(1, endPosition);
            }
                
        }

        public override void Play(GameObject firePoint)
        {
            if (IsPlaying == false) {
                TargetCollider = firePoint.GetComponent<Collider>();

                IsPlaying = true;
            }

        }
        public override void Play(Vector3 startPosition, Vector3 endPosition) {

            
            LineRenderer line0 = FirePoints[0];
            LineRenderer line1 = FirePoints[1];

            
            line0.startWidth = StartDiameter;
            line0.endWidth = EndDiameter;

            line1.startWidth = StartDiameter;
            line1.endWidth = EndDiameter;

            if (IsSoundPlayed == false) {

                AudioSources[CurrentFirePoint].Play();
                
                if (IsMultiLaser == false) {
                    IsSoundPlayed = true;
                }
            }


            line0.SetPosition(0, line0.transform.position);
            line1.SetPosition(1, line1.transform.position);


        }

        new public void Stop(LineRenderer line) {

            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, this.transform.position);

            IsPlaying = false;

        }

        public override void Stop() {


            foreach (LineRenderer line in FirePoints) {

                line.SetPosition(0, this.transform.position);
                line.SetPosition(1, this.transform.position);
            }

            IsPlaying = false;
        }

    }
}