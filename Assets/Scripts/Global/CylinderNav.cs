using System;
using UnityEngine;
using UnityEngine.AI;


namespace MaxGame {

    public class CylinderNav : MonoBehaviour {


        void Awake() {

            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            agent.SetDestination(new Vector3(1100.0f, 25f, 1100.0f));

            Debug.Log($"Agent is on Navmesh: " + agent.isOnNavMesh);

        }
    }
}