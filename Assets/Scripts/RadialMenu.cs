using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    /// <summary>
    /// This is a dynamically rendered and resized 3D radial world menu.
    /// It dynamically moves it's buttons to face the player, and resizes them based on the player distance.
    ///
    /// The menu can display up to 6 prefab buttons.
    ///</summary>

    
    public class RadialMenu : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(ButtonOne))]
        private GameObject buttonOne;
        public GameObject ButtonOne { get => buttonOne; set => buttonOne = value; }

        [SerializeField, PropertyBackingField(nameof(ButtonTwo))]
        private GameObject buttonTwo;
        public GameObject ButtonTwo { get => buttonTwo; set => buttonTwo = value; }
    

        [SerializeField, PropertyBackingField(nameof(ButtonThree))]
        private GameObject buttonThree;
        public GameObject ButtonThree { get => buttonThree; set => buttonThree = value; }
    

        [SerializeField, PropertyBackingField(nameof(ButtonFour))]
        private GameObject buttonFour;
        public GameObject ButtonFour { get => buttonFour; set => buttonFour = value; }
    

        [SerializeField, PropertyBackingField(nameof(ButtonFive))]
        private GameObject buttonFive;
        public GameObject ButtonFive { get => buttonFive; set => buttonFive = value; }
    

        [SerializeField, PropertyBackingField(nameof(ButtonSix))]
        private GameObject buttonSix;
        public GameObject ButtonSix { get => buttonSix; set => buttonSix = value; }

        protected List<GameObject> ButtonList;

        protected bool IsOpen = false;
    
    

        void Start() {

            ButtonList = new List<GameObject>();

            if (ButtonOne != null) {

                ButtonList.Add(ButtonOne);
            }
            if (ButtonTwo != null) {

                ButtonList.Add(ButtonTwo);
            }
            if (ButtonThree != null) {

                ButtonList.Add(ButtonThree);
            }
            if (ButtonFour != null) {

                ButtonList.Add(ButtonFour);
            }
            if (ButtonFive != null) {

                ButtonList.Add(ButtonFive);
            }
            if (ButtonSix != null) {

                ButtonList.Add(ButtonSix);
            }
        }


        void Update() {

            


        }

        public void OpenMenu() {


        }

        public void CloseMenu() {

        }
    }
}