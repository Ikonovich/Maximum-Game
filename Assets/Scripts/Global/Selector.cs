using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Candlelight;



namespace MaxGame {


    public class Selector : MonoBehaviour {


        [SerializeField, PropertyBackingField(nameof(CameraObject))]
        private GameObject cameraObject;
        public GameObject CameraObject { get => cameraObject; set => cameraObject = value; }

        [SerializeField, PropertyBackingField(nameof(SelectionArea))]
        private GameObject selectionArea;
        public GameObject SelectionArea { get => selectionArea; set => selectionArea = value; }

        protected OverheadCamera OverheadCamera;
        protected Camera Camera;

        protected InputAction SpecialSelectAction;
        protected InputAction SelectAction;

        protected InputAction AltSelect;

        protected InputAction ZoomAction;

        protected InputAction SnapToGridAction;

        protected InputAction MousePosition;

        protected GameController GameController;

        protected GameObject BuildObject;

        protected GameObject FormationObject;

        protected Formation Formation;

        protected string BuildID;

        protected float SelectionTime = 0.01f;

        protected float SelectionCountdown = 0.0f;

        protected bool IsSelectMode = false;

        protected bool IsBuildMode = false;

        protected bool IsFormationMode = false;

        protected float BuildTime = 0.4f;

        protected float BuildCountdown = 0.0f;

        /// <summary>
        ///  RotateValue stores
        /// </summary>
        protected float RotateValue = 0f;

        protected float RotateScalar = 100f;

        protected float RotateCountdown = 0f;

        protected float RotateTime = 0.1f;


        protected List<InteractionItem> SelectedList;

        protected int TeamID;

        protected void Start() {


            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            OverheadCamera = CameraObject.GetComponent<OverheadCamera>();

            TeamID = OverheadCamera.TeamID;

            SelectedList = new List<InteractionItem>();

            var playerInput = gameObject.GetComponent<PlayerInput>();

            Camera = CameraObject.GetComponent<Camera>();

            if (playerInput != null) {

                SelectAction = playerInput.actions.FindAction("Select");
                AltSelect = playerInput.actions.FindAction("AltSelect");
                SpecialSelectAction = playerInput.actions.FindAction("SpecialSelect");

                MousePosition = playerInput.actions.FindAction("MousePosition");
                SnapToGridAction = playerInput.actions.FindAction("SnapToGrid");
                ZoomAction = playerInput.actions.FindAction("Zoom");
            }
        }


        protected void Update() {

            if (BuildCountdown > 0) {

                BuildCountdown -= Time.deltaTime;
            }

            if (SelectionCountdown > 0) {
                SelectionCountdown -= Time.deltaTime;
            }

            ProcessMouseDetection();

        }

        protected void ProcessMouseDetection() {


            int range = 10000;

            Vector2 mousePosition = MousePosition.ReadValue<Vector2>();

            // Disables colliding with the Ignore Raycasts layer
            int layerMask = 187;


            RaycastHit collider;
            Ray rayCast = Camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(rayCast, out collider, range, layerMask)) {

                if (AltSelect.ReadValue<float>() > 0f) {

                    AlternateSelect(collider);
                }
                else if (IsBuildMode == true) {
                    BuildMode(collider);
                }
                else if (IsFormationMode == true) {

                    FormationMode(collider);
                }
                else if (IsSelectMode == true) {

                    SelectMode(collider);
                }
                else if ((SelectAction.ReadValue<float>() > 0f) && (SelectionCountdown <= 0f)) {

                    if (SpecialSelectAction.ReadValue<float>() > 0f) {

                        SpecialSelect(collider);
                        
                    }
                    else {
                        IsSelectMode = true;
                        SelectMode(collider);
                    }

                }
                else {
                    
                    GameObject item = collider.transform.gameObject;

                    if (item.GetComponent<InteractionItem>() == true) {
                        InteractionItem interactionItem = item.GetComponent<InteractionItem>();
                        interactionItem.CursorInteract();
                    }
                }
            }   
        }

