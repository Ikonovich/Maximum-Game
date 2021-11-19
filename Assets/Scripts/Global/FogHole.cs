using System;
using UnityEngine;
using Candlelight;
using VolumetricFogAndMist;


namespace MaxGame {


    public class FogHole : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(Radius))]
        private float radius = 5.0f;
        public float Radius { get => radius; set => radius = value; }
        
        [SerializeField, PropertyBackingField(nameof(UpdateDistance))]
        private float updateDistance = 1.0f;
        public float UpdateDistance { get => updateDistance; set => updateDistance = value; }
        
        Vector3 LastPosition = Vector3.zero;

        void Update() {

            Vector3 position = this.transform.position;

            if ((LastPosition - position).magnitude > UpdateDistance) {

                LastPosition = position;
                VolumetricFog fog = VolumetricFog.instance;

                if (fog != null) {
                    Debug.Log($"Setting hole in fog");
                    fog.SetFogOfWarAlpha(position, radius, 0);
                }
            }

        }
    }
}