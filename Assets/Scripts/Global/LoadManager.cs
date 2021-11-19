using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Candlelight;


namespace MaxGame {


    public class LoadManager : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(LoadBarTexture))]
        private RenderTexture loadBarTexture;
        public RenderTexture LoadBarTexture { get => loadBarTexture; set => loadBarTexture = value; }
        
        [SerializeField, PropertyBackingField(nameof(LoadBarMaterial))]
        private Material loadBarMaterial;
        public Material LoadBarMaterial { get => loadBarMaterial; set => loadBarMaterial = value; }
        

        protected AsyncOperation LoadOp;

        protected float MessageCountdown = 0.0f;

        protected UIDocument MenuDocument;

        protected VisualElement Menu;

        protected ImageElement LoadBar;

        protected float ProgressValue;

        protected Label LoadMessage;

        protected List<String> Messages;

        protected String CurrentMessage = "None";

        protected MaterialApplicator Applicator;

        protected String ReadyScene = "No Ready Scene";

        protected bool IsReady = false;

        protected bool IsLoading = false;


        void Start() {

            
            Applicator = GetComponent<MaterialApplicator>();
            MenuDocument = GetComponent<UIDocument>();
            Menu = MenuDocument.rootVisualElement;

            Menu.style.visibility = Visibility.Hidden;

            
        }


        void Update() {

            if (IsLoading) {

                float percent = 0.0f;
                if (LoadOp != null) {

                    percent = LoadOp.progress;
                }

                Debug.Log("Setting oad message");

                MessageCountdown -= Time.deltaTime;

                if (MessageCountdown <= 0) {

                    NewMessage();
                }

                LoadMessage.text = (percent * 100f).ToString() + "%: " + CurrentMessage;


                LoadBarMaterial.SetFloat("_Percent", percent);

                RenderTexture.active = LoadBarTexture;
                GL.Clear(true, true, Color.clear);

                Applicator.ApplyMaterial(LoadBarTexture, LoadBarMaterial);

                LoadBar.SetTexture(LoadBarTexture);


            }

            if (IsReady == true) {

                Debug.Log($"Loading scene: " + ReadyScene);
                LoadOp = SceneManager.LoadSceneAsync(ReadyScene);
                IsReady = false;
            }
        }

        public void LoadScene(String sceneName) {

            
            LoadMessage = Menu.Q<Label>("LoadMessage");
            LoadBar = Menu.Q<ImageElement>("LoadBar");

            Menu.style.visibility = Visibility.Visible;



            Messages = new List<String>();

            Messages.Add("Disabling sentient AI...");
            Messages.Add("Nuking from orbit...");
            Messages.Add("Crashing audio bus...");
            Messages.Add("Instigating registers...");
            Messages.Add("Downloading megahertz...");
            Messages.Add("Erasing browser history...");
            Messages.Add("Recalculating redundant calculations...");
            Messages.Add("Munching ram chips...");
            Messages.Add("Contemplating compilation...");
            Messages.Add("Obfuscating parameters...");

            NewMessage();
            LoadMessage.text = "0.0%: " + CurrentMessage;


        

            ReadyScene = sceneName;
            IsReady = true;
            IsLoading = true;
            LoadBarMaterial.SetFloat("_Percent", 0.0f);
            
            RenderTexture.active = LoadBarTexture;
            GL.Clear(true, true, Color.clear);
            Applicator.ApplyMaterial(LoadBarTexture, LoadBarMaterial);
            LoadBar.SetTexture(LoadBarTexture);


        }

        protected void NewMessage() {

            System.Random rand = new System.Random();

            int num = rand.Next(0, Messages.Count);
            int time = rand.Next(0, 2);

            float percent = 0.0f;

            MessageCountdown = (float)time;
            MessageCountdown = 0.3f;

            

            CurrentMessage = Messages[num];

            Debug.Log($"New message set.");


        }

    }
}