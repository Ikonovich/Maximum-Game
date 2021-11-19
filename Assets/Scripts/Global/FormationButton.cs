using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {


    public class FormationButton : RadialButton {

        [SerializeField, PropertyBackingField(nameof(FormationShape))]
        private FormationShape formationShape;
        public FormationShape FormationShape { get => formationShape; set => formationShape = value; }
        
        void Awake() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            Player = GameController.GetPlayer();
            Renderer renderer = GetComponentInChildren<Renderer>();
            Material = renderer.material;

            Material.SetTexture("_MainTex", Texture);

        }

        public override void Selected() {

            Debug.Log($"Formation button selected.");

            Unit unit = Parent.GetComponent<Unit>();

            unit.HideMenu();
        }


    }
}
