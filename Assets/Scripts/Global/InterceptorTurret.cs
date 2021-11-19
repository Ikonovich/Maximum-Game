using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {



    /// </summary>
    /// This class provides a unique turret class that, rather than using the Scanner
    /// to search for targets, uses a trigger collision body to locate and
    /// destroy enemy munitions. 
    /// </summary>

    public class InterceptorTurret : Turret {


        void Update() {

            ProcessEffects();

        }
    

        public override void SetTarget(GameObject target) {

            AttackTarget = target;
        }
    }


}