using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace MaxGame {

    public class MapAnimator : MonoBehaviour {

        protected Texture2D Texture;

        protected GameController GameController;

        protected Material Material;

        protected Color[] ColorMap;

        protected Color[] StoredColorMap;

        protected int Xsize = 401;
        protected int Zsize = 402;

        protected int TerrainHeight = 2000;

        protected int TerrainWidth = 2000;

        protected GameObject Player;

        protected Mesh Mesh;

        protected float UpdateCountdown = 0.000f;
        protected float UpdateTime = 0.100f;

        protected List<MapItem> MapList;

        protected Dictionary<int, Color> TeamColors;



        protected void Awake() {

            TeamColors = new Dictionary<int, Color>();
            TeamColors.Add(0, Color.grey);
            TeamColors.Add(1, Color.blue);
            TeamColors.Add(2, Color.red);
            TeamColors.Add(3, Color.yellow);
            TeamColors.Add(4, Color.green);



            Texture redTex = (Texture)Resources.Load("RedBlock");
            Texture blueTex = (Texture)Resources.Load("BlueBlock");
            Texture blackTex = (Texture)Resources.Load("BlackBlock");


            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();


            //Renderer renderer = GetComponent<Renderer>();
            //Material = renderer.material;
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            Mesh = meshFilter.mesh;

            StoredColorMap = new Color[(Xsize * Zsize)];
            for (int i = 0; i < StoredColorMap.Length; i++) StoredColorMap[i] = Color.clear;
            

            //Mesh.SetColors(StoredColorMap);



            //Test items

            // List<MapItem> MapList = new List<MapItem>();
            // MapItem itemOne = new MapItem(10, 10, Color.white, 3);
            // MapItem itemTwo = new MapItem(30, 30, Color.grey, 5);
            // MapItem itemThree = new MapItem(90, 90, Color.blue, 7);
            // MapItem itemFour = new MapItem(60, 60, Color.red, 10);

        }

        protected void Update() {

            
            if (UpdateCountdown <= 0) {

                UpdateCountdown = UpdateTime;
                GenerateMap();
            }
            else {
                UpdateCountdown -= Time.deltaTime;
            }
        }

        protected void GenerateList() {

            MapList = new List<MapItem>();

            Dictionary<int, GameItem> itemDict = GameController.GetWorldItemDict();

            int i = 0;
            foreach (KeyValuePair<int, GameItem> kvp in itemDict) {

                GameItem item = kvp.Value;
                GameObject itemObject = item.gameObject;

                int xPos = (int)(Math.Round(itemObject.transform.position.x) / (TerrainWidth / Xsize));
                int zPos = (int)(Math.Round(itemObject.transform.position.z) / (TerrainHeight / Zsize));


                MapItem mapItem = new MapItem(xPos, zPos, TeamColors[item.TeamID], item.MapSize, item.GetID());


                Debug.Log($"Map item created. Item Num: " + i + " X Position: " + xPos + " Z Position: " + zPos);
                MapList.Add(mapItem);
                i++;
            }
        }

        protected void GenerateMap() {

            GenerateList();

            ColorMap = new Color[(Xsize * Zsize)];
            Array.Copy(StoredColorMap, ColorMap, ColorMap.Length);

            for (int i = 0; i < MapList.Count; i++) {

                MapItem item = MapList[i];
                Color[] lineArray = new Color[(item.Size * 2) + 1];


                for (int j = 0; j < lineArray.Length; j++) lineArray[j] = item.Color;


                Debug.Log($"Color for item " + item.GameID + " has been set as " + lineArray[0].ToString());

                for (int k = 0; k < lineArray.Length; k++) {

                    int index =  ((item.Xposition - 1 + (k - item.Size)) * Zsize) + (item.Zposition - item.Size);

                    Array.Copy(lineArray, 0, ColorMap, Mathf.Clamp(index - 5, 0, ColorMap.Length - lineArray.Length - 1), lineArray.Length);

                    Debug.Log($"Drawing map dude num " + i + " line num " + k + " Color is " + lineArray[0].ToString()); 
                }
            }

            WriteMap(ColorMap);
        }

        protected void WriteMap(Color[] map) {

            //StoredColorMap = new Color[(Xsize * Zsize)];
            //for (int i = 0; i < StoredColorMap.Length; i++) StoredColorMap[i] = Color.black;
            
            
            //GameObject.DestroyImmediate(Texture);
          

            Mesh.SetColors(ColorMap);
            Debug.Log("Color map size: " + ColorMap.Length + " Map mesh size: " + Mesh.vertices.Length);

            //Material.SetTexture("_Overlay",Texture);
           

        }




    }

    public class MapItem {

        public int Xposition;

        public int Zposition;

        public Color Color;

        public int Size;

        public int GameID;

        public MapItem(int xIn, int zIn, Color colorIn, int sizeIn, int idIn) {

            Xposition = xIn;
            Zposition = zIn;
            Color = colorIn;
            Size = sizeIn;
            GameID = idIn;
        }

    }


}

