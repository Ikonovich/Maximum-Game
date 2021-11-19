using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace MaxGame {

    public class MenuButton : Button, IPointerExitHandler, IPointerEnterHandler {

        protected Image Border;
        
        protected Color BorderColor;
        protected RawImage Background;

        protected string ItemName;



        void Awake() {

            Border = GetComponent<Image>();

            BorderColor = Border.color;

            Background = GetComponentInChildren<RawImage>(true);

        }

        public void SetItem(string itemName) {

            Background = GetComponentInChildren<RawImage>(true);


            Debug.Log("Getting item: " + itemName);

            ItemName = itemName;

            Texture2D texture = (Texture2D)Resources.Load<Texture2D>(ItemName + "Image");

            if (texture == null) {

                Debug.Log($"Looked for: " + ItemName + "Image but did not find it");
            }
            else if (Background == null) {

                Debug.Log($"Button background is null");

            }


            Background.texture = texture;

        }

        public string GetItem() {

            return ItemName;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            
            Border.color = Color.cyan;

            Debug.Log("Pointer entered button");
        }

        public void OnPointerExit(PointerEventData eventData) {

            Border.color = BorderColor;
        }

        public void OnPointerClick(PointerEventData eventData) {

            Border.color = Color.green;
            Debug.Log("Menu button called OnPointerClick");

        }

        public void OnClick() {

            Debug.Log("Menu button called OnClick");

        }



    }

}