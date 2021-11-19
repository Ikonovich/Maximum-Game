using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace MaxGame {

    public class BuildButton : MenuButton {


        void Awake() {

            Border = GetComponent<Image>();

            BorderColor = Border.color;

            Background = GetComponentInChildren<RawImage>(true);

        }

    }

}