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

        protected InputAction EnableRotate;

        protected InputAction MousePosition;


        protected Vector2 InputVector = Vector2.zero;

        protected float ZoomScalar = 0.0f;

        protected float TranslateSpeed = 100.0f;
        protected float RotateSpeed = 30.0f;

        protected float ScrollSpeed = 15.0f;

        protected bool IsPlayer = true;

        protected Camera Camera;

        protected AudioListener Listener;

        protected GameObject BuildMenu;

        protected GameController GameController;


        void Start()
        {

            
            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            
            Transform temp = transform.Find("BuildQueueMenu");
            BuildMenu = temp.gameObject;
            BuildMenu.SetActive(false);

            
            Camera = GetComponent<Camera>();
            Listener = GetComponent<AudioListener>();
            var playerInput = GetComponent<PlayerInput>();

            if (playerInput != null) {
                EnableRotate = playerInput.actions.FindAction("EnableRotate");
                MousePosition = playerInput.actions.FindAction("MousePosition");

            }

        }

        // Update is called once per frame
        void Update()
        {
            if (IsPlayer == true) {

                ProcessMovement();
            }
        }

    
        protected void ProcessMovement() {

            if ((InputVector != Vector2.zero) || (ZoomScalar != 0.0f)) {


                float xKeyboard = InputVector.x; 
                float zKeyboard = InputVector.y;


                xKeyboard *= Time.deltaTime;
                zKeyboard *= Time.deltaTime;
                

                transform.Translate(new Vector3(xKeyboard * TranslateSpeed, -ZoomScalar * ScrollSpeed * Time.deltaTime, zKeyboard * TranslateSpeed), Space.World);
            }
        }

        protected void OnToggleBuildMenu() {

            Debug.Log($"Toggling build menu");

            bool active = !(BuildMenu.activeSelf);
            BuildMenu.SetActive(active);
           // BuildMenu.GetComponent<BuildQueueMenu>().InitiateMenus();

            if (active == true) {
                BuildMenu.GetComponent<BuildQueueMenu>().Populate();
            }
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
                mouseMovement *= (TranslateSpeed * 0.1f);
                transform.Translate(new Vector3(mouseMovement.x, 0.0f, mouseMovement.y), Space.World);
            }
        }


    }
}
