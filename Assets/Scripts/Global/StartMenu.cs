using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Candlelight;


namespace MaxGame {


    public class StartMenu : MonoBehaviour {

        protected Button NewGame;

        protected Button LoadGame;

        protected Button Settings;

        protected UIDocument MenuDocument;

        protected VisualElement Menu;

       [SerializeField, PropertyBackingField(nameof(Loader))]
       private GameObject loader;
       public GameObject Loader { get => loader; set => loader = value; }

       [SerializeField, PropertyBackingField(nameof(PropertyName))]
       private RenderTexture progressTexture;
       public RenderTexture ProgressTexture { get => progressTexture; set => progressTexture = value; }
       
       
        

        void Start() {

            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            NewGame = Menu.Q<Button>("NewGame");
            LoadGame = Menu.Q<Button>("LoadGame");
            Settings = Menu.Q<Button>("Settings");

            NewGame.clicked += () => NewGamePressed();
            LoadGame.clicked += () => LoadGamePressed();
            Settings.clicked += () => SettingsPressed();


            NewGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
            NewGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
            NewGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
            NewGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

            NewGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = new Color(48, 48, 48, 255));
            NewGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = new Color(48, 48, 48, 255));
            NewGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = new Color(48, 48, 48, 255));
            NewGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = new Color(48, 48, 48, 255));


            LoadGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
            LoadGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
            LoadGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
            LoadGame.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

            LoadGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = new Color(48, 48, 48, 255));
            LoadGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = new Color(48, 48, 48, 255));
            LoadGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = new Color(48, 48, 48, 255));
            LoadGame.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = new Color(48, 48, 48, 255));

            Settings.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderTopColor = Color.green);
            Settings.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderBottomColor = Color.green);
            Settings.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderRightColor = Color.green);
            Settings.RegisterCallback<MouseEnterEvent>(e => (e.target as VisualElement).style.borderLeftColor = Color.green);

            Settings.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderTopColor = new Color(48, 48, 48, 255));
            Settings.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderBottomColor = new Color(48, 48, 48, 255));
            Settings.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderRightColor = new Color(48, 48, 48, 255));
            Settings.RegisterCallback<MouseLeaveEvent>(e => (e.target as VisualElement).style.borderLeftColor = new Color(48, 48, 48, 255));

        }

        protected void NewGamePressed() {

            
            LoadManager loadManager = Loader.GetComponent<LoadManager>();
            loadManager.LoadScene("GaiaSceneOne");



        }

        protected void LoadGamePressed() {


        }

        protected void SettingsPressed() {



        }

    }

}