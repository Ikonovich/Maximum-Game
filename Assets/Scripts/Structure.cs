using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {


    /// </summary>
    /// This class provides the basic functionalities to stationary world items.
    /// Production Buildings and Turrets inherit from this class.
    ///
    /// </summary>

    //[RequireComponent(typeof(SensorSphere))]
    //[RequireComponent(typeof(BoxCollider))]
    //[RequireComponent(typeof(Rigidbody))]

    public class Structure : GameItem {

        [HideInInspector]
        public GameObject TargetMarker;

    
    
    }

}