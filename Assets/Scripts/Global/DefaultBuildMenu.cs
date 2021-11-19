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

    public class DefaultBuildMenu : MonoBehaviour {



        [SerializeField, PropertyBackingField(nameof(Parent))]
        private GameObject parent;
        public GameObject Parent { get => parent; set => parent = value; }

        protected int TeamID;
        

        protected UIDocument MenuDocument;

        protected VisualElement Menu;

        protected Dictionary<Button, string> ButtonDict;
        

        protected List<Button> ButtonList;

        protected Dictionary<Button, string> QueueButtonDict;

        protected List<string> ItemList;

        protected GameController GameController;

        protected Selector Selector;

        protected bool IsInitiated = false;

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


        protected void Start() {

            GameObject controllerObject = GameObject.Find("GameController");
            GameController = controllerObject.GetComponent<GameController>();

            OverheadCamera overhead = Parent.GetComponent<OverheadCamera>();
            TeamID = overhead.TeamID;

            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;


            ItemList = new List<string>();
            
            ItemList.Add("Refinery");
            ItemList.Add("Drone");
            ItemList.Add("Tank");



        }

        public void InitiateMenus() {

            ButtonList = new List<Button>();
            ButtonDict = new Dictionary<Button, string>();
            
            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            if (MenuDocument == null) {

                Debug.Log($"MenuDocument is null");
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

                tempButton.RegisterCallback<MouseEnterEvent>(e => MouseEnteredButton(tempButton));
                tempButton.RegisterCallback<MouseLeaveEvent>(e => MouseExitedButton(tempButton));

                tempButton.clicked += () => MenuButtonClicked(tempButton);

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

            Debug.Log($"Creating menu of " + ButtonList.Count + " Buttons");
            IsInitiated = true;
        }

        public void Populate() {

            InitiateMenus();
                
            ItemList = new List<string>();
            ItemList.Add("Refinery");
            ItemList.Add("MissileTurret");
            ItemList.Add("MechFab");
            ItemList.Add("VehicleFab");
            ItemList.Add("LaserTurret");
            ItemList.Add("LaserInterceptorTurret");






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

            Debug.Log($"Menu populated");


        }

        protected void MenuButtonClicked(Button button) {

            Debug.Log($"Button clicked " + nameof(button));

            string itemID = ButtonDict[button];

            GameController.BuildItem(itemID);

            CloseMenu();

        }

        protected void MouseEnteredButton(Button button) {



            string itemID = ButtonDict[button];

            GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);
            GameItem gameItem = (GameItem)originalObject.GetComponent<GameItem>();

            NameLabel.text = gameItem.Name;
            EnergyLabel.text = gameItem.EnergyCost.ToString();
            MetalLabel.text = gameItem.ElementsCost.ToString();
            CrystalLabel.text = gameItem.CarbonCost.ToString();
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