using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    /// <summary>
    /// This class is used to position each item stored in the GameController's WorldItemDict
    /// on the world map. 
    ///
    /// The update countdown is used to improve efficiency.
    /// </summary>





    public class GridComponent : MonoBehaviour {

        protected GameController GameController;

        protected GameObject Player;

        /// <summary>
        /// IsLong is used to determine the magnification axis of this object.
        /// </summary>

        [SerializeField, PropertyBackingField(nameof(IsLong))]
        private bool isLong = false;
        public bool IsLong { get => isLong; set => isLong = value; }
        

        protected Material Material;

        protected float UpdateTime = 0.1f;

        protected float UpdateCountdown = 0.0f;



        protected void Awake() {

            GameObject controllerObj = GameObject.Find("/GameController");
            GameController = controllerObj.GetComponent<GameController>();

            MeshRenderer renderer = GetComponent<MeshRenderer>();

            Material = renderer.material;


        }


        protected void Update() {

        //     if (UpdateCountdown <= 0) {

        //         UpdateCountdown = UpdateTime;

        //         Player = GameController.GetPlayer();

        //         ///transform.LookAt(Player.transform, Vector3.up);

        //         float distance = (Player.transform.position - transform.position).magnitude;

        //         if (distance < 400) {

        //             .Log($"Setting grid transparency");

        //             Material.SetColor("_Color", new Color(0, 0, 0, (distance / 800)));
        //         }

                
        //         if (distance < 200) {


        //             Material.SetColor("_Color", new Color(0, 0, 0, 0));

        //         }


        //     }
        //     else {

        //         UpdateCountdown -= Time.deltaTime;
        //     }
        }
    }
}