using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {

    /// <summary>
    ///  This class generates a visible grid square around every whole coordinate segment on the map.
    ///
    /// </summary>




    public class TerrainGrid : MonoBehaviour {

        protected Vector3[] GridCoords;

        protected GameObject GridHolder;

        public int GridDivisor = 25;

        protected GameObject[] Grid;

        protected TerrainData TerrainData;

        GameController GameController;


        protected void Awake() {

            GameObject controllerObj = GameObject.Find("/GameController");
            GameController = controllerObj.GetComponent<GameController>();

            
            GridHolder = GameObject.Find("/GameController/Terrain/Grid");
            GameObject gridPrototype = GameObject.Find("/GameController/Terrain/Grid/GridPrototype");
            
            GameObject gridPrototypeLong = GameObject.Find("/GameController/Terrain/Grid/GridPrototypeLong");


            GameObject terrainObj = GameObject.Find("/GameController/Terrain");
            Terrain terrain = terrainObj.GetComponent<Terrain>();
            TerrainData = terrain.terrainData;

            int xSize = (int)Mathf.Round(TerrainData.size.x);
            int zSize = (int)Mathf.Round(TerrainData.size.z);

            int gridSizeX = xSize / GridDivisor;
            int gridSizeZ = zSize / GridDivisor;

            int gridSize = gridSizeX * gridSizeZ;

            GridCoords = new Vector3[gridSize];

            Grid = new GameObject[gridSize];

            Debug.Log($"Terrain Alpha Map is named: " + TerrainData.AlphamapTextureName);

            // Texture2D texture = (Texture2D)Resources.Load("blankTerrain");
            // SplatPrototype[] splatArray = new SplatPrototype[1];
            // SplatPrototype terrainTexture = new SplatPrototype();
            // terrainTexture.texture = texture;
            // splatArray[0] = terrainTexture;
            // TerrainData.splatPrototypes = splatArray;



            for (int x = 0; x < gridSizeX; x++) {

                for (int z = 0; z < gridSizeZ; z++) {

                    GameObject newGridCoord = Instantiate(gridPrototype);
                    GameObject newGridCoordLong = Instantiate(gridPrototypeLong);


                    newGridCoord.transform.parent = GridHolder.transform;
                    newGridCoordLong.transform.parent = GridHolder.transform;


                    newGridCoord.transform.position = new Vector3(x * GridDivisor, 500, z * GridDivisor);
                    newGridCoordLong.transform.position = new Vector3(x * GridDivisor, 500, z * GridDivisor);

                    
                }

            }
        }










    }
}