using System;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    public class HealthBar : MonoBehaviour {


        [HideInInspector]
        public GameObject Parent;
        
        protected GameController GameController;

        protected Material BarMat;

        protected void Awake() {

            Parent = transform.parent.gameObject;

            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            BarMat = renderer.material;

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
        }

        public void Update() {

            GameObject player = GameController.GetPlayer();


            BarMat.SetFloat("_Percent", GetPercentage());

        }

        public float GetPercentage() {

            GameItem gameItem = Parent.GetComponent<GameItem>();

            return ((float)gameItem.CurrentHealth / (float)gameItem.MaxHealth);
        }
    }
}