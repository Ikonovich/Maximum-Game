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

        [SerializeField, PropertyBackingField(nameof(BuildItemsText))]
        private TextAsset buildItemsText;
        public TextAsset BuildItemsText { get => buildItemsText; set => buildItemsText = value; }
        

              
        [SerializeField, PropertyBackingField(nameof(CanBuild))]
        private bool canBuild = true;
        public bool CanBuild { get => canBuild; set => canBuild = value; }
        
        
        protected BuildingMenu BuildingMenu;
        
        protected GameObject SpawnAreas;

        protected List<string> BuildableItems;

        void Start() {
            Initialize();
        }

            

        protected override void Initialize() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            
            TargetMarker = GetComponentInChildren<TargetMarker>(true).gameObject;


            Transform tempTransform = transform.Find("SelectionEffect");
            SelectionEffect = tempTransform.gameObject;

            StatusEffects = new List<StatusEffect>();

            // Get the build menu.
            BuildingMenu = GetComponentInChildren<BuildingMenu>();


            // Initializes the spawn area
            tempTransform = transform.Find("SpawnArea");
            SpawnAreas = tempTransform.gameObject;

            // Gathers the materials for setting transparency.
            
            List<Material> Materials = new List<Material>();

            MapIcon = GetComponentInChildren<MapIcon>(true).gameObject;
            
            if (TeamID != 1) {

                MapIcon.SetActive(false);
            }
            Register();
            
        }

        protected void Update() {



            if (InteractionCountdown > 0) {

                InteractionCountdown -= Time.deltaTime;

                if (InteractionCountdown <= 0) {

                }
            }

            if (DoubleSelectCountdown > 0) {

                DoubleSelectCountdown -= Time.deltaTime;
            }

            ProcessEffects();

        }

        public override void CursorInteract()
        {
            InteractionCountdown = InteractionTime;
            
        }

        public override void Selected() {

            if (IsSelectable == true) {

                //TargetMarker.SetActive(true);

                SelectionEffect.SetActive(true);


                Debug.Log($"Building selected");

                if (DoubleSelectCountdown > 0) {

                    Debug.Log($"Building double selected");

                    OpenBuildMenu();

                }
                else {
                    DoubleSelectCountdown = DoubleSelectTime;
                }
            }
        }

        public override void Deselected() {

            if (IsSelectable == true) {

                SelectionEffect.SetActive(false);

                //TargetMarker.SetActive(false);

            }
            
        }

        protected void OpenBuildMenu() {

            if (CanBuild == true) {

                Debug.Log($"Opening build menu from building");

                BuildingMenu.OpenMenu();
            }
        }

        public List<string> GetBuildable() {

            if (BuildableItems == null) {

                BuildableItems = new List<string>();
                string[] itemArray = buildItemsText.text.Split(',');

                foreach (string item in itemArray) {
                    BuildableItems.Add(item);
                    Debug.Log($"Returning build item: {item}");


                }
            }
            return BuildableItems;
        }

        public bool BuildItem(string itemID) {

            GameObject spawnObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);

            return Spawn(spawnObject);

        }

        /// <summary>
        /// This checks each of the children of the building's spawn area and spawns the provided object
        /// at the first empty one (one that isn't colliding).
        /// Returns true if it finds an empty spawn spot, false otherwise.
        ///
        ///</summary>
        protected bool Spawn(GameObject buildObject) {

            foreach (Transform child in SpawnAreas.transform) {

                GameObject spawnObject = child.gameObject;
                SpawnArea spawnArea = spawnObject.GetComponent<SpawnArea>();



                if (spawnArea.IsOccupied == false) {

                    GameObject newObject = Instantiate(buildObject);
                    Unit item = newObject.GetComponent<Unit>();

                    MonoBehaviour[] components = newObject.GetComponents<MonoBehaviour>();

                    for (int i = 0; i < components.Length; i++) {

                        components[i].enabled = true;

                    }

                    item.Warp(child.position);
                    newObject.transform.rotation = child.rotation;
                    

                    item.enabled = true;
                    item.IsActive = true;
                    item.TeamID = this.TeamID; 
                    item.Register();
                    item.IsSelectable = true;

                    return true;
                }
                else {
                    Debug.Log($"Spawn area is occupied");
                }

            }

            return false;
        }

        public override void SetTarget(GameItem target) {

            Debug.Log($"Target object set in building");
            AttackTarget = target.gameObject;

        }

            
        public override void SetTarget(Vector3 target) {
            
            Debug.Log($"Target point set in building");
            TargetPoint = target;

        }

    }
    

}