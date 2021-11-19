using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {
    
// This class is used by the game controller to keep track of teams. 
// It stores the team ID, current resources available to the team, and a list of all units on the team.

    public class TeamState {

        public int TeamID { get; set; }

        protected AITeamController TeamController;

        public List<GameItem> UnitList;

        public List<Building> ProductionBuildings;
            

        // Stores the amount of each resource the team has, referenced by the resourcetype.
        // Used by a variety of classes to get the amount of resources available.

        public Dictionary<ResourceType, int> ResourceDict;

        // Stores a list of buildings that this team can construct.
        // Accessed and used by the HUD.

        public List<int> AvailableBuildings;

        protected float DetectionTime = 1.5f;

        public Dictionary<GameItem, float> DetectedItems;

        public void CreateTeam(int teamID) {

            ResourceDict = new Dictionary<ResourceType, int>();
            DetectedItems = new Dictionary<GameItem, float>();

            UnitList = new List<GameItem>();

            ResourceDict.Add(ResourceType.Energy, 1500);
            ResourceDict.Add(ResourceType.Elements, 22075);
            ResourceDict.Add(ResourceType.Carbon, 901);

            TeamID = teamID;

            // Sets the default available build items

            AvailableBuildings = new List<int>();
            
            AvailableBuildings.Add(1);
            AvailableBuildings.Add(2);
            AvailableBuildings.Add(3);
            AvailableBuildings.Add(4);
            AvailableBuildings.Add(5);



        }

        /// <summary>
        /// This update iterates through the items detected by a 
        /// team and decrements their detection timer, removing
        /// them from the detection list once it reaches zero.
        /// On every detection by a Sensor Sphere, the detection 
        /// timer is reset.
        /// To avoid this class inheriting MonoBehaviour, it is
        /// called by the game controller each frame.
        /// </summary>
        public void ControlledUpdate(float deltaTime) {

            Dictionary<GameItem, float> newDetectedItems = new Dictionary<GameItem, float>();
            List<GameItem> detectedList = new List<GameItem>();

            foreach (KeyValuePair<GameItem, float> kvp in DetectedItems) {

                float value = kvp.Value - deltaTime;
                if ((value > 0) && (kvp.Key != null)) {
                    newDetectedItems.Add(kvp.Key, value);
                    detectedList.Add(kvp.Key);
                }
            }
            
            DetectedItems = newDetectedItems;
        }

        // Takes a resource dictionary and subtracts current resource dictionary.
        // The operation will fail to complete if any of the costs is higher than
        // the amount of that resource the team has.
        // Returns true if the operation is a success, false if otherwise. 


        public bool Purchase(Dictionary<ResourceType, int> cost) {

            foreach (KeyValuePair<ResourceType, int> pair in cost) {

                if (ResourceDict[pair.Key] < pair.Value) {

                    return false;
                }
            }

            foreach (KeyValuePair<ResourceType, int> pair in cost) {


                ResourceDict[pair.Key] -= pair.Value;
            }

            return true;
        }

        
        public void Return(Dictionary<ResourceType, int> cost) {

            foreach (KeyValuePair<ResourceType, int> pair in cost) {

                    
                    ResourceDict[pair.Key] += pair.Value;
                }
            }

        // Used to update a single resource, ideal when mining.
        public void UpdateResource(ResourceType resource, int modifier) {

            Console.WriteLine("Resource updated: Added " + modifier.ToString() + " of " + resource.ToString());
            
            ResourceDict[resource] += modifier;
        }

        public bool CanDetect(GameItem target) {

            return DetectedItems.ContainsKey(target);
        }

        public void AddDetectedItem(GameItem target) {

            if (!CanDetect(target)) {
                DetectedItems.Add(target, DetectionTime);
                target.Detected();

            }
            else {
                DetectedItems[target] = DetectionTime;
            }
        }

        public void RemoveDetected(GameItem target) {

            DetectedItems.Remove(target);
            target.Undetected();
        }

        public List<GameItem> GetDetected() {

            List<GameItem> detectedList = new List<GameItem>();

            foreach (KeyValuePair<GameItem, float> kvp in DetectedItems) {
                detectedList.Add(kvp.Key);
            }

            return detectedList;
        }

        public void AddTeamUnit(GameItem unit) {

            UnitList.Add(unit);

            if (TeamController == null) {

                Debug.Log($"Adding unit for team {TeamID}. Team Controller is null");
            }
            else {
                Debug.Log($"Adding unit for team {TeamID}. Team Controller is not null");

            }


            if ((TeamController != null) && (unit is Building building) && (building.CanBuild)) {
                TeamController.AddProductionBuilding(building);

                Debug.Log($"Adding production building {building.Name} for team {TeamID}");
            }

        }

        public void RemoveTeamUnit(GameItem unit) {

            UnitList.Remove(unit);

            if ((TeamController != null) && (unit is Building building) && (building.CanBuild)) {
                TeamController.RemoveProductionBuilding(building);
            }
        }

        public List<GameItem> GetUnitList() {

            return UnitList;
        }

        public int GetUnitCount() {

            return UnitList.Count;
        }

        /// <summary>
        /// This method allows this TeamState to be commandeered by
        /// an AI. 
        /// </summary>
        /// <param ai="The AI controller to which this Team will belong"></param>
        public void SetAI(AITeamController ai) {

            TeamController = ai;

            Debug.Log($"Team Controller set for team {TeamID}");

        }

    }
}