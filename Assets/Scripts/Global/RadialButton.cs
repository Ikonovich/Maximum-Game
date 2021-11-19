using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {


    public class RadialButton : WorldButton {

        [Header("Texture")]
        [SerializeField, PropertyBackingField(nameof(Texture))]
        private Texture texture;
        public Texture Texture { get => texture; set => texture = value; }
        
        [SerializeField, PropertyBackingField(nameof(Offset))]
        private Vector3 offset;
        public Vector3 Offset { get => offset; set => offset = value; }
        

        protected float LerpSpeed = 1.0f;

        protected float ShowTime = 5.0f;

        protected float ShowCountdown = 0.0f;

        public bool IsLerping;
        
        protected bool IsClosing = false;

        protected Material Material;

        void Awake() {

            Renderer renderer = GetComponentInChildren<Renderer>();
            Material = renderer.material;

            Material.SetTexture("_MainTex", Texture);
        }


        void Update() {

            Vector3 newPosition = Vector3.zero;

            if (IsLerping == true) {

                newPosition = Vector3.Lerp(this.transform.localPosition, Offset, Time.deltaTime * LerpSpeed);
                this.transform.localPosition = newPosition;
            }
            else if (IsClosing) {

                Debug.Log($"Button position: " + transform.parent.transform.localPosition);
                if ((transform.parent.transform.localPosition).magnitude < 0.1f) {

                    Hide();
                    
                }

                newPosition = Vector3.Lerp(this.transform.localPosition, Vector3.zero, Time.deltaTime * LerpSpeed);
                this.transform.localPosition = newPosition;
            }

        }

        public void Lerp(Vector3 lerpVector, float lerpSpeedIn) {

            Debug.Log($"Setting up button lerp");
            Offset = lerpVector;
            LerpSpeed = lerpSpeedIn;
            IsLerping = true;
        }

        public virtual void Close(float lerpSpeedIn) {

            Debug.Log($"Setting up button close");
            Offset = this.transform.parent.transform.localPosition;
            LerpSpeed = lerpSpeedIn;
            IsLerping = false;
            IsClosing = true;
        }

        public virtual void Hide() {

            this.transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}