using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {


    /// <summary>
    /// This class handles building all game items for all other AI controllers.
    /// It maintains a set of priority queues, each consisting of a series of QueueItems.
    /// A QueueItem contains a reference to the type of item to be built, the position
    /// which it should be built as close to as possible, the name of the controller
    /// that requested it's construction, and how long it has been in the queue.
    /// A QueueList contains a list of queue items, a priority, and the cost of building
    /// all items in the queue combined.
    ///
    /// This class is controlled and initialized by the AITeamController.
    /// </summary>


    public class AIBuildController : MonoBehaviour {

        protected int TeamID;
        protected TeamState TeamState;

        protected AITeamController TeamController;

        protected CollisionTester CollisionTester;

        /// <summary>
        /// This dictionary maps units to constructable buildings
        /// that can create them, for use when a unit has been 
        /// requested that does not have a building that can create
        /// it. Used by AddMissingBuilding.
        /// </summary>
        protected Dictionary<string, string> UnitToBuildingDict;

        protected float MaxWaitTime = 500f;
        protected QueueList QueueHigh;
        protected QueueList QueueMedium;
        protected QueueList QueueLow;

        protected bool HasStarted = false;

        
        /// <summary>
        /// This method is called by the Team Controller during Awake.
        /// </summary>

        public void ExternalInitialize(int teamID, TeamState teamState, AITeamController teamController) {


            TeamID = teamID;
            TeamState = teamState;
            TeamController = teamController;

            GameObject collisionTesterObject = GameObject.Find("/GameController/CollisionTester");
            CollisionTester = collisionTesterObject.GetComponent<CollisionTester>();

            QueueHigh = new QueueList(1);
            QueueMedium = new QueueList(2);
            QueueLow = new QueueList(3);

            MapUnitsToBuildings();

        }

        protected void MapUnitsToBuildings() {

            UnitToBuildingDict = new Dictionary<string, string>();
            UnitToBuildingDict.Add("SupportWalker", "Refinery");
            UnitToBuildingDict.Add("CannonWalker", "MechFab");
            UnitToBuildingDict.Add("LaserWalker", "MechFab");
            UnitToBuildingDict.Add("MissileWalker", "MechFab");
            UnitToBuildingDict.Add("HeavyWalker", "MechFab");
            UnitToBuildingDict.Add("ATV", "VehicleFab");

        }

        void Update() {

            if (HasStarted == false) {

                BuildBuilding("Refinery", new Vector3(100f, 10f, 100f));
                HasStarted = true;
            }

    
            if (QueueHigh.Count > 0) {
                GameItem topItem = QueueHigh.Queue[0].Item;
                if (TeamState.Purchase(topItem.GetCost()) == true) {

                    Build(QueueHigh.Queue[0]);
                    QueueHigh.Dequeue();
                }

                UpdateQueue(QueueMedium);
                UpdateQueue(QueueLow);
            }

        }

        /// <summary>
        /// This method updates the provided queue, updating the wait time of each
        /// queued item, moving the items to a higher queue when they pass the wait
        /// threshold.
        /// </summary>

        protected void UpdateQueue(QueueList queue) {

            List<QueueItem> queueList = new List<QueueItem>(queue.Queue);

            for (int i = 0; i < queueList.Count; i++) {
                QueueItem item = queueList[i];
                item.WaitTime += Time.deltaTime;

                if (item.WaitTime > MaxWaitTime) {

                    if (queue.Priority == 3) {
                        QueueMedium.Enqueue(item);
                    }
                    else if (queue.Priority == 2) {
                        QueueHigh.Enqueue(item);
                    }
                    queue.Dequeue(item);
                }
            }
        }

        public void Add(string itemID, Vector3 closestPosition, int priority, string requestingController) {

            GameObject itemObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);
            GameItem item = itemObject.GetComponent<GameItem>();

            if (priority == 1) {
                QueueHigh.Enqueue(item, itemID, closestPosition, requestingController);
            }
            else if (priority == 2) {
                QueueLow.Enqueue(item, itemID, closestPosition, requestingController);
            }
            else if (priority == 3) {
                QueueLow.Enqueue(item, itemID, closestPosition, requestingController);
            }
            else {
                throw new ArgumentOutOfRangeException("Priority is out of bounds.");
            }
        }

        
        /// <summary>
        /// This method handles building whatever item is provided.
        /// If the item is a unit, it finds the closest building to the provided
        /// coordinates and sends it to be built. If the item is a 
        /// building, it finds an empty grid coordinate close to the provided coordinates
        /// and places it there.
        /// </summary>

        protected void Build(QueueItem item) {

            if (item.Item is Unit unit) {

               BuildUnit(item.ItemID, item.Position);

            }
            else if (item.Item is Building building) {
                BuildBuilding(item.ItemID, item.Position);
            }

        }

        protected void BuildBuilding(string itemID, Vector3 position) {

            Building headquarters = TeamController.GetHeadquarters(position);

            if (headquarters == null) {

                Debug.Log("Headquarters is null");
            }

            Vector3 placement = FindEmptyGrid(headquarters.transform.position);

            GameObject placeholderObject = Instantiate(GameObject.Find("/GameController/Placeholder"));

            placeholderObject.transform.position = placement;

        }

        protected void BuildUnit(string itemID, Vector3 position) {

            List<Building> buildings = TeamController.GetProductionBuildings();

            float currentDistance = 0;
            Building currentBuilding = null;

            for (int i = 0; i < buildings.Count; i++) {
                Building tempBuilding = buildings[i];

                if (tempBuilding.GetBuildable().Contains(itemID)) {

                    float tempDistance = (tempBuilding.transform.position - position).magnitude;

                    if (currentBuilding != null) {
                        if (tempDistance < currentDistance) {
                            currentBuilding = tempBuilding;
                            currentDistance = tempDistance;
                        }

                    }
                    else {
                        
                        currentBuilding = tempBuilding;
                        currentDistance = tempDistance;
                    }
                }
            }

            if (currentBuilding == null) {

                AddMissingBuilding(itemID, position);
                Add(itemID, position, 2, "BuildController");
            }
            else {

                currentBuilding.BuildItem(itemID);
            }
        }

        protected void AddMissingBuilding(string unitID, Vector3 position) {
            
            string buildingType = UnitToBuildingDict[unitID];
            
            Add(buildingType, position, 1, "BuildController");
        }

        protected Vector3 FindEmptyGrid(Vector3 position) {

            Vector3 coreVector = position;

            Vector2Int gridRef = TeamController.GetGridReference(position);
            Vector3Int gridCoords = TeamController.GetGridCoords(gridRef);

            float xDirection = 1;
            float zDirection = 0;

            int rotateNumber = 8;
            int rotateCount = 0;

            Vector3 offset = new Vector3(50f, 0f, 0f);
            int offsetMultiplier = 1;



            while (CheckGridIsEmpty(gridCoords) == false) {
                
                xDirection = offset.x * offsetMultiplier;
                zDirection = offset.z * offsetMultiplier;

                float rotateFactor = (360/rotateNumber) * rotateCount;

                float xFloat = xDirection * Mathf.Cos(rotateFactor * Mathf.PI/180f) - zDirection * Mathf.Sin(rotateFactor * Mathf.PI/180f);
                float zFloat = xDirection * Mathf.Sin(rotateFactor * Mathf.PI/180f) + zDirection * Mathf.Cos(rotateFactor * Mathf.PI/180f);

                Vector3 rotation = new Vector3(xFloat, 0, zFloat);

                gridRef = TeamController.GetGridReference(rotation + coreVector);
                gridCoords = TeamController.GetGridCoords(gridRef);

                Debug.Log($"Rotated: {rotation} Factor: {rotateFactor} Final: {rotation + coreVector} Grid: {gridRef} Coords: {gridCoords}");
            
                rotateCount += 1;

                if (rotateCount == rotateNumber) {
                    rotateCount = 0;
                    rotateCount += 8;
                    offsetMultiplier += 1;
                }

            }
            return gridCoords;
        }

        protected bool CheckGridIsEmpty(Vector3Int gridCoords) {

            CollisionTester.transform.position = gridCoords;

            if (CollisionTester.CollisionCount == 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public struct QueueList {

        public int Priority;
        public int Count;
        public Dictionary<ResourceType, int> TotalCostDict;
        public List<QueueItem> Queue;
        

        public QueueList(int priority) {
            Queue = new List<QueueItem>();
            TotalCostDict = new Dictionary<ResourceType, int>();

            TotalCostDict.Add(ResourceType.Energy, 0);
            TotalCostDict.Add(ResourceType.Elements, 0);
            TotalCostDict.Add(ResourceType.Carbon, 0);

            Priority = priority;
            Count = 0;
        }

        public void Enqueue(GameItem gameItem, string itemID, Vector3 position, string requestingController) {
            
            Dictionary<ResourceType, int> itemCost = gameItem.GetCost();
            AddCost(itemCost);

            QueueItem item = new QueueItem(gameItem, itemID, position, requestingController);
            Queue.Add(item);
            Count++;
        }

        public void Enqueue(QueueItem item) {

            Dictionary<ResourceType, int> itemCost = item.Item.GetCost();
            AddCost(itemCost);

            item.WaitTime = 0f;
            Queue.Add(item);
            Count++;
        }

        public void Dequeue() {

            Dictionary<ResourceType, int> itemCost = Queue[0].Item.GetCost();
            RemoveCost(itemCost);

            Queue.RemoveAt(0);
            Count--;
        }
        public void Dequeue(QueueItem item) {

            Dictionary<ResourceType, int> itemCost = item.Item.GetCost();
            RemoveCost(itemCost);

            Queue.Remove(item);
            Count--;
        }

        private void AddCost(Dictionary<ResourceType, int> cost) {

            TotalCostDict[ResourceType.Energy] = TotalCostDict[ResourceType.Energy] + cost[ResourceType.Energy];
            TotalCostDict[ResourceType.Carbon] = TotalCostDict[ResourceType.Carbon] + cost[ResourceType.Carbon];
            TotalCostDict[ResourceType.Elements] = TotalCostDict[ResourceType.Elements] + cost[ResourceType.Elements];

        }

        private void RemoveCost(Dictionary<ResourceType, int> cost) {

            TotalCostDict[ResourceType.Energy] = TotalCostDict[ResourceType.Energy] - cost[ResourceType.Energy];
            TotalCostDict[ResourceType.Carbon] = TotalCostDict[ResourceType.Carbon] - cost[ResourceType.Carbon];
            TotalCostDict[ResourceType.Elements] = TotalCostDict[ResourceType.Elements] - cost[ResourceType.Elements];

        }

    }

    public struct QueueItem {

        public GameItem Item;
        public string ItemID; 
        public Vector3 Position;
        public float WaitTime;
        
        public string RequestingController;

        public QueueItem(GameItem item, string itemID, Vector3 position, string requestingController) {

            Item = item;
            ItemID = itemID;
            Position = position;
            WaitTime = 0f;
            RequestingController = requestingController;
        }
    }

}

