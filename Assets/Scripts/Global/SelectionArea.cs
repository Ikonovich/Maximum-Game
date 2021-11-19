using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaxGame {

    public class SelectionArea : MonoBehaviour {

        protected List<GameObject> ColliderList;

        protected Vector3 Origin;

        protected Vector3 CurrentOrigin;

        protected bool IsActive = false;

        protected void Start() {
            ColliderList = new List<GameObject>();

        }

        protected void Update() {


        }

        public void Selection(Vector3 collisionPoint) {

            if (IsActive == false) {
                Origin = collisionPoint;
                ColliderList = new List<GameObject>();
                IsActive = true;
            }
            else {
                CurrentOrigin = Origin - ((Origin - collisionPoint) / 2);

                this.gameObject.transform.position = CurrentOrigin;
                this.gameObject.transform.localScale = (Origin - collisionPoint) + new Vector3(0, 200, 0);
            }

        }

        public List<GameObject> EndSelection() {
            

            this.gameObject.transform.localScale = Vector3.zero;
            IsActive = false;

            return ColliderList;
        }

        protected void OnTriggerEnter(Collider collider) {

            GameObject item = collider.gameObject;
            
            
            if (!(ColliderList.Contains(item))) {

                Debug.Log($"Adding collider to selection list.");

                ColliderList.Add(item);
            }
            
        }

        protected void OnTriggerExit(Collider collider) {

            GameObject item = collider.gameObject;
            
            
            if (ColliderList.Contains(item)) {

                Debug.Log($"Adding collider to selection list.");

                ColliderList.Remove(item);
            }
            
        }

        protected List<GameObject> GetColliderList() {

            return ColliderList;
        }


    }
}