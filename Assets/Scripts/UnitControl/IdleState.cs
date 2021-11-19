using System;
using UnityEngine;

namespace MaxGame.Units.Control {

	public class IdleState : MachineState {


		public override string StateName { get; }= "Idle";

		
		/// <remarks>
		/// Stores the Unit that this state is responsible for the behavior of.
		/// Set from the unit controller.
		/// </remarks>

		public override Unit Parent { get; set; }



		/// <remarks>
		/// This parameter determines how far away the unit will travel from the position
		/// where it became idle, in order to attack an enemy. 
		/// </remarks>
		protected float AggressiveDistance = 5.0f;

		/// <remarks>
		/// This stores the position where the unit entered the idle state.
		/// After pursuing an enemy in the aggressive stance a certain distance or until it is destroyed,
		/// the unit will return to this location.
		///</remarks>

		protected Vector3 IdlePosition;


		/// <remarks>
		/// Stores the movement controller..
		/// </remarks>
		protected MovementController MovementController;

		protected UnitController UnitController;


		public void Start() {

			MovementController = gameObject.GetComponent<MovementController>();
			UnitController = gameObject.GetComponent<UnitController>();

			Parent = gameObject.GetComponent<Unit>();
		}

		public override void RunState() {
			

			// Checks to see if the unit is in the aggressive stance and if it has detected a target.

			if (Parent.Stance == "Aggressive") {

				if (Parent.AttackTarget != null) {

				Vector3 targetPos = Parent.AttackTarget.transform.position;
				Vector3 parentPos = Parent.gameObject.transform.position;
				// Gets the distance from IdlePosition and distance to the target.

					float distanceFromIdle = (IdlePosition - parentPos).magnitude;

					float targetDistance = (parentPos - targetPos).magnitude;

					if ((distanceFromIdle < AggressiveDistance) && (targetDistance > Parent.Range)) {

						MovementController.MoveToAttack(Parent.AttackTarget.GetComponent<GameItem>());
						
					}
				}

			}
		}

		public override void BeginState() {

			Console.WriteLine("Controller is beginning");


			InitializeState();
		}

		public override void EndState() {
			
		}


		public override void InitializeState() {

			IdlePosition = Parent.gameObject.transform.position;
		}

		public override void TransitionState(MachineState newState) {

			Console.WriteLine("IdleState is transitioning the state");

			EndState();
			newState.BeginState();
		}

		 public override void ChangeState(string stateName) {


			UnitController.ChangeState(stateName);
		 }
	}
}
