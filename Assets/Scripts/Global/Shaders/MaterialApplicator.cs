using System;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    public class MaterialApplicator : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(FromTexture))]
        private Texture fromTexture;
        public Texture FromTexture { get => fromTexture; set => fromTexture = value; }
        


        [SerializeField, PropertyBackingField(nameof(ToTexture))]
        private RenderTexture toTexture;
        public RenderTexture ToTexture { get => toTexture; set => toTexture = value; }
        

        [SerializeField, PropertyBackingField(nameof(Material))]
        private Material material;
        public Material Material { get => material; set => material = value; }


        protected void Update() {

        }

        public void ApplyMaterial(RenderTexture toTexture, Material material) {

            Graphics.Blit(FromTexture, toTexture, material);
        }

    }
}