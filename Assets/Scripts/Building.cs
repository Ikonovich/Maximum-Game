using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {


    /// </summary>
    /// This class provides the basic functionalities to production buildings.
    /// Specifically, any structure with a build menu will utilize this class.
    /// It provides functionality for accessing build menus and setting rally points
    /// for newly produced units.
    ///
    /// </summary>

    public class Building : Structure {


        [SerializeField, PropertyBackingField(nameof(BuildMenuObject))]
        private GameObject buildMenuObject;
        public GameObject BuildMenuObject { get => buildMenuObject; set => buildMenuObject = value; }
        
        protected BuildQueueMenu BuildQueueMenu;

        protected float DoubleSelectTime = 0.22f;

        protected float DoubleSelectCountdown = 0.0f;

        protected void Start() {

            Initialize();
        }
    
        protected override void Initialize() {

        }


        protected void Update() {

            if (DoubleSelectCountdown > 0) {

                DoubleSelectCountdown -= Time.deltaTime;
            }
        }

        public override void Selected() {

            Debug.Log($"Building selected");

            if (DoubleSelectCountdown > 0) {

                Debug.Log($"Building double selected");

                ToggleBuildMenu();

            }
            else {
                DoubleSelectCountdown = DoubleSelectTime;
            }
        }

        protected void ToggleBuildMenu() {

            Debug.Log($"Toggling Structure menu");

            bool active = !(BuildMenuObject.activeSelf);
            BuildMenuObject.SetActive(active);

            if (active == true) {
                BuildMenuObject.GetComponent<BuildQueueMenu>().Populate();
            }
        }
    }

}