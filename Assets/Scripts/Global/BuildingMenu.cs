
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

using Candlelight;



namespace MaxGame {

    public class BuildingMenu : MonoBehaviour {

        protected GameController GameController;

        protected GameObject Container;

        protected Building Parent;

        protected List<BuildButton> ButtonList;

        protected List<QueueButton> Queue;

        protected QueueButton QueueButton;

        protected bool IsOpen = false;

        protected int Page = 0;

        protected float ClickTime = 0.1f;

        protected float ClickCountdown = 0.0f;

        protected InputAction Exit;

        

        void Awake() {

            GameObject controllerObject = GameObject.Find("GameController");
            GameController = controllerObject.GetComponent<GameController>();

            Container = transform.Find("Container").gameObject;

            Parent = transform.parent.gameObject.GetComponent<Building>();

            Queue = new List<QueueButton>();
            QueueButton = GetComponentInChildren<QueueButton>(true);

            ButtonList = new List<BuildButton>();
            BuildButton[] buildButtons = GetComponentsInChildren<BuildButton>(true);
            
            //Debug.Log("Num of build buttons: " + buildButtons.Length);

            int i = 0;
            foreach (BuildButton button in buildButtons) {
                
                i++;

                button.onClick.AddListener(delegate{MenuButtonClicked(button);});

                ButtonList.Add(button);
                //Debug.Log("Build menu acquired button: " + i);

            }
        }

        void Update() {

            ProcessQueue();

            if (ClickCountdown > 0) {

                ClickCountdown -= Time.deltaTime;
            }
        }

        protected void ProcessQueue() {

            if (Queue.Count > 0) {

                QueueButton button = Queue[0];

                if (button.Decrement() == true) {

                    if (Parent.BuildItem(button.GetItem()) == true) {
                        Queue.RemoveAt(0);
                        GameObject.Destroy(button.gameObject);
                        PopulateQueue();

                    }
                }
            }
        }

        protected void Populate() {

            List<string> items = GameController.CheckBuildable(Parent.GetBuildable(), Parent.TeamID);

            int itemNum = 9 * Page;

            Debug.Log("Build menu populating.");

            for (int i = 0; i < ButtonList.Count; i++) {

                Debug.Log("Button num: " + i);

                BuildButton tempButton = ButtonList[i];

                if (itemNum < items.Count) {

                    tempButton.onClick.AddListener(() => MenuButtonClicked(tempButton));

                    tempButton.gameObject.SetActive(true);
                    tempButton.SetItem(items[i]);
                    itemNum++;
                }
                else {

                    tempButton.gameObject.SetActive(false);
                    itemNum++;
                }
            }
        }

        public void Test() {

            Debug.Log($"OnClick called test");

        }

        protected void PopulateQueue() {

            for (int i = 0; i < Queue.Count; i++) {

                Debug.Log($"Queue button populating");

                QueueButton tempButton = Queue[i];


                tempButton.transform.localPosition = QueueButton.transform.localPosition + new Vector3(25f * i, 0f, 0f);
                tempButton.transform.SetSiblingIndex(Queue.Count - (i + 1));
                tempButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// This method duplicates the base QueueButton object
        /// and adds the copy to the Queue, setting the background
        /// image to the one associated with the itemID.
        /// </summary>
        protected void Enqueue(string itemID) {

            GameObject itemObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);
            GameItem item = itemObject.GetComponent<GameItem>();

            GameObject buttonObj = Instantiate(QueueButton.gameObject);
            QueueButton tempButton = buttonObj.GetComponent<QueueButton>();
            buttonObj.transform.parent = QueueButton.transform.parent;
            tempButton.SetItem(itemID);
            tempButton.SetTimer(item.BuildTime);


            tempButton.onClick.AddListener(() => QueueButtonClicked(tempButton));

            Queue.Add(tempButton);
            PopulateQueue();


        }

        protected void Dequeue() {
            
            BuildButton button = Queue[0].GetComponent<BuildButton>();

            if  (Parent.BuildItem(button.GetItem()) == true) {

                Queue.RemoveAt(0);

                PopulateQueue();
            }
            else {
                Debug.Log("Cannot dequeue, likely because all spawn areas are blocked.");
            }

        }

        protected void MenuButtonClicked(BuildButton button) {

            Debug.Log($"Build button clicked " + nameof(button));


            if (ClickCountdown <= 0) {

                ClickCountdown = ClickTime;

                Debug.Log($"Button clicked " + nameof(button));

                string itemID = button.GetItem();


                GameObject originalObject = GameObject.Find("/GameController/PreviewArea/Items/" + itemID);

                GameItem gameItem = (GameItem)originalObject.GetComponent<GameItem>();
                
                Dictionary<ResourceType, int> cost = gameItem.GetCost();
                Building building = Parent.GetComponent<Building>();

                TeamState teamState = GameController.GetTeamState(building.TeamID);

                if (teamState.Purchase(cost) == true) {
                    
                    Debug.Log($"Adding  " + itemID + "to queue.");

                    Enqueue(itemID);
                }
            }
        }

         protected void QueueButtonClicked(QueueButton button) {

            Debug.Log($"Queue button clicked " + nameof(button));


            if (ClickCountdown <= 0) {

                ClickCountdown = ClickTime;

            Queue.Remove(button);
            GameObject.Destroy(button.gameObject);
            PopulateQueue();

            Debug.Log($"Item removed from queue.");

            }
        }

        public void OnEscape(InputValue input) {
            
            if (IsOpen == true) {
                CloseMenu();
            }
        }



        public void Toggle() {

            if (IsOpen == true) {
                CloseMenu();
            }
            else {
                OpenMenu();
            }
        }

        public void CloseMenu() {

            Container.SetActive(false);
            IsOpen = false;

        }

        public void OpenMenu() {

            IsOpen = true;
            Container.SetActive(true);
            Populate();
        }
    }
}