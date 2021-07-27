using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {

    ///</summary>
    /// This enumerates the available resource types.
    /// This allows resource deposits to be set to the particular resource type
    /// and buildable items to cost them.
    /// </summary>
	public enum ResourceType {
		Energy,
		Metal,
		Crystal
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

        protected Dictionary<int, GameItem> WorldItemDict;
        protected Dictionary<int, TeamState> TeamList;

        protected OverheadCamera OverheadCamera;

        //#nullable
        protected Unit ActiveUnit;

        protected bool IsOverheadView = true;


        void Start() {

            Transform overhead = transform.Find("OverheadCamera");

            OverheadCamera = overhead.gameObject.GetComponent<OverheadCamera>();

        }

        void Update() {

        }

        public void Register(GameItem gameItem) {

            int teamID = gameItem.TeamID;

            if (TeamList.ContainsKey(teamID) == false) {

                TeamState teamState = new TeamState();

                teamState.CreateTeam(teamID);
                TeamList.Add(teamID, teamState);
            }

        }

        public void RemoveFromRegister(int gameID) {

            WorldItemDict.Remove(gameID);
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
    }
}
