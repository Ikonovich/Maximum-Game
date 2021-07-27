using System;
using UnityEngine;
using UnityEngine.UIElements;



    public class ImageElement : VisualElement {

        public Image Texture;
        public string ItemID = "None";


        public ImageElement() {

            Texture = new Image();
            Add(Texture);

            //Texture.AddToClassList("image");
            //AddToClassList("imageElement");

        }

        public void SetItemID(string stringID) {

            ItemID = stringID;
        }

        public void SetTexture(Texture texture) {

            Texture.image = texture;

            Debug.Log($"Setting image texture");
        }

        #region UXML
        public new class UxmlFactory : UxmlFactory<ImageElement, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits {}
        #endregion



}