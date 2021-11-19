using System;
using System.Collections.Generic;
using UnityEngine;


namespace MaxGame {

    /// <summary>
    /// This class is used to position each item stored in the GameController's WorldItemDict
    /// on the world map. 
    ///
    /// The update countdown is used to improve efficiency.
    /// </summary>





    public class GameMap : MonoBehaviour {

        protected GameController GameController;

        protected float UpdateCountdown = 0.0f;

        protected float UpdateTime = 0.5f;




        protected void Awake() {

            GameObject controllerObj = GameObject.Find("/GameController");
            GameController = controllerObj.GetComponent<GameController>();

        }


        protected void UpdateLate() {

            if (UpdateCountdown <= 0) {

                UpdateCountdown -= Time.deltaTime;

                foreach (KeyValuePair<int, GameItem> kvp in GameController.GetWorldItemDict()) {

                    GameObject itemObj = kvp.Value.gameObject;
                    Vector3 position = gameObject.transform.position;


                

                } 

                UpdateCountdown = UpdateTime;
            }



        }




    }
}