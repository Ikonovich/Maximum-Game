using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        [SerializeField, PropertyBackingField(nameof(Parent))]
        private GameObject parent;
        public GameObject Parent { get => parent; set => parent = value; }
        
        
        protected UIDocument MenuDocument;

        protected VisualElement Menu;

        protected List<Button> ButtonList;


        protected Dictionary<Button, string> ButtonDict;

        protected List<Button> QueueButtonList;
        protected List<string> Queue;


        protected List<string> ItemList;

        protected GameController GameController;

        protected bool IsInitiated = false;

        protected bool HasWritten = false;

        protected float ClickTime = 0.1f;

        protected float ClickCountdown = 0.0f;
        
        protected VisualElement EnergyDisplay;
        protected VisualElement CrystalDisplay;

        protected VisualElement MetalDisplay;

        protected VisualElement HealthDisplay;

        protected VisualElement ShieldDisplay;

        protected Label NameLabel;
        protected Label EnergyLabel;
        protected Label MetalLabel;
        protected Label CrystalLabel;
        protected Label HealthLabel;
        protected Label ShieldLabel;








        // Stores the build countdown of the unit currently at the front of the queue.
        protected float BuildCountdown;
        // Stores the total build time of the item currently being built.
        protected float BuildTime;


        protected void Awake() {

            GameObject controllerObject = GameObject.Find("GameController");
            GameController = controllerObject.GetComponent<GameController>();

            MenuDocument = gameObject.GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            ItemList = new List<string>();
            Queue = new List<string>();
        }


        protected void Update() {

            if (ClickCountdown > 0) {

                ClickCountdown -= Time.deltaTime;

            }

            if (BuildCountdown > 0) {

                BuildCountdown -= Time.deltaTime;


                if (BuildCountdown <= 0) {

                    Dequeue();
                }
            }
        } 



        public void InitiateMenus() {

            ButtonDict = new Dictionary<Button, string>();
            ButtonList = new List<Button>();
            
            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            if (MenuDocument == null) {

                Debug.Log($"Menu docu is null");
            }

            if (Menu == null) {

                Debug.Log($"Menu is null");
            }

            ///

            NameLabel = (Label)Menu.Q("NameLabel");
            
            EnergyDisplay = Menu.Q("EnergyDisplay");
            EnergyLabel = (Label)Menu.Q("EnergyLabel");

            MetalDisplay = Menu.Q("MetalDisplay");
            MetalLabel = (Label)Menu.Q("MetalLabel");

            CrystalDisplay = Menu.Q("CrystalDisplay");
            CrystalLabel = (Label)Menu.Q("CrystalLabel");


            HealthDisplay = Menu.Q("HealthDisplay");
            HealthLabel = (Label)Menu.Q("HealthLabel");

            ShieldDisplay = Menu.Q("ShieldDisplay");
            ShieldLabel = (Label)Menu.Q("ShieldLabel");




            NameLabel.style.visibility = Visibility.Hidden;
            EnergyDisplay.style.visibility = Visibility.Hidden;
            MetalDisplay.style.visibility = Visibility.Hidden;
            CrystalDisplay.style.visibility = Visibility.Hidden;

            HealthDisplay.style.visibility = Visibility.Hidden;
            ShieldDisplay.style.visibility = Visibility.Hidden;

            ///
            
            Button exitButton = Menu.Q<Button>("ExitButton");
            exitButton.clicked += () => ExitButtonClicked();


            for (int i = 0; i < 9; i++) {

                Debug.Log($"Looking for button: " + "Button" + i.ToString());


                Button tempButton = Menu.Q<Button>("Button" + i.ToString());

                tempButton.clicked += () => MenuButtonClicked(tempButton);

                tempButton.RegisterCallback<MouseEnterEvent>(e => MouseEnteredButton(tempButton));
                tempButton.RegisterCallback<MouseLeaveEvent>(e => MouseExitedButton(tempButton));

                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.yellow);

            
                ButtonDict.Add(tempButton, "None");
                ButtonList.Add(tempButton);


            }

            Debug.Log($"Created menu of " + ButtonList.Count + " Buttons");

            QueueButtonList = new List<Button>();


            for (int i = 0; i < 6; i++) {

                Button tempButton = Menu.Q<Button>("QueueButton" + i.ToString());


                tempButton.clicked += () => QueueButtonClicked(tempButton);
                tempButton.style.visibility = Visibility.Hidden;

                tempButton.RegisterCallback<MouseEnterEvent>(e => MouseEnteredButton(tempButton));
                tempButton.RegisterCallback<MouseLeaveEvent>(e => MouseExitedButton(tempButton));

                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
                tempButton.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.yellow);
                tempButton.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.yellow);


                QueueButtonList.Add(tempButton);

            }
            
        }

        public void Populate() {

            InitiateMenus();
            
                
            ItemList = new List<string>();
            ItemList.Add("HeavySpider");
            ItemList.Add("Drone");



            for (int i = 0; i < 9; i++) {

                Debug.Log($"Populating button " + i);

                Button tempButton = ButtonList[i];


                if (i < ItemList.Count) {
                    
                    Texture2D texture = (Texture2D)Resources.Load<Texture2D>(ItemList[i] + "Image");

                    Background backgroundTexture = Background.FromTexture2D(texture);

                    StyleBackground background = new StyleBackground(backgroundTexture);

                    tempButton.style.backgroundImage = background;
                    tempButton.style.visibility = Visibility.Visible;
                    tempButton.tooltip = ItemList[i];
                    ButtonDict[tempButton] = ItemList[i];

                        
                }
                else {

                    Texture2D texture = Resources.Load<Texture2D>("blank.png");

                    StyleBackground background = new StyleBackground(texture);

                    tempButton.style.backgroundImage = background;
                    tempButton.tooltip = "None";
                    ButtonDict[tempButton] = "None";

                    tempButton.style.visibility = Visibility.Hidden;
                }
            }
        
            HasWritten = true;

            Debug.Log($"Menu populated.");             
            
            
            PopulateQueue();

        }

        /// <summary>
        /// Removes the first item from the queue and sends it Building.Spawn() to be built.
        ///
        /// </summary>
        protected void Dequeue() {


            Building building = Parent.GetComponent<Building>();
               

            if  (building.BuildItem(Queue[0]) == true) {

                Queue.RemoveAt(0);

                if (Queue.Count > 0) {
                
                    GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + Queue[0]);
                    GameItem gameItem = (GameItem)originalObject.GetComponent<GameItem>();
                    BuildTime = gameItem.BuildTime;
                    BuildCountdown = BuildTime;

                }

                PopulateQueue();
            }
            else {
                Debug.Log("Cannot dequeue, likely because all spawn areas are blocked.");
            }

        }

        protected void PopulateQueue() {

            Debug.Log($"Populating queue menu ");


            for (int i = 0; i < 6; i++) {

                Button tempButton = QueueButtonList[i];


                if (i < (Queue.Count)) {

                    

                    Debug.Log($"Populating queue button " + i + "with menu item " + Queue[i]);
                    
                    
                    Texture2D texture = Resources.Load<Texture2D>(Queue[i] + "Image");

                    StyleBackground background = new StyleBackground(texture);

                    tempButton.style.backgroundImage = background;
                    tempButton.tooltip = "None";



                    tempButton.style.visibility = Visibility.Visible;
                        
                }
                else {

                    Debug.Log($"Populating queue button " + i + "with blank");



                    Texture2D texture = Resources.Load<Texture2D>("blank");

                    StyleBackground background = new StyleBackground(texture);

                    tempButton.style.backgroundImage = background;
                    tempButton.tooltip = "None";

                    tempButton.style.visibility = Visibility.Hidden;
                }
            }
            Debug.Log($"Queue menu populated");
                        
        }

        protected void MenuButtonClicked(Button button) {

            if (ClickCountdown <= 0) {

                ClickCountdown = ClickTime;

                Debug.Log($"Button clicked " + nameof(button));

                string itemID = ButtonDict[button];


                GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);

                GameItem gameItem = (GameItem)originalObject.GetComponent<GameItem>();
                
                Dictionary<ResourceType, int> cost = gameItem.GetCost();
                Building building = Parent.GetComponent<Building>();

                TeamState teamState = GameController.GetTeamState(building.TeamID);

                if (teamState.Purchase(cost) == true) {
                    
                    Debug.Log($"Adding  " + itemID + "to queue.");

                    
                    Queue.Add(itemID);
                    BuildCountdown = gameItem.BuildTime;

                    PopulateQueue();
                }
            }
        }
        protected void MouseEnteredButton(Button button) {



            string itemID = ButtonDict[button];

            GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);
            GameItem gameItem = (GameItem)originalObject.GetComponent<GameItem>();

            NameLabel.text = gameItem.Name;
            EnergyLabel.text = gameItem.EnergyCost.ToString();
            MetalLabel.text = gameItem.MetalCost.ToString();
            CrystalLabel.text = gameItem.CrystalCost.ToString();
            HealthLabel.text = gameItem.MaxHealth.ToString();

            
            NameLabel.style.visibility = Visibility.Visible;
            EnergyDisplay.style.visibility = Visibility.Visible;
            MetalDisplay.style.visibility = Visibility.Visible;
            CrystalDisplay.style.visibility = Visibility.Visible;

            HealthDisplay.style.visibility = Visibility.Visible;

            if (gameItem.HasShields == true) {

                ShieldLabel.text = gameItem.MaxShieldStrength.ToString();
                ShieldDisplay.style.visibility = Visibility.Visible;

            }


        }

        protected void MouseExitedButton(Button tempButton) {

            NameLabel.style.visibility = Visibility.Hidden;
            EnergyDisplay.style.visibility = Visibility.Hidden;
            MetalDisplay.style.visibility = Visibility.Hidden;
            CrystalDisplay.style.visibility = Visibility.Hidden;

            HealthDisplay.style.visibility = Visibility.Hidden;
            ShieldDisplay.style.visibility = Visibility.Hidden;

        }
        
        



        protected void QueueButtonClicked(Button button) {

            if (ClickCountdown <= 0) {

                ClickCountdown = ClickTime;


                int index = QueueButtonList.IndexOf(button);
                Queue.RemoveAt(index);
                PopulateQueue();

                Debug.Log($"Item removed from queue button: "  + index);
            }
        }

        protected void ExitButtonClicked() {

            CloseMenu();
        }

        public void ToggleMenu() {

            if (MenuDocument.enabled == true) {

                CloseMenu();
            }
            else {
                OpenMenu();
            }

        }

        public void OpenMenu() {

            MenuDocument = gameObject.GetComponent<UIDocument>();

            Debug.Log($"Opening build menu from menu");


            MenuDocument.enabled = true;

            Populate();
        }

        /// <summary<
        /// Safely disables the menu.
        /// <summary>

        public void CloseMenu() {

            MenuDocument.enabled = false;
        }
    }
}