using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame { 

    /// <summary>
    /// This object allows instantiation of a generic radial menu.
    /// Must be attached to an empty game object if top level (to appropriately
    /// orient towards the player via LookAt).
    /// A top level menu can handle anywhere from 1 to 10 or so buttons.
    /// A non-top level menu should be restricted to three (3) buttons.
    /// </summary>

    public class RadialMenu : RadialButton {

        [Header("Texture")]
        [SerializeField, PropertyBackingField(nameof(CloseTexture))]
        private Texture closeTexture;
        public Texture CloseTexture { get => closeTexture; set => closeTexture = value; }

        [SerializeField, PropertyBackingField(nameof(ButtonObjList))]
        private List<GameObject> buttonObjList = new List<GameObject>();
        public List<GameObject> ButtonObjList { get => buttonObjList; set => buttonObjList = value; }

        [SerializeField, PropertyBackingField(nameof(InterpolateSpeed))]
        private float interpolateSpeed = 10.0f;
        public float InterpolateSpeed { get => interpolateSpeed; set => interpolateSpeed = value; }
        
        [SerializeField, PropertyBackingField(nameof(IsTopLevel))]
        private bool isTopLevel;
        public bool IsTopLevel { get => isTopLevel; set => isTopLevel = value; }
        
        protected List<RadialButton> ButtonList;

        protected bool MenuOpen = false;



        void Awake() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            Player = GameController.GetPlayer();

            Renderer renderer = GetComponentInChildren<Renderer>();
            Material = renderer.material;

            Material.SetTexture("_MainTex", CloseTexture);


            ButtonList = new List<RadialButton>();
            for (int i = 0; i < ButtonObjList.Count; i++) {

                RadialButton button = ButtonObjList[i].GetComponent<RadialButton>();
                button.Parent = Parent;
                button.gameObject.SetActive(false);
                ButtonList.Add(button);
            }
        }

        void Update() {

            Debug.Log($"Menu open?:" + MenuOpen);

            
            if (IsTopLevel) {
                this.transform.LookAt(Player.transform.position, Vector3.up);
            }


            if (ShowCountdown > 0.0f) {

                ShowCountdown -= Time.deltaTime;

                if ((ShowCountdown < 0.0f) && (MenuOpen == false)) {

                    Close(InterpolateSpeed);
                }
            }

            if (IsLerping == true) {

                Vector3 newPosition = Vector3.Lerp(this.transform.localPosition, Offset, Time.deltaTime * LerpSpeed);
                this.transform.localPosition = newPosition;
            }
            else if (IsClosing == true) {


                Vector3 newPosition = Vector3.zero;

                if (IsTopLevel) {
                    
                    if (this.transform.localPosition.magnitude < 10f) {

                        Hide();

                    }
                    newPosition = Vector3.Lerp(this.transform.localPosition, Vector3.zero, Time.deltaTime * LerpSpeed);
                }
                else { 
                    
                    if (this.transform.localPosition.magnitude < 0.1f) {

                        Hide();
                    }

                    newPosition = Vector3.Lerp(this.transform.localPosition, transform.parent.transform.localPosition, Time.deltaTime * LerpSpeed);
                }
                this.transform.localPosition = newPosition;
                
                for (int i = 0; i < ButtonList.Count; i++) {
                            
                    RadialButton button = ButtonList[i];
                    button.Close(InterpolateSpeed);

                }

            }

            if (MenuOpen == true) {


				// Gets the distance between this unit and the player and uses it to 
                // adjust the size and offset of the menu and buttons.


				float distance = (this.transform.position - Player.transform.position).magnitude;
				float radius = Mathf.Pow(distance, (1.0f/4.0f));
				float height = Mathf.Sqrt(distance) * 2;



				// Sets up the parameters if this is a top level menu.
                if (IsTopLevel) {

                    
				    Vector3 offsetVec = new Vector3(2.0f, 0.0f, 0.0f);
                    float offsetAngle = 360 / ButtonList.Count;

                    this.transform.position = transform.parent.transform.position + Offset;
                    this.transform.localScale = new Vector3(height / 2.0f, height / 2.0f, 0.0f);


                    Debug.Log($"Number of menu buttons: " + ButtonList.Count);


                    for (int i = 0; i < ButtonList.Count; i++) {
                            
                        RadialButton button = ButtonList[i];
                    
                        //Vector3 buttonOffset = Quaternion.AngleAxis(offsetAngle * i, Vector3.forward) * offsetVec;
                        Vector3 buttonOffset = Quaternion.Euler(0.0f, 0.0f, offsetAngle * i) * offsetVec;


                        
                        Debug.Log($"Calling button lerp with vector: " + buttonOffset.ToString());
                        button.Lerp(buttonOffset, InterpolateSpeed);
                    }
                }
                else {

                    
                    
                    float xSign = 0.0f;
                    float ySign = 0.0f;

                    if (this.transform.localPosition.x != 0.0f) {

                        xSign = Mathf.Sign(this.transform.localPosition.x);

                    }
                    if (this.transform.localPosition.y != 0.0f) {

                        ySign = Mathf.Sign(this.transform.localPosition.y);

                    }
                    
                    //Vector3 offsetVec = new Vector3(xSign, ySign, 0.0f);

                    Vector3 offsetVec = this.transform.localPosition / 1.5f;
                    float offsetAngle = -180.0f / ButtonList.Count;



                    for (int i = 0; i < ButtonList.Count; i++) {
                            
                        RadialButton button = ButtonList[i];
                        
                        Vector3 buttonOffset = Quaternion.Euler(0.0f, 0.0f, offsetAngle * i + 60.0f) * offsetVec;


                            
                        Debug.Log($"Calling button lerp with vector: " + buttonOffset.ToString());
                        button.Lerp(buttonOffset, InterpolateSpeed);
                    }
                    
                }
            }

        }

        public override void Selected() {
            
            Debug.Log($"Radial menu selected");
            OpenMenu();

        }

        public override void Deselected() {

            Close(InterpolateSpeed);
        }

        public void OpenMenu() {

            MenuOpen = true;
            Material.SetTexture("_MainTex", Texture);

            Vector3 offsetVec = new Vector3(0f, 10f, 0f);
            float offsetAngle = 360 / ButtonList.Count;



            for (int i = 0; i < ButtonList.Count; i++) {

                RadialButton button = ButtonList[i];
                button.gameObject.SetActive(true);
            }
        }

        
        public override void Close(float lerpSpeedIn) {

            
            MenuOpen = false;
            IsLerping = false;
            IsClosing = true;
            InterpolateSpeed = lerpSpeedIn;

        }

        public override void Hide() {

            if (ButtonList == null) {

                Debug.Log($"Stupid fucking button list magically vanished");
            }
            else {
                for (int i = 0; i < ButtonList.Count; i++) {
                                
                    RadialButton button = ButtonList[i];
                    if (IsTopLevel == false) {
                        this.transform.localPosition = Vector3.zero;
                    }
                            
                    button.Hide();
                }
            }


            if (Material != null) {
                Material.SetTexture("_MainTex", CloseTexture);
            }

            gameObject.SetActive(false);

        }

        public void OnInteract() {

            IsClosing = false;
            IsLerping = true;
            Lerp(Offset, InterpolateSpeed);
            ShowCountdown = ShowTime;
        }

    }



}