        protected void SelectMode(RaycastHit colliderOriginal) {

            Debug.Log($"Select mode online");

            // We need a raycast that won't hit triggers to place the selection area..

            int range = 10000;

            Vector2 mousePosition = MousePosition.ReadValue<Vector2>();

            int layerMask = 187;


            RaycastHit collider;
            Ray rayCast = Camera.ScreenPointToRay(mousePosition);

            Physics.Raycast(rayCast, out collider, range, layerMask, QueryTriggerInteraction.Ignore);

            
            SelectionArea area = SelectionArea.GetComponent<SelectionArea>();

            if (collider.transform.gameObject.GetComponent<WorldButton>() == true) {

                WorldButton button = collider.collider.gameObject.GetComponent<WorldButton>();
                if (button is FormationButton formationButton) {
                    Debug.Log($"Button is formation button");

                    CreateFormation(formationButton.FormationShape);
                }
                else {
                    Debug.Log($"Button is NOT formation button");

                }

                Select(collider.collider.gameObject.GetComponent<WorldButton>());
                SelectionCountdown = SelectionTime;
                IsSelectMode = false;

            }
            else if (SelectAction.ReadValue<float>() > 0f) {

                area.Selection(collider.point);
            }

            else {
                
                
                List<GameObject> objectList = area.EndSelection();
                List<InteractionItem> tempList = new List<InteractionItem>();

                foreach (GameObject instance in objectList) {

                    if (instance.GetComponent<InteractionItem>() == true) {

                        tempList.Add(instance.GetComponent<InteractionItem>());
                    }
                }
                Debug.Log($"Calling deselect");
                Deselect();

                SelectedList = new List<InteractionItem>();


                /// <remarks>
                /// If the list count is equal to 1, we want to check to see if this is a structure,
                /// adding it if it is. If the list size is greater than 1, only selected units will be
                /// added. 
                /// </remarks>

                if (tempList.Count == 1) {

                    Select(tempList[0]);

                }

                else {
                    foreach (InteractionItem item in tempList) {
                        
                        if (item is Unit unit) {

                            Select(unit);

                        }
                    }
                }

                Debug.Log($"Number of selected items: " + SelectedList.Count);

                IsSelectMode = false;
            }
        }

        protected void Select(InteractionItem item) {

            if (item.IsSelectable) {
                item.Selected();
                SelectedList.Add(item);
            }
        }

        protected void SpecialSelect(RaycastHit collider) {

            if (collider.transform.gameObject.GetComponent<Unit>() == true) {
                Unit unit = collider.transform.gameObject.GetComponent<Unit>();

                FormationObject = unit.GetFormation().gameObject;
                Formation = FormationObject.GetComponent<Formation>();
                Select(Formation);
                IsFormationMode = true;
            }
        }


        public void Deselect() {

            Debug.Log($"Deselect called");

            foreach (InteractionItem item in SelectedList) {

                item.Deselected();
            }
            
            SelectedList = new List<InteractionItem>();

        }

        public void DeselectItem(InteractionItem item) {

            item.Deselected();
            SelectedList.Remove(item);
        }

        protected void BuildMode(RaycastHit colliderOriginal) {
            
            // We need a raycast that won't hit triggers to place the building.

            int range = 10000;

            Vector2 mousePosition = MousePosition.ReadValue<Vector2>();

            int layerMask = 2;
            layerMask = ~layerMask;

            RaycastHit collider;
            Ray rayCast = Camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(rayCast, out collider, range, layerMask, QueryTriggerInteraction.Ignore)) {


                GameItem buildItem = BuildObject.GetComponent<GameItem>();
                BuildObject.transform.position = collider.point + buildItem.BuildOffset;

                                    
                if ((RotateCountdown <= 0) && (RotateValue != 0.0f)) {
                    
                    BuildObject.transform.Rotate(0f, RotateValue, 0f);
                    RotateCountdown = RotateTime;
                }
                else {

                    RotateCountdown -= Time.deltaTime;;
                }
            }

            

            //BuildObject.transform.rotation = (Quaternion.FromToRotation(Vector3.up, collider.normal));



