using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {


    /// <summary>
    /// This class acts as an overarching controller for AI-controlled
    /// teams.
    /// It has the following responsibilities:
    /// - Utilizing pre-existing TeamState information to 
    /// keep track of available resources, units, and known enemy
    /// units.
    /// - Keeping a running tally of known enemy strength.
    /// - At the start of the game, generating a grid which it uses 
    /// to form a heatmap of allied and enemy strengths and resource   
    /// locations.
    /// Doling out priority and resource availability to the AI 
    /// subroutines based on the above knowledge.
    /// </summary>


    public class AITeamController : MonoBehaviour {

        protected AIBuildController BuildController;
        protected AIScoutController ScoutController;
        public int TeamID = 0;
        protected TeamState TeamState;
        protected GameController GameController;

        protected Dictionary<Vector2Int, Vector3Int> GridRefToCoords;
        protected Dictionary<Vector2Int, List<GameItem>> GridToItems;
        protected Dictionary<GameItem, Vector2Int> ItemToGrid;

        protected Dictionary<Vector2Int, GameItem> GridToResources;

        protected List<GameItem> TeamUnits;

        protected List<Building> ProductionBuildings;

        /// <summary>
        /// This stores a heatmap of team strength for each 
        /// grid reference. The dictionary key represents the
        /// grid reference, the Vec2 value represents the strength
        /// of the allies in the that coordinate in the X axis
        /// and the strength of the enemies in that coordinate in 
        /// the Y axis.
        /// </summary>
        protected Dictionary<Vector2Int, Vector3Int> HeatMap;

        protected int MapSizeX = 2000;

        protected int MapSizeZ = 2000;

        protected int Resolution = 50;

        public void Awake() {

            Initialize();
        }

        protected void Initialize() {

            
            GenerateGrid();


            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            TeamState = GameController.GetTeamState(TeamID);
            TeamState.SetAI(this);

            TeamUnits = new List<GameItem>();
            ProductionBuildings = new List<Building>();

            UpdateTeamItems();

            ScoutController = GetComponent<AIScoutController>();
            BuildController = GetComponent<AIBuildController>();

            ScoutController.ExternalInitialize(TeamID, TeamState, this);
            BuildController.ExternalInitialize(TeamID, TeamState, this);

        }

        /// <summary>
        /// This class generates the grid used to track all of the
        /// game items known to the AI. The grid is represented by two
        /// dictionaries, one mapping units to grid coordinates, 
        /// one mapping grid coordinates to lists of units. 
        /// </summary>
        protected void GenerateGrid() {

            GridRefToCoords = new Dictionary<Vector2Int, Vector3Int>();
            GridToItems = new Dictionary<Vector2Int, List<GameItem>>();
            ItemToGrid = new Dictionary<GameItem, Vector2Int>();
            HeatMap = new Dictionary<Vector2Int, Vector3Int>();


            int i = 0;
            for (int x = 0; x <= MapSizeX; x += Resolution) {
                
                int j = 0;
                for (int z = 0; z <= MapSizeZ; z += Resolution) {

                    List<GameItem> tempList = new List<GameItem>();

                    Vector3Int gridCoords = new Vector3Int(x, 0, z);
                    Vector2Int gridReference = new Vector2Int(i, j);

                    
                    GridRefToCoords.Add(gridReference, gridCoords);
                    GridToItems.Add(gridReference, tempList);
                    HeatMap.Add(gridReference, Vector3Int.zero);

                    Debug.Log($"Grid added at location: {gridCoords} with reference {gridReference}");

                    j++;
                }
                i++;
            }
        }

        public void AddProductionBuilding(Building building) {

            ProductionBuildings.Add(building);

            Debug.Log($"Add production building called: {building.Name}");

        }

        public void RemoveProductionBuilding(Building building) {

            ProductionBuildings.Remove(building);
        }

        public List<Building> GetProductionBuildings() {

            return ProductionBuildings;
        }

        protected void BuildItem(string ItemName, Vector3 closestPosition, int priority, string requestingController) {

            BuildController.Add(ItemName, closestPosition, priority, requestingController);
        }

        protected void UpdateTeamItems() {

            TeamUnits = TeamState.GetUnitList();
            UpdateGrid(TeamUnits);
        }

        protected void UpdateDetectedItems() {

            UpdateGrid(TeamState.GetDetected());
        }

        protected void UpdateGrid(List<GameItem> itemList) {

            for (int i = 0; i < itemList.Count; i++) {

                GameItem item = itemList[i];
                Vector2Int currentGrid = GetGridReference(item);

                if (ItemToGrid.ContainsKey(item)) {

                    Vector2Int oldGrid = ItemToGrid[item];
                    ItemToGrid[item] = currentGrid;
                    GridToItems[oldGrid].Remove(item);
                    GridToItems[currentGrid].Add(item);

                    HeatMap[oldGrid] = HeatMap[oldGrid] - GetHeatValue(item);
                    HeatMap[currentGrid] = HeatMap[oldGrid] + GetHeatValue(item);
                    

                }
                else {

                    ItemToGrid.Add(item, currentGrid);
                    GridToItems[currentGrid].Add(item);
                    HeatMap[currentGrid] = HeatMap[currentGrid] + GetHeatValue(item);
                }
            }
        }

        /// <summary>
        /// The following two numbers return a grid reference number based on a provided
        /// item's position or provided vector.
        /// </summary>

        protected Vector2Int GetGridReference(GameItem item) {

            int xPos = (int)item.transform.position.x;
            int zPos = (int)item.transform.position.z;

            Vector2Int grid = new Vector2Int(xPos / Resolution, zPos / Resolution);

            return grid;
        }

        public Vector2Int GetGridReference(Vector3 position) {

            int xPos = (int)position.x;
            int zPos = (int)position.z;

            Vector2Int grid = new Vector2Int(xPos / Resolution, zPos / Resolution);

            Debug.Log($"GridRef: {grid} BasedOn {position}.");
            return grid;
        }

        public Vector3Int GetGridCoords(Vector2Int gridReference) {

            return GridRefToCoords[gridReference];
        }


        
        /// <summary>
        /// This method returns a Vector3Int containing the
        /// heat value of the item submitted.
        /// Allied item values are returned in the X position,
        /// world items are returned in the Y position,
        /// and enemies are returned in the Z position. 
        /// </summary>

        protected Vector3Int GetHeatValue(GameItem item) {

            if (item.TeamID == 0) {
                return new Vector3Int(0, 1, 0);
            }
            else if (item.TeamID == TeamID) {
                return new Vector3Int(1, 0, 0);
            }
            else {
                return new Vector3Int(1, 0, 0); 
            }
        }

        /// <summary>
        /// This method returns the team headquarters that is closest to
        /// the provided position.
        /// </summary>

        public Building GetHeadquarters(Vector3 position) {

            Debug.Log($"Getting headquarters. Production buildings count: {ProductionBuildings.Count}");

            float distance = 0;
            Building headquarters = null;
            for (int i = 0; i < ProductionBuildings.Count; i++) {
                
                Building tempBuilding = ProductionBuildings[i];
                if (tempBuilding.Name == "Headquarters") {
                    
                    float tempDistance = (tempBuilding.transform.position - position).magnitude;

                    if (headquarters != null)  {
                        if (tempDistance < distance) {
                            headquarters = tempBuilding;
                            distance = tempDistance;
                        }
                    }
                    else {
                        headquarters = tempBuilding;
                        distance = tempDistance;
                    }
                }

            }
            return headquarters;

        }



    }

}