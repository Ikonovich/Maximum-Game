using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class LaserEffect : FiringEffect {

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

        protected List<Tuple<LineRenderer, float>> FirePoints;

        protected int CurrentFirePoint = 0;

        protected LineRenderer Line;

        protected void Awake() {

            AudioSources = GetComponents<AudioSource>();

            FirePoints = new List<Tuple<LineRenderer, float>>();


            foreach (GameObject firePoint in FirePointObjects) {
                    
                LineRenderer line = firePoint.GetComponent<LineRenderer>();

                FirePoints.Add(new Tuple<LineRenderer,float>(line, 0.0f));
            
            }
        }

        protected void Update() {

            for (int i = 0; i < FirePoints.Count; i++) {

                if (FirePoints[i].Item2 > 0) {

                    FirePoints[i] = new Tuple<LineRenderer, float>(FirePoints[i].Item1, FirePoints[i].Item2 - Time.deltaTime);

                    if (FirePoints[i].Item2 <= 0) {
                            
                        Stop(FirePoints[i].Item1);

                        if (IsSoundPlayed == true) {

                            IsSoundPlayed = false;
                        }
                    }
                }
            }
        }


        public override void Play(Vector3 startPosition, Vector3 endPosition) {

            
            Debug.Log($"Laser effect playing");

            LineRenderer line = FirePoints[CurrentFirePoint].Item1;
            FirePoints[CurrentFirePoint] = new Tuple<LineRenderer, float>(line, PlayDuration);
            
            line.startWidth = StartDiameter;
            line.endWidth = EndDiameter;

            if (IsSoundPlayed == false) {

                AudioSources[CurrentFirePoint].Play();
                
                if (IsMultiLaser == false) {
                    IsSoundPlayed = true;
                }
            }


            Debug.Log($"Current fire point num: " + CurrentFirePoint);
            Debug.Log($"Num of fire points: " + FirePoints.Count);


            line.SetPosition(0, line.transform.position);
            line.SetPosition(1, endPosition);

            CurrentFirePoint += 1;

            if (CurrentFirePoint == (FirePoints.Count)) {
                CurrentFirePoint = 0;
            }
        }

        new public void Stop(LineRenderer line) {

            
            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, this.transform.position);
        }

    }
}