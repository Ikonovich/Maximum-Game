using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace MaxGame.Units.Control {

	//<summary>
	// This class handles all movement related behaviors for the unit that is set as it's parent.
	// It is called by various states as they seek to achieve their goals.
	// It does not effect what state the unit is in directly, but the unit controller may utilize
	// information from this class to decide what state to transition to.
	// The Unit-specific variables used by this class, such as Mass, are stored in the parent Unit.
	//</summary>


	public class MovementController : MonoBehaviour {


		protected Unit Parent;
		
		protected Animator Animator;

		protected NavMeshAgent Agent;

		protected Rigidbody Body;

		protected UnitController UnitController;

		// <remarks>
		// Stores the distance from a point that is acceptable to consider it reached.
		//
		// </remarks>
		public float DistanceMargin = 15.0f;

		// <remarks>
		// A factor to control how powerful the steering behavior is. Lower values turn more slowly.
		// </remarks>
		public float SteeringFactor = 10.0f;

		
		// <remarks>
		// Stores the distance from a point that is acceptable to consider it reached.
		//
		// </remarks>

		public float FollowDistance = 30.0f;

		protected void Start() {

			Parent = gameObject.GetComponent<Unit>();
			Animator = gameObject.GetComponent<Animator>();
			Agent = gameObject.GetComponent<NavMeshAgent>();
			UnitController = gameObject.GetComponent<UnitController>();
			Body = gameObject.GetComponent<Rigidbody>();



		}

		// <remarks>
		// This returns a bool letting the state machine know that the requested point has been reached.
		// </remarks>

		public void ApplyMovement() {

			
			// Animator.SetBool("Move", true);

				
			// // 	Multiplying the direction vector times acceleration and adding it to the velocity vector.

			// Parent.Velocity += (InputVector * Parent.Acceleration);

			
			// // Code handling deceleration

			// // First, we check to see if the unti is a ground unit.
			// // A different Deaccel factor applies to non ground units.

			// // Next we normalize the current velocity vector, subtracts the normalized input
			// // vector, and applies deceleration to the result.
			// // Effectively, only applies movement resistance in diirections
			// // that the player is not moving.

			

			// Parent.Velocity -= ((Parent.Velocity.normalized - InputVector.normalized) * Parent.VelocityDecay);

			// if (Parent.Velocity.magnitude > Parent.MaximumVelocity) {

			// 	Parent.Velocity = Parent.Velocity.normalized * Parent.MaximumVelocity;


			// }
			
			Animator.SetFloat("LinearVelocity", Agent.velocity.z);
			Animator.SetFloat("AngularVelocity", Body.angularVelocity.y);
			//Debug.Log($"Agent Linear velocity: " + Agent.velocity.z);
			//Debug.Log($"Agent Angular velocity: " + Agent.velocity.x);





			// Parent.transform.Translate((Parent.Velocity * Time.deltaTime), Space.World);
			// Parent.transform.LookAt(Parent.transform.position + Parent.Velocity.normalized * Time.deltaTime);
		
		}

		public bool MoveToPoint(Vector3 targetIn) {


			Vector3 target = targetIn;
				

			Agent.SetDestination(targetIn);
			Agent.isStopped = false;


				//movementVector = (target - Parent.gameObject.transform.position).normalized;


				//Debug.Log($"Calculated movement vector: " + movementVector.ToString());


				
			ApplyMovement();


			float distance = (targetIn - this.transform.position).magnitude;

			if (distance < DistanceMargin + 0.1) {

				Debug.Log($"Reached point.");

				Animator.SetFloat("LinearVelocity", 0f);
				Animator.SetFloat("AngularVelocity", 0f);
				Agent.ResetPath();

				ChangeState("Idle");
			}


			return false;

		}

	
		
		public bool MoveToFollow(GameItem targetItem) {

			Vector3 targetPos = targetItem.gameObject.transform.position;
			Vector3 parentPos = Parent.gameObject.transform.position;

									


			//Vector3 desiredVector = (targetPos - parentPos).normalized * Parent.MaximumVelocity;



			// Using the previous velocity to establish a gradient between the current vector
			// and the new vector.

			// Vector3 movementVector = desiredVector - Parent.Velocity;

			float distance = (targetPos - parentPos).magnitude;

			//Debug.Log($"Moving to follow");
			//Debug.Log($"Distance: " + distance.ToString());

			if (distance > FollowDistance) {

				Agent.isStopped = false;

				
				// Debug.Log($"Sending movement vector from move to follow: " + movementVector.ToString());
				Agent.SetDestination(targetItem.transform.position);
				ApplyMovement();
				
				return false;
			}
			else {

				Animator.SetFloat("LinearVelocity", 0f);
				Animator.SetFloat("AngularVelocity", 0f);
				Agent.ResetPath();
				
				return true;
			}

		}

		public bool MoveToAttack(GameItem targetItem) {

			Vector3 targetPos = targetItem.gameObject.transform.position;
			Vector3 parentPos = Parent.gameObject.transform.position;


			//Vector3 desiredVector = (targetPos - parentPos).normalized * Parent.MaximumVelocity;


			// Using the previous velocity to establish a gradient between the current vector
			// and the new vector.

			//Vector3 movementVector = desiredVector - Parent.Velocity;

			
			float distance = (targetPos - parentPos).magnitude;

			// <remarks>
			// Seeks to place the unit within 90% of the weapon range of the parent object.
			// This means that the target moving won't constantly take it in and out of weapoon
			// range.
			// </remarks>
			//Debug.Log($"Moving to attack");
			//Debug.Log($"Distance: " + distance.ToString());


			if (distance > Parent.Range * 0.9f) {

				Agent.isStopped = false;

				//Debug.Log($"Sending movement vector from move to attack: " + movementVector.ToString());

				Agent.SetDestination(targetItem.transform.position);

				ApplyMovement();
				
				return false;

			}
			else {

				Agent.isStopped = true;
				Animator.SetFloat("LinearVelocity", 0f);
				Animator.SetFloat("AngularVelocity", 0f);
				Agent.ResetPath();
				
				return true;
			}
		}

		protected void ChangeState(string stateName) {


			UnitController.ChangeState(stateName);
		 }
	}
}
