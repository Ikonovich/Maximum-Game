using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MaxGame {

    ///<summary> 
    ///  This is the default camera view for the player.
    ///
    ///</summary>
    public class OverheadCamera : MonoBehaviour
    {

        public int TeamID = 1;

        protected bool IsFrozen = false;

        protected InputAction EnableRotate;

        protected InputAction MousePosition;

        protected InputAction SpeedMove;


        protected Vector2 InputVector = Vector2.zero;

        protected float ZoomScalar = 0.0f;

        protected float TranslateSpeed = 100.0f;
        protected float RotateSpeed = 5.0f;

        protected float ScrollSpeed = 15.0f;

        protected bool IsPlayer = true;

        protected Camera Camera;

        protected AudioListener Listener;

        protected GameObject BuildMenu;

        protected GameController GameController;

        protected Selector Selector;



        void Start()
        {

            
            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            
            Transform temp = transform.Find("DefaultBuildMenu");
            BuildMenu = temp.gameObject;
            
            Camera = GetComponent<Camera>();
            Listener = GetComponent<AudioListener>();
            Selector = GetComponent<Selector>();
            var playerInput = GetComponent<PlayerInput>();


            if (playerInput != null) {
                EnableRotate = playerInput.actions.FindAction("EnableRotate");
                MousePosition = playerInput.actions.FindAction("MousePosition");
                SpeedMove = playerInput.actions.FindAction("SpeedMove");

            }

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (IsPlayer == true) {

                ProcessMovement();
            }
        }
    
        protected void ProcessMovement()  {

            if (IsFrozen == false) {

                float terrainHeight = Terrain.activeTerrain.SampleHeight(this.transform.position);
                Debug.Log($"Terrain height at camera location: " + terrainHeight);

                if (((this.transform.position.y - terrainHeight) < 10.0f) && (ZoomScalar > 0)) {

                    ZoomScalar = 0.0f;

                    Debug.Log($"Zoom set to 0 by force.");
                }

                if ((InputVector != Vector2.zero) || (ZoomScalar != 0.0f)) {

                    float translateSpeed = TranslateSpeed;


                    if (SpeedMove.ReadValue<float>() > 0.0f) {

                        translateSpeed = TranslateSpeed * 2.5f;
                    }


                    float xKeyboard = InputVector.x; 
                    float zKeyboard = InputVector.y;


                    xKeyboard *= Time.deltaTime;
                    zKeyboard *= Time.deltaTime;
                    
                    float yCurrent = transform.position.y;

                    transform.Translate(new Vector3(xKeyboard * translateSpeed, 0.0f, zKeyboard * translateSpeed));

                    transform.position = new Vector3(transform.position.x, yCurrent, transform.position.z);
                    
                    RaycastHit collider;
                    Ray rayCast = Camera.ScreenPointToRay(MousePosition.ReadValue<Vector2>());

                    Physics.Raycast(rayCast, out collider, 500);
                    transform.position = Vector3.MoveTowards(transform.position, rayCast.GetPoint(500), ZoomScalar * translateSpeed * 0.03f * Time.deltaTime);
                    
                }
            }
        }

        protected void OnToggleDebugConsole() {

            Debug.Log($"Toggling debug console");

        }

        protected void OnToggleBuildMenu() {

            Debug.Log($"Toggling build menu");

            Selector.ExitBuildMode();
   
            BuildMenu.GetComponent<DefaultBuildMenu>().ToggleMenu();
        }

        public void SetPlayer(bool active) {

            Camera.enabled = active;
            Listener.enabled = active;

        }

        protected void OnZoom(InputValue input) {


            ZoomScalar = input.Get<float>();

            
            //Debug.Log($"OnZoom: {ZoomScalar}");

        }

        protected void OnMove(InputValue input) {


            InputVector = input.Get<Vector2>();
            
           // Debug.Log($"OnMove: {InputVector}");

        }

        protected void OnMousePosition(InputValue input) {

            
            Vector2 mousePosition = input.Get<Vector2>();
            //Debug.Log($"Mouse position {mousePosition} ");
            

        }

        protected void OnMouseMove(InputValue input) {


            Vector2 mouseMovement = input.Get<Vector2>();

            mouseMovement *= Time.deltaTime;

            if (EnableRotate.ReadValue<float>() > 0.0f) {

                mouseMovement *= RotateSpeed;

                transform.Rotate(-mouseMovement.y, mouseMovement.x, 0.0f);

                
                Vector3 euler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(euler.x, euler.y, 0.0f);

            }
            else {
               // mouseMovement *= (TranslateSpeed * 0.05f);
                //transform.Translate(new Vector3(mouseMovement.x, 0.0f, mouseMovement.y), Space.World);
            }
        }

        public void Freeze() {

            Debug.Log($"Overhead camera frozen");

            IsFrozen = true;
        }

        public void Unfreeze() {

            Debug.Log($"Overhead camera unfrozen");

            IsFrozen = false;
        }
    }
}
