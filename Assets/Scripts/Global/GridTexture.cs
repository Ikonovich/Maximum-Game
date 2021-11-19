using System;
using UnityEngine;
using UnityEditor;

namespace MaxGame {

    public class GridTexture : MonoBehaviour {

        protected Texture2D Texture;
    

        protected void Start() {

            int xSize = 100;
            int ySize = 100;
            int barWidth = 3;

            Texture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);

            //Texture.SetPixel(0, 0, Color.black);
            //Texture.SetPixel(1, 0, Color.black);
            //Texture.SetPixel(0, 1, Color.black);
            //Texture.SetPixel(1, 1, Color.black);

            Color red = new Color(1f, 0f, 0f, 1f);

            Color[] xGridBar = new Color[(barWidth * ySize)];
            Color[] textureColors = new Color[(xSize * ySize)];

                for (int i = 0; i < xGridBar.Length; i++) {

                    xGridBar[i] = red;
                }
            
                  
                //Debug.Log($"Grid map index = " + (x * y) + " . Array size: " + colors.Length);

                for (int x = 0; x < xSize; x += (barWidth * 5)) { 
        
                    Array.Copy(xGridBar, 0, textureColors, x * ySize, xGridBar.Length);

                }
            Texture.SetPixels(textureColors, 0);

            Texture.Apply();

            Renderer renderer = GetComponent<Renderer>();
            Material material = renderer.material;

            material.mainTexture = Texture;




        }








            // for (int x = 1; x < xSize; x++) {


            //     for (int y = 0; y < ySize; y++) {
                    
            //         Debug.Log($"Grid map index = " + (x * y) + " . Array size: " + colors.Length);
                    
            //         Texture.SetPixel(x, y, color);
            //     }

            // }
                
            //AssetDatabase.CreateAsset(Texture, "Assets/NewTexture.png");

            ///Debug.Log("Asset Created at: " + AssetDatabase.GetAssetPath(Texture));
            


        protected void WriteTexture(Texture2D texture, string item) {


            Debug.Log($"Writing texture to file: " + item);


            
        }

    }
}