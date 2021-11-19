using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using MaxGame;

namespace MaxGame.Units.Control {

	//<summary>
	// This controller class handles changing states for individual units.
	// When another state reaches a change-of-state point, it emits a signal that 
	// instructs this Controller to direct a state change.
	// </summary>


	public class UnitController : MonoBehaviour {


		// <remarks>
		// Used to store a list of all states available to this controller.
		// For the UnitController, this means a list of all child nodes.
		// </remarks>
		protected Dictionary<string, MachineState> StateDict;

		// <remarks>
		// Used to store the current state of the object.
		// If at any point during runtime this becomes null, something terrible has happened.
		// </remarks>
		protected string ActiveState;

		//<remarks> 
		// Used to store the default object state name, which is set when the object is created.
		// Typically, this will be idle.
		//</remarks>
		protected string DefaultState = "Idle";

		//<remarks>
		// Stores the parent unit of this control structure.
		// </remarks>
		public Unit Parent;

		protected UnityEngine.AI.NavMeshAgent Agent;

		
		protected Animator Animator;

		public void Start() {

			
			Animator = gameObject.GetComponent<Animator>();
			Agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
			Parent = gameObject.GetComponent<Unit>();

			StateDict = new Dictionary<string, MachineState>();
			ActiveState = DefaultState;

			var childStates = gameObject.GetComponents<MachineState>();


			Debug.Log($"Adding states to unit controller");

			foreach (MachineState state in childStates) {
				
				Debug.Log($"Adding state " + state.StateName + " to unit controller");
				

				StateDict.Add(state.StateName, state);
			}


		}

		// <remarks>
		// Calls the currently active state to run during every physics cycle.
		//
		// </remarks>

		protected void Update() {


			MachineState activeState = StateDict[ActiveState];
			activeState.RunState();

		}

		///<remarks>
		/// This method is called to signal the unit controller that it needs to change the state.
		///</remarks>
		public void ChangeState(string stateName) {

			Debug.Log($"Changing state to " + stateName);
			TransitionState(stateName);
		}

		/// <remarks>
		/// This method calls the active state to conduct a state transition.
		/// </remarks>

		public void TransitionState(string stateName) {

			MachineState newState = StateDict[stateName];
			MachineState activeState = StateDict[ActiveState];

			activeState.TransitionState(newState);

			ActiveState = stateName;
		}


		public void TargetAssigned(GameItem target) {

			if (target.TeamID == Parent.TeamID) {

				TransitionState("FollowTarget");

			}
			else {

				TransitionState("AttackTarget");
			}
		}

		public void TargetAssigned(Vector3 target) {

			TransitionState("MoveToPoint");

		}

		public string GetStance() {

			return Parent.Stance;
		}





	}

}
