using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace MaxGame {

    public class QueueButton : MenuButton {

        protected float Countdown = 0.0f;

        float BuildCountdown = 0f;

        void Awake() {

            Border = GetComponent<Image>();

            BorderColor = Border.color;

            Background = GetComponentInChildren<RawImage>(true);

        }
        
        public void SetTimer(float time) {

            BuildCountdown = time;
        }

        public bool Decrement() {

            BuildCountdown -= Time.deltaTime;

            if (BuildCountdown <= 0f) {
                BuildCountdown = -1f;
                return true;
            }
            return false;
        }

    }

}