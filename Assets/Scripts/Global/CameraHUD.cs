using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Candlelight;

namespace MaxGame {

    public class CameraHUD : MonoBehaviour {

        
        [SerializeField, PropertyBackingField(nameof(PowerCounterObject))]
        private GameObject powerCounterObject;
        public GameObject PowerCounterObject { get => powerCounterObject; set => powerCounterObject = value; }

        protected GameController GameController;

        protected TeamState TeamState;

        protected TextMeshProUGUI PowerCounter;

        protected TextMeshProUGUI CarbonCounter;

        protected TextMeshProUGUI ElementsCounter;

        protected TextMeshProUGUI UnitCounter;

        protected int TeamID;



        protected void Start() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            
            TeamID = GameController.GetOverhead().TeamID;
            TeamState = GameController.GetTeamState(TeamID);

            GameObject tempObject = transform.Find("PowerCounter/PowerText").gameObject;
            PowerCounter = PowerCounterObject.GetComponent<TextMeshProUGUI>();
            
            tempObject = transform.Find("ElementsCounter/ElementsText").gameObject;
            ElementsCounter = tempObject.GetComponent<TextMeshProUGUI>();

            tempObject = transform.Find("CarbonCounter/CarbonText").gameObject;
            CarbonCounter = tempObject.GetComponent<TextMeshProUGUI>();

            tempObject = transform.Find("UnitCounter/UnitText").gameObject;
            UnitCounter = tempObject.GetComponent<TextMeshProUGUI>();

        }

        protected void Update() {

            Dictionary<ResourceType, int> resourceDict = TeamState.ResourceDict;

            PowerCounter.text = resourceDict[ResourceType.Energy].ToString();

            ElementsCounter.text = resourceDict[ResourceType.Elements].ToString();

            CarbonCounter.text = resourceDict[ResourceType.Carbon].ToString();

            UnitCounter.text = TeamState.GetUnitCount().ToString();
        }
    }
}