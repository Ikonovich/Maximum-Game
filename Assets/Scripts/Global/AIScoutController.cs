using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {


    /// <summary>
    /// This class handles scouting behavior for other AI controllers.
    /// When a scout behavior is requested, it first checks to see if any
    /// unoccupied scouts are available. 
    /// If a scout is no available , it sends one to the build
    /// queue with the appropriate priority. 
    /// If the build queue returns a time estimate that is too long, 
    /// it will override the goal of a lower priority scout if available.
    ///
    /// This class is controlled and initialized by the AITeamController.
    /// </summary>


    public class AIScoutController : MonoBehaviour {

        protected int TeamID;
        
        protected TeamState TeamState;

        protected AITeamController TeamController;


        public void ExternalInitialize(int teamID, TeamState teamState, AITeamController teamController) {

            TeamID = teamID;
            TeamState = teamState;
            TeamController = teamController;
        }

    }
}