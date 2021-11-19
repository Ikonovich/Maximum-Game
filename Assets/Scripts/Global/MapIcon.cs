using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    public class MapIcon : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(Icon))]
        private Texture icon;
        public Texture Icon { get => icon; set => icon = value; }
        
        protected GameController GameController;
        protected GameObject MinimapCamObject;

        protected Vector3 DefaultScale;

        protected Material Material;

        protected GameItem Parent;

        protected Color Color;

        protected int TeamID;



        void Awake() {


            MinimapCamObject = GameObject.Find("MinimapCam");
            
            Material = GetComponent<Renderer>().material;
            Material.SetTexture("_MainTex", Icon);

            Parent = transform.parent.gameObject.GetComponent<GameItem>();

            TeamID = Parent.TeamID;

            Color = GameController.GetColor(TeamID);

            Material.SetColor("_Color", Color);

            DefaultScale = transform.localScale;

        }


        void Update() {

            if (TeamID != Parent.TeamID) {
                TeamID = Parent.TeamID;

                Color = GameController.GetColor(TeamID);

                Material.SetColor("_Color", Color);
            }

            //transform.localScale = new Vector3(MinimapCamObject.transform.position.y / 15f, MinimapCamObject.transform.position.y / 15f, 1f);     

            transform.localScale = new Vector3(Mathf.Sqrt(MinimapCamObject.transform.position.y + 200f), Mathf.Sqrt(MinimapCamObject.transform.position.y + 200f), 1f);

        }
        
    }
}