using System;
using UnityEngine;

namespace MaxGame {

    //<summary>
    //  This is a generic state that permits generic interfacing with a StateController node.
    //
    //
    //</summary>

    public abstract class MachineState : MonoBehaviour {

        // <remarks>
        // Stores the name of this state.
        // Used to access the state in the Controller's state dictionary.
        // </remarks>
        public abstract string StateName { get; }

        // <remarks>
        // Stores the parent unit effected by this state.
        // </remarks>
        public abstract Unit Parent { get; set; }


        // <remarks>
        // Called when moving from this state to a new state.
        // </remarks>
        public abstract void TransitionState(MachineState newState);


        // <remarks>
        // Called when running this state during a physics frame.
        // </remarks>
        public abstract void RunState();


        // <remarks>
        // Called when entering this state to setup any necessary parameters.
        // </remarks>
        public abstract void InitializeState();

        // <remarks>
        // Called when moving into this state.
        // </remarks>
        public abstract void BeginState();

        // <remarks>
        // Called when leaving this state to deinitialize, if necessary.
        // </remarks>
        public abstract void EndState();


        // <remarks>
        // Called when leaving this state reaches a state transition point. Intended to 
        // tell the UnitController to update the state.
        // </remarks>

        public abstract void ChangeState(string stateName);

    }
}