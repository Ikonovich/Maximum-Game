using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;



namespace MaxGame {


    public class Selector : MonoBehaviour {


        [SerializeField, PropertyBackingField(nameof(CameraObject))]
        private GameObject cameraObject;
        public GameObject CameraObject { get => cameraObject; set => cameraObject = value; }

        protected Camera Camera;

        protected InputAction Select;
        protected InputAction MousePosition;

        protected GameController GameController;

        protected GameObject BuildObject;

        protected float SelectionTime = 0.15f;

        protected float SelectionCountdown = 0.0f;

        protected bool IsBuildMode = false;

        protected float BuildTime = 1.0f;

        protected float BuildCountdown = 0.0f;

        protected float RotateValue = 0f;

        protected float RotateScalar = 100f;



        protected void Start() {


            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();


            var playerInput = gameObject.GetComponent<PlayerInput>();

            Camera = CameraObject.GetComponent<Camera>();

            if (playerInput != null) {
                Select = playerInput.actions.FindAction("Select");
                MousePosition = playerInput.actions.FindAction("MousePosition");

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

            // Disables colliding with the Preview layer
            int layerMask = 1 << 6;
            layerMask = ~layerMask;

            RaycastHit collider;
            Ray rayCast = Camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(rayCast, out collider, range, layerMask)) {

                if (IsBuildMode == true) {
                    BuildMode(collider);
                }
                else {
                    GameObject item = collider.transform.gameObject;

                    if (item.GetComponent<GameItem>() == true) {
                        GameItem gameItem = item.GetComponent<GameItem>();

                        if ((Select.ReadValue<float>() > 0f) && (SelectionCountdown <= 0f)) {

                            Debug.Log($"Selection");
                            SelectionCountdown = SelectionTime;
                            gameItem.Selected();

                        }
                        else {
                            gameItem.CursorInteract();
                        }

                    }
                }
            }   
        }

        protected void BuildMode(RaycastHit collider) {


            BuildObject.transform.position = collider.point;
            
            BuildObject.transform.Rotate(0f, RotateValue * RotateScalar, 0f);


            //BuildObject.transform.rotation = (Quaternion.FromToRotation(Vector3.up, collider.normal));



             if ((Select.ReadValue<float>() > 0) && (BuildCountdown <= 0)) {

                FinalizeBuild();
                SelectionCountdown = SelectionTime;
             }

        }

        protected void FinalizeBuild() {
            
            BoxCollider collider = BuildObject.GetComponent<BoxCollider>();
            collider.enabled = true;


            BuildObject.layer = 1;
            IsBuildMode = false;
        }

        public void BuildItem(string itemID) {

            Debug.Log($"Receiving command to build " + itemID);

            GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);

            BuildObject = Instantiate(originalObject);

            BoxCollider collider = BuildObject.GetComponent<BoxCollider>();

            collider.enabled = false;

            BuildCountdown = BuildTime;
            IsBuildMode = true;

        }

        protected void OnRotateItem(InputValue input) {

            Debug.Log($"Rotating ");


            if (IsBuildMode == true) { 


                RotateValue = input.Get<float>() * Time.deltaTime;


            }
        }
    }

}