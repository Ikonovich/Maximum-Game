using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    ///</summary>
    /// This enumerates the available resource types.
    /// This allows resource deposits to be set to the particular resource type
    /// and buildable items to cost them.
    /// </summary>
	public enum ResourceType {
		Energy,
		Elements,
		Carbon
	}

    /// <summary>
    /// This enumerates the richness of a resource deposit.
    /// </summary>

    public enum ResourceRichness {
        Normal = 1,
        Rich = 2,
        VeryRich = 3
    }

        public enum TeamColor {
        
            Grey = 0,
            Blue = 1,
            Red = 2,
            Green = 3,
            Yellow = 4,
            Magenta = 5,
            Cyan = 6,
            Black = 7
        }

    ///</summary>
    /// This is the universal game controller. 
    /// When a unit in the game is instantiated it registers with this class and receives a 
    /// randomly generated 6 digit GameID number.
    /// 
    /// The GameController then registers this unit to a TeamState, creating a new state if one with that
    /// unit's team ID does not exist. 
    /// Units are responsible for reporting their destruction and change of team to the GameController.
    /// 
    /// </summary>

    public class GameController : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(OverheadCameraObject))]
        private GameObject overheadCameraObject;
        public GameObject OverheadCameraObject { get => overheadCameraObject; set => overheadCameraObject = value; }
        

        protected Dictionary<int, GameItem> WorldItemDict = new Dictionary<int, GameItem>();

        protected Dictionary<int, TeamState> TeamList = new Dictionary<int, TeamState>();


        
        protected OverheadCamera OverheadCamera;

        protected Unit ActiveUnit;

        protected bool IsOverheadView = true;

        protected void Awake() {

             
            //WorldItemDict = new Dictionary<int, GameItem>();
            OverheadCamera = OverheadCameraObject.GetComponent<OverheadCamera>();


        }

        void Update() {

            foreach (KeyValuePair<int, TeamState> kvp in TeamList) {

                kvp.Value.ControlledUpdate(Time.deltaTime);
            }
        }

        public static Color GetColor(int teamID) {

            switch(teamID) {

                case 0:
                    return new Color(0.5f, 0.5f, 0.5f, 1f);  
                
                case 1:
                    return new Color(0f, 0f, 1f, 1f);  
                
                case 2:
                    return new Color(1f, 0f, 0f, 1f);  
                
                case 3:
                    return new Color(1f, 0f, 0f, 1f); 

                case 4:
                    return new Color(0f, 1f, 0f, 1f); 

                case 5:
                    return new Color(1f, 0f, 1f, 1f); 

                case 6:
                    return new Color(0f, 1f, 1f, 1f); 

                case 7:
                    return new Color(1f, 1f, 1f, 1f); 
            }

            return new Color(1f, 0.7f, 0f, 1f);
        }

        public static Color GetColor(TeamColor color) {

            switch(color) {

                case TeamColor.Grey:
                    return new Color(0.5f, 0.5f, 0.5f, 1f);  
                
                case TeamColor.Blue:
                    return new Color(0f, 0f, 1f, 1f);  
                
                case TeamColor.Red:
                    return new Color(1f, 0f, 0f, 1f);  
                
                case TeamColor.Green:
                    return new Color(1f, 0f, 0f, 1f); 

                case TeamColor.Yellow:
                    return new Color(0f, 1f, 0f, 1f); 

                case TeamColor.Magenta:
                    return new Color(1f, 0f, 1f, 1f); 

                case TeamColor.Cyan:
                    return new Color(0f, 1f, 1f, 1f); 

                case TeamColor.Black:
                    return new Color(1f, 1f, 1f, 1f); 

            }

            return new Color(1f, 0.7f, 0f, 1f);

        }

        public int Register(GameItem gameItem) {

            Debug.Log($"Registering item with controller: {gameItem.Name}");


            System.Random rand = new System.Random();

            int teamID = gameItem.TeamID;

            if (TeamList.ContainsKey(teamID) == false) {

                TeamState teamState = new TeamState();

                teamState.CreateTeam(teamID);
                TeamList.Add(teamID, teamState);
            }

            int newID = 0;

            while ((newID == 0) || (WorldItemDict.ContainsKey(newID) == true))  {

                newID = rand.Next(100000, 999999);
            }

            WorldItemDict.Add(newID, gameItem);
            TeamList[gameItem.TeamID].AddTeamUnit(gameItem);

            return newID;

        }

        public void RemoveFromRegister(int gameID) {

            GameItem item = WorldItemDict[gameID];

            if (item !=  null) {

                TeamList[item.TeamID].RemoveTeamUnit(item);
                WorldItemDict.Remove(gameID);
                Debug.Log("Item removed from Register: " + gameID);

            }
        }

        public void EnterUnit(Unit unit) {

            Debug.Log($"Entering unit");

            if ((ActiveUnit != null) && (IsOverheadView == false)) {
                ActiveUnit.SetPlayer(false);
            }
            else {
                OverheadCamera.SetPlayer(false);
                IsOverheadView = false;
            }
            ActiveUnit = unit;
            ActiveUnit.SetPlayer(true);
        }
        public void EnterOverhead() {

            IsOverheadView = true;
            ActiveUnit.SetPlayer(false);
            OverheadCamera.SetPlayer(true);
        }

        // This method handles building from inside the overhead camera or a unit.
        // Buildings have their item building handles internally.

        public void BuildItem(string itemID) {

            if (IsOverheadView == true) {

                Selector selector = OverheadCamera.gameObject.GetComponent<Selector>();
                selector.BuildItem(itemID);

            }
            else {

                Selector selector = ActiveUnit.gameObject.GetComponent<Selector>();
                selector.BuildItem(itemID);
            }
        }

        public TeamState GetTeamState(int teamID) {
        

            if (TeamList.ContainsKey(teamID) == false) { 
                
                TeamState teamState = new TeamState();
                teamState.CreateTeam(teamID);
                TeamList.Add(teamID, teamState);

            }
    
            return TeamList[teamID];
        }

        public GameObject GetPlayer() {
            
                if (IsOverheadView == true) {

                    return OverheadCameraObject;
                }
                else {

                    return ActiveUnit.gameObject;
            }
        }

        public Dictionary<int, GameItem> GetWorldItemDict() {

            return WorldItemDict;
        }

        public OverheadCamera GetOverhead() {
            
            return OverheadCamera;
            
        }

        /// <summary>
        /// This method takes a list of items that an object can
        /// build, compares them to the list of items available
        /// to that team, and returns the ones that are available.
        /// </summary>
        /// <param items="The list of items to be checked"></param>
        /// <param teamID="The Team whose available items are checked"></param>
        public List<string> CheckBuildable(List<string> items, int teamID) {

            return items;
        }
    }
}
