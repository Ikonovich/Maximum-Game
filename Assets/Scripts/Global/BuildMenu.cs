using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;



namespace MaxGame {


    public class BuildMenu : MonoBehaviour {

        protected UIDocument MenuDocument;

        protected VisualElement Menu;

        protected void Start() {

            Transform temp = transform.Find("BuildMenu");
            MenuDocument = temp.gameObject.GetComponent<UIDocument>();

            Menu = MenuDocument.rootVisualElement;
        }

        protected void Open() {

            PopulateMenu();
        
        }

        protected void Close() {

        }

        protected void PopulateMenu() {


        }

    }

}