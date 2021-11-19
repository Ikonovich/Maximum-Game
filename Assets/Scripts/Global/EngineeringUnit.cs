using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Candlelight;

namespace MaxGame {

    public class EngineeringUnit : Unit {

        protected override void ProcessTargeting() {

            // Debug.Log($"Scan countdown: " + ScanCountdown);

            // if ((AttackTarget == null) && (AutoTarget == null) && (ScanCountdown <= 0)) {
                
            //     ScanCountdown = ScanTime;

            //     List<GameItem> tempList = ScanTree.StartSearch(this.transform.position, Range);
            //     foreach (GameItem item in tempList) {


            //         if ((item.TeamID == TeamID) && (item != this)) {

            //             if ((item.CurrentHealth < item.MaxHealth) && (item.CurrentHealth > 0)) {

            //                 AutoTarget = item.gameObject;
                            
            //                 Debug.Log($"Engineer search returned item with Game ID: " + item.GameID);

            //                 break;
            //             }
            //         }
                
            //     }

            //     Debug.Log($"Search returned " + tempList.Count + " items.");
            // }
        }

            public override void SetTarget(GameItem target) {

                if (target != this) {

                    if (target.TeamID == TeamID) {

                        if ((target.CurrentHealth < target.MaxHealth) && (target.CurrentHealth > 0)) {
    
                            AttackTarget = target.gameObject;
                        }

                        FollowTarget = target;
                    }
                    else if (target is ResourceDeposit deposit) {

                        AttackTarget = target.gameObject;
                    }
                }


        }
    }
}
