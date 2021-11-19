using System;
using UnityEngine;


namespace MaxGame {

    public class CameraToTexture : MonoBehaviour {

        protected bool HasDrawn = false;

        void Update() {

            if (HasDrawn == false) {

                var Cameras = GetComponentsInChildren<Camera>();

                int i = 0;
                foreach (Camera camera in Cameras) {

                    RenderTexture rendTex = new RenderTexture(256, 256, 16, RenderTextureFormat.Default,  RenderTextureReadWrite.Default);

                    Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);

                    camera.targetTexture = rendTex;

                    
                    RenderTexture.active = rendTex;

                    if (rendTex == null) {

                        Debug.Log($"Rendtex null for some stupid reason");
                    }

                    camera.Render();

                    texture.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0, false);

                    texture.Apply();

                    byte[] bytes;

                    bytes = texture.EncodeToPNG();

                    System.IO.File.WriteAllBytes(@"C:\Users\ikono\Documents\MaximumGame\Assets\Icons\GameItemImage" + i + ".JPG", bytes);

                    Debug.Log($"Writing texture " + i);
                    i++;

                }

                HasDrawn = true;
            }

        }
    }
}