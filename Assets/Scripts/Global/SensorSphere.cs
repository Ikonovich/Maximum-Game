using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    /// <summary>
    /// This class functions as a combination of range sensor
    /// and line of sight sensor.
    /// It is triggered whenever a GameItem enters its range,
    /// and from there keeps track of whether or not the item
    /// is within the Line of Sight or not. 
    /// It can also be setup to function as non-LOS sensors, 
    /// such as radar.
    /// A ranking system is used to determine whether or not the
    /// sensor can detect a specific item at a specific time.
    ///
    /// A sensor can detech any item with the same relevant stealth 
    /// level as itself. I.E. a level 0 radar can detect only
    /// objects with 0 radar stealth value, a level 1 sensor can
    /// detect objects with a level 1 radar stealth value or below,
    /// etc.
    /// </summary>


    public class SensorSphere : MonoBehaviour {

        [SerializeField]
        private float LineOfSight = 100f;
        [SerializeField]
        [Range(0, 3)]
        protected int SightLevel = 0;

        [SerializeField]
        protected bool HasRadar = false;

        [SerializeField]
        protected float RadarRange = 0f;

        [SerializeField]
        [Range(0, 3)]
        protected int RadarLevel = 0;

        /// <summary>
        /// If this is enabled the sensor seeks targets 
        /// that are friendly, rather than enemies.
        /// </summary>

        [SerializeField]
        protected bool IsFriendlyScan = false;

        protected GameItem Parent;

        protected int WeaponRange = 0;

        protected TeamState TeamState;

        protected GameController GameController;

        protected float ScanTimer = 0.5f;
        
        protected float ScanCountdown = 0.0f;

        void Awake() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            FogHole fogHole = GetComponent<FogHole>();

            if (fogHole != null) {

                fogHole.Radius = LineOfSight + 5;
            }

            Parent = GetComponent<GameItem>();

            if (Parent is Unit unit) {

                WeaponRange = unit.Range;
            }

            TeamState = GameController.GetTeamState(Parent.TeamID);


        }

        void Update() {

            ScanCountdown -= Time.deltaTime;

            if (ScanCountdown <= 0) {
                Scan();
            }
        }


        protected void Scan() {

            ScanCountdown = ScanTimer;

            float range = LineOfSight;

            if (HasRadar == true) {

                range = Mathf.Max(LineOfSight, RadarRange);
            }

            int layerMask = 8195;
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask, QueryTriggerInteraction.Ignore);

            Debug.Log($"Sensor sphere weapn range for GameID {Parent.GameID}: {WeaponRange}");

            for (int i = 0; i < colliders.Length; i++) {
                

                GameObject itemObject = colliders[i].gameObject;
                GameItem item = itemObject.GetComponent<GameItem>();

                if ((item != null) && (item.gameObject != gameObject)) {

                    if ((item.TeamID != Parent.TeamID) && (item.TeamID != 0)) {

                        Debug.Log($"Sensor sphere for GameID {Parent.GameID} processing item {item.GameID} with range {range}");


                        float distance = (itemObject.transform.position - transform.position).magnitude;

                        if (distance < LineOfSight) {

                            item.LOSDetected();

                        }
                        else {
                            item.LOSUndetected();
                        }

                        if (distance < range) {
                            
                            TeamState.AddDetectedItem(item);
                        }

                        if ((IsFriendlyScan == false) && (Parent.AttackTarget == null) && (Parent.AutoTarget == null) && (item.TeamID != 0) && (distance < WeaponRange)) {
                            

                            Parent.AutoTarget = item.gameObject;
                        }
                    }
                    else if ((IsFriendlyScan == true) && (item.TeamID == Parent.TeamID)) {

                        float distance = (itemObject.transform.position - transform.position).magnitude;

                        
                        Debug.Log($"Sensor sphere for GameID {Parent.GameID} processing friendly item {item.GameID} with range {distance}");

                        if ((distance < WeaponRange) && (item.CurrentHealth < item.MaxHealth)) {

                            Parent.AutoTarget = item.gameObject;
                        }
                    }
                }
            }
        }
    }
}