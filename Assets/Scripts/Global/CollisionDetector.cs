using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame { 


    /// <summary>
    /// This class is used to implement raycast-based collision avoidance. Currently, it is designed to be 
    /// attached directly to an empty game object, which is rotated to provide the 
    /// forward direction for the current raycast.
    /// The parent GameItem must be the parent transform of the Rotator object.
    /// The Rotator transform is rotated by 15 degrees for each raycast.
    /// When choosing a travel direction, each direction is weighted by the sum of the three 
    /// raycast returns nearest to that direction, as well as which direction is more oriented 
    /// towards the parent's PathPoint, or the TargetPoint if the PathPoint is null.
    /// </summary>


    public class CollisionDetector : MonoBehaviour {

        
        protected GameItem Parent;

        protected Transform Rotator;

        protected float Distance = 10.0f;

        protected List<float> ResultList;
        

        void Awake() {

            Rotator = this.transform;

            Parent = transform.parent.GetComponent<GameItem>();

        }

        /// <summary>
        /// This method scans the surrounding using raycasts cast at 45 degree intervals. 
        /// It weights each result by taking the distance to the target achieved by 
        /// going one unit in that direction and comparing it to the current distance to the target.
        /// Directions that will result in going further from the target result in negative values.
        /// Direction weights should always be 1 or less.
        ///
        /// If the raycast in that direction results in a collision, the inverse of the
        /// collider distance mulitplied by 10 is subtracted from the Direction Weight.
        /// Through this mechanism sufficiently far colliders can still result in the unit choosing
        /// that direction.
        ///
        /// The calculated results are added to the ResultList with index 0 containing the 0 rotation,
        /// index 1 containing 45 degree rotation, etc.
        /// </summary>

        public List<float> Scan() {

            ResultList = new List<float>();

            Rotator.localEulerAngles = Vector3.zero;

            //Vector3 targetPoint = Parent.Path.corners[Parent.PathIndex] + new Vector3(0f, 5f, 0f);
            Vector3 targetPoint = Parent.TargetPoint + new Vector3(0f, 5f, 0f);


            float baseDistance = (targetPoint - Parent.transform.position).magnitude;

            for (int i = 0; i < 8; i++) {

                float directionWeight = (targetPoint - (Rotator.position + (Rotator.forward * 10))).magnitude;

                RaycastHit hit;

                if (Physics.Raycast(Rotator.position + Rotator.forward, Rotator.forward, out hit, Distance)) {

                    ResultList.Add(directionWeight - ((1 / hit.distance) * 30));
                    Debug.Log($"Degree from local zero: " + (i * 22.5f).ToString() + "Weight: " + (directionWeight - ((1 / hit.distance) * 10)));
                }
                else {

                    ResultList.Add(directionWeight);
                }

                Rotator.Rotate(0f, 30f, 0f);
                Debug.DrawLine(Rotator.position, Rotator.position + (Rotator.forward * 10), Color.black, 1.0f, false);

            }

            return ResultList;
        }



    }

}