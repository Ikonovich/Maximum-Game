using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Candlelight;

namespace MaxGame {

    public class UnitHUD : MonoBehaviour {


        [SerializeField, PropertyBackingField(nameof(Parent))]
        private GameObject parent;
        public GameObject Parent { get => parent; set => parent = value; }

        [SerializeField, PropertyBackingField(nameof(BackingTexture))]
        private Texture backingTexture;
        public Texture BackingTexture { get => backingTexture; set => backingTexture = value; }
        

        [SerializeField, PropertyBackingField(nameof(HealthTexture))]
        private RenderTexture healthTexture;
        public RenderTexture HealthTexture { get => healthTexture; set => healthTexture = value; }

        [SerializeField, PropertyBackingField(nameof(HealthMaterial))]
        private Material healthMaterial;
        public Material HealthMaterial { get => healthMaterial; set => healthMaterial = value; }
        
        Label EnergyLabel;
        Label MetalLabel;
        Label CrystalLabel;
        ImageElement HealthBar;



        

        protected GameController GameController;

        protected TeamState TeamState;

        protected UIDocument Document;

        protected VisualElement Menu;

        protected int TeamID = 1;




        protected void OnEnable() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            //TeamState = GameController.GetTeamState(TeamID);

            Document = gameObject.GetComponent<UIDocument>();
            Menu = Document.rootVisualElement;


            EnergyLabel = (Label)Menu.Q("EnergyLabel");
            MetalLabel = (Label)Menu.Q("MetalLabel");
            CrystalLabel = (Label)Menu.Q("CrystalLabel");
            HealthBar = (ImageElement)Menu.Q("HealthBar");


        }

        protected void Update() {

        

            Debug.Log($"Updating energy label");

            EnergyLabel.text = (5555).ToString(); // resourceDict[ResourceType.Energy].ToString();

            Debug.Log($"Updating metal label");


            MetalLabel.text = (100).ToString(); // resourceDict[ResourceType.Metal].ToString();

            Debug.Log($"Updating crystal label");


            CrystalLabel.text = (999).ToString(); //resourceDict[ResourceType.Crystal].ToString();


            float HealthPercent = ((float)Parent.GetComponent<Unit>().CurrentHealth / (float)Parent.GetComponent<Unit>().MaxHealth);


            HealthMaterial.SetFloat("_Percent", HealthPercent); 

            RenderTexture.active = HealthTexture;
            GL.Clear(true, true, Color.clear);
            //HealthTexture.Release();

            Graphics.Blit(BackingTexture, HealthTexture, HealthMaterial);
            HealthBar.SetTexture(HealthTexture);

        }
    }
}