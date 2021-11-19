using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    /// <summary>
    /// This class helps dynamically generate textures for menu icons.
    /// </summary>

    public class PreviewRenderer : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(CameraObject))]
        private GameObject cameraObject;
        public GameObject CameraObject { get => cameraObject; set => cameraObject = value; }

        protected bool HasWritten = false;

        protected float Countdown = 1.0f;
        

        void LateUpdate() {
            

            if (Countdown <= 0) {

                if (HasWritten == false) {
                    
                    List<string> itemList = new List<string>();
                    itemList.Add("Drone");
                    itemList.Add("Tank");
                    itemList.Add("Artillery");
                    itemList.Add("Refinery");

                    for (int i = 0; i < itemList.Count; i++) {

                        SaveTexture(itemList[i]);
                    }
                    HasWritten = true;
                }
            }
            else {
                Countdown -= Time.deltaTime;
            }
        }

        protected void SaveTexture(string item) {

            Renderer renderer = GetComponent<Renderer>();
            Material material = renderer.material;

            RenderTexture rendTex = (RenderTexture)Resources.Load<RenderTexture>(item + "Render");
            //Camera camera = CameraObject.GetComponent<Camera>();
            //camera.Render();


            //RenderTexture rendTex = camera.GetComponent<Camera>().targetTexture;

            Debug.Log($"Writing texture to file: " + item);
            Texture2D texture2D = new Texture2D(512, 512, TextureFormat.RGB24, false);

            RenderTexture.active = rendTex;

            if (rendTex == null) {

                Debug.Log($"Rendtex null for some stupid reason");
            }

            texture2D.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0, false);

            texture2D.Apply();

            material.SetTexture("_MainTex", texture2D);


            byte[] bytes = texture2D.EncodeToPNG();

            File.WriteAllBytes((@"C:\Users\ikono\Documents\MaximumGame\Assets\Resources\" + item) + "Image.JPG", bytes);


            
        }
    }
}