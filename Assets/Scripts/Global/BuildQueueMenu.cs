using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Candlelight;



namespace MaxGame {


    /// </summary>
    /// This class populates handles menu opening and closing and populates the menu
    /// by providing a text file to the json handler. This file must be set in the editor.
    /// 
    /// When the menu is initialized, each button is assigned as a key in a dictionary 
    /// for it's respective menu (the regular menu or the queue menu);
    /// When populated, they assign a string to each button entry. These strings are passed around
    /// and used to identify buildable item types.
    /// </summary>

    public class BuildQueueMenu : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(QueueEnabled))]
        private bool queueEnabled;
        public bool QueueEnabled { get => queueEnabled; set => queueEnabled = value; }
        

        protected UIDocument MenuDocument;

        protected VisualElement Menu;

        protected Dictionary<Button, string> ButtonDict;

        protected List<Tuple<Button, ImageElement>> ImageTuples;

        protected Dictionary<Button, string> QueueButtonDict;

        protected List<string> ItemList;

        protected GameController GameController;

        protected bool IsInitiated = false;


        protected void Start() {

            GameObject controllerObject = GameObject.Find("GameController");
            GameController = controllerObject.GetComponent<GameController>();

            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;


            ItemList = new List<string>();
            ItemList.Add("Drone");
            ItemList.Add("Refinery");
            ItemList.Add("Tank");

            Initialize();
            InitiateMenus();
            Populate();


        }

        protected void Initialize() {

        }

        public void InitiateMenus() {

            ButtonDict = new Dictionary<Button, string>();

            ImageTuples = new List<Tuple<Button, ImageElement>>();
            
            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            
            
            Button exitButton = Menu.Q<Button>("ExitButton");
            exitButton.clicked += () => ExitButtonClicked();


            for (int i = 0; i < 9; i++) {

                Debug.Log($"Looking for button: " + "Button" + i.ToString());


                Button tempButton = Menu.Q<Button>("Button" + i.ToString());

                ImageElement tempImage = tempButton.Q<ImageElement>();

                Tuple<Button, ImageElement> imageTuple = new Tuple<Button, ImageElement>(tempButton, tempImage);
                ImageTuples.Add(imageTuple);


                tempButton.clicked += () => MenuButtonClicked(tempButton);

                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.yellow);

            

                IsInitiated = true;

            }

            Debug.Log($"Creating menu of " + ImageTuples.Count + " Buttons");


            if (QueueEnabled == true) {
            
                QueueButtonDict = new Dictionary<Button, string>();

                for (int i = 1; i < 8; i++) {

                    Button tempButton = Menu.Q<Button>("QueueButton" + i.ToString());

                    tempButton.clicked += () => QueueButtonClicked(tempButton);
                    tempButton.style.visibility = Visibility.Hidden;



                    Debug.Log($"Queue button " + i + " hidden.");
                }
            }
        }

        public void Populate() {

            InitiateMenus();
            
                
            ItemList = new List<string>();
            ItemList.Add("Drone");
            ItemList.Add("Refinery");
            ItemList.Add("Tank");


            for (int i = 0; i < 9; i++) {

                Debug.Log($"Populating button " + i);

                Button tempButton = ImageTuples[i].Item1;
                ImageElement tempImage = ImageTuples[i].Item2;

                //Button tempButton = Menu.Q<Button>("Button" + i.ToString());
                //ImageElement tempImage = tempButton.Q<ImageElement>();


                if (i < ItemList.Count) {
                        
                    Texture texture = Resources.Load<RenderTexture>(ItemList[i] + "Icon");

                    tempImage.SetTexture(texture);
                    tempImage.SetItemID(ItemList[i]);


                    tempButton.style.visibility = Visibility.Visible;
                    tempButton.tooltip = ItemList[i];

                        
                }
                else {

                    Texture texture = Resources.Load<Texture>("EmptyIcon");
                    tempImage.SetTexture(texture);
                    tempImage.SetItemID("None");

                    tempButton.style.visibility = Visibility.Hidden;
                }
            }
            Debug.Log($"Menu populated");


        }

        protected void MenuButtonClicked(Button button) {

            Debug.Log($"Button clicked " + nameof(button));

            ImageElement tempImage = button.Q<ImageElement>();
            string itemID = tempImage.ItemID;

            GameController.BuildItem(itemID);

            gameObject.SetActive(false);

        }

        protected void QueueButtonClicked(Button button) {

            string itemID = ButtonDict[button];

            Debug.Log($"Button clicked " );
        }

        protected void ExitButtonClicked() {

            gameObject.SetActive(false);
        }

    }

}