            if ((SelectAction.ReadValue<float>() > 0) && (BuildCountdown <= 0)) {

                FinalizeBuild();
                SelectionCountdown = SelectionTime;
            }
            if (SnapToGridAction.ReadValue<float>() > 0) {


                Collider tempCollider = BuildObject.GetComponent<Collider>();
                Bounds bounds = tempCollider.bounds;


                float xEdge = BuildObject.transform.position.x + bounds.extents.x;
                float zEdge = BuildObject.transform.position.z + bounds.extents.z;

                float xRemainder = (bounds.extents.x % 1) / 2;
                float zRemainder = (bounds.extents.z % 1) / 2;



                int xIntOffset = (int)(xEdge / 10) * 10;
                int zIntOffset = (int)(zEdge / 10) * 10;

                

                float xOffset = (float)xIntOffset + bounds.extents.x;
                float zOffset = (float)zIntOffset + bounds.extents.z;


                BuildObject.transform.position =  new Vector3(xOffset, BuildObject.transform.position.y, zOffset); 
            }

        }

        protected void FinalizeBuild() {
            
            GameItem buildItem = BuildObject.GetComponent<GameItem>();
            TeamState teamState = GameController.GetTeamState(TeamID);

            if (teamState.Purchase(buildItem.GetCost()) == true) {
                
				FogHole fogHole = BuildObject.GetComponent<FogHole>();

                if (fogHole != null) {

                    fogHole.enabled = true;
                }

                SensorSphere sensor = BuildObject.GetComponent<SensorSphere>();
                sensor.enabled = true;

                BoxCollider collider = BuildObject.GetComponent<BoxCollider>();
                collider.enabled = true;


                NavMeshObstacle obstacle = BuildObject.GetComponent<NavMeshObstacle>();
                
                obstacle.enabled = true;

                
                BuildObject.layer = 1;

                buildItem.enabled = true;
                buildItem.IsSelectable = true;
                buildItem.TeamID = TeamID;
                buildItem.CurrentHealth = 1;
                buildItem.SetTransparency();
                buildItem.Register();

                BuildItem(BuildID);
            }
        }

        public void BuildItem(string itemID) {

            Debug.Log($"Receiving command to build " + itemID);

            GameObject tempObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);

            GameItem tempItem = tempObject.GetComponent<GameItem>();
            

            BuildObject = Instantiate(tempObject);
            BuildObject.transform.rotation = Quaternion.identity;

            BoxCollider collider = BuildObject.GetComponent<BoxCollider>();
            collider.enabled = false;

            NavMeshObstacle obstacle = BuildObject.GetComponent<NavMeshObstacle>();
            obstacle.enabled = false;

            BuildCountdown = BuildTime;
            BuildID = itemID;
            IsBuildMode = true;

        }

        protected void AlternateSelect(RaycastHit collider) {


            // Anytime this is selected, build mode deactivates.

            if (IsBuildMode == true) {
                ExitBuildMode();
            }
            
            GameObject item = collider.transform.gameObject;

            if (item.GetComponent<GameItem>() == true) {

                GameItem target = item.GetComponent<GameItem>();

                if (SelectedList != null) {
                    foreach (GameItem gameItem in SelectedList) {

                        Debug.Log($"Setting item as unit target");
                        gameItem.SetTarget(target);
                    }

                }
            }
            else {

                if (SelectedList != null) {
                    
                    foreach (InteractionItem interItem in SelectedList) {

                        if (interItem is GameItem gameItem) {
                            gameItem.SetTarget(collider.point);
                        }
                    }
                }
            }
        }

        public void ExitBuildMode() {

            GameObject.Destroy(BuildObject);
            BuildObject = null;
            IsBuildMode = false;

            
            Deselect();
        }

        protected void OnRotateItem(InputValue input) {

            Debug.Log($"Rotating ");


            if ((IsBuildMode == true) || (IsFormationMode == true)) { 

                RotateValue = input.Get<float>() * 5;
            }

        }

        protected void OnAltSelect(InputValue input) {

            Debug.Log($"Alt select firing.");

        }

        protected void CreateFormation(FormationShape shape) {

            Debug.Log($"Creating formation in selector");

            List<Unit> unitList = new List<Unit>();
            GameObject tempObject = GameObject.Find("/GameController/PreviewArea/FormationObject");
            FormationObject = Instantiate(tempObject);
            Formation = FormationObject.GetComponent<Formation>();


            foreach (InteractionItem item in SelectedList) {

                if (item is Unit unit) {

                    unitList.Add(unit);
                }
            }
            if (unitList.Count > 1) {

                Formation.CreateFormation(unitList, shape);
                Formation.TeamID = this.TeamID;
                
                IsFormationMode = true;
                BuildCountdown = BuildTime;
            }

        }

        protected void FormationMode(RaycastHit collider) {

            GameObject area = Formation.GetArea();
            area.SetActive(true);


            area.transform.position = collider.point;

            if ((RotateCountdown <= 0) && (RotateValue != 0.0f)) {
                 
                area.transform.Rotate(0f, RotateValue, 0f);

                RotateCountdown = RotateTime;
            }
            else {

                RotateCountdown -= Time.deltaTime;;
            }

            if ((SelectAction.ReadValue<float>() > 0) && (BuildCountdown <= 0f)) {

                FinalizeFormation(collider);
            }
        }

        protected void FinalizeFormation(RaycastHit collider) {

            Debug.Log($"Finalizing formation");
            IsFormationMode = false;

            Formation.Finalize();
            Formation.SetTarget(collider.point);

        }
    }
}