using System;
using UnityEngine;
using TMPro;
using Candlelight;



namespace MaxGame {

    public class ResourceDeposit : GameItem {

        [SerializeField, PropertyBackingField(nameof(ResourceType))]
        private ResourceType resourceType = ResourceType.Energy;
        public ResourceType ResourceType { get => resourceType; set => resourceType = value; }
        
        [SerializeField, PropertyBackingField(nameof(ResourceRichness))]
        private ResourceRichness resourceRichness = ResourceRichness.Normal;
        public ResourceRichness ResourceRichness { get => resourceRichness; set => resourceRichness = value; }
        
        [SerializeField, PropertyBackingField(nameof(Amount))]
        private int amount;
        public int Amount { get => amount; set => amount = value; }

        protected Camera OverheadCamera;

        protected TextMeshProUGUI ResourceDisplay;

        protected TextMeshProUGUI ResourceCounter;

        protected float CounterInitialPosition = 0f;
        public float CounterSpeed = 30.0f;

        protected float ShowCountdown = 0.0f;

        protected float ShowTimer = 1.0f;

        protected float HarvestCountdown = 0.0f;

        protected float HarvestTimer = 2.0f;

        void Start() {

            ResourceDisplay = transform.Find("ResourceDisplay/Text").GetComponent<TextMeshProUGUI>();

            ResourceCounter = transform.Find("ResourceCounter/Text").GetComponent<TextMeshProUGUI>();

            ResourceDisplay.enabled = false;
            ResourceCounter.enabled = false;

            Initialize();

        }

        protected override void Initialize() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

    
            OverheadCamera = GameController.GetOverhead().gameObject.GetComponent<Camera>();
            Register();

            MapIcon = GetComponentInChildren<MapIcon>(true).gameObject;

            

        }

        void Update() {

            ResourceDisplay.text = Amount.ToString();

            ResourceCounter.transform.localScale = new Vector3(Mathf.Sqrt(OverheadCamera.transform.position.y / 300f), Mathf.Sqrt(OverheadCamera.transform.position.y / 300f), 1f);
            ResourceDisplay.transform.localScale = new Vector3(Mathf.Sqrt(OverheadCamera.transform.position.y / 300f), Mathf.Sqrt(OverheadCamera.transform.position.y / 300f), 1f);

            ResourceCounter.transform.parent.LookAt(OverheadCamera.transform);
            ResourceDisplay.transform.parent.LookAt(OverheadCamera.transform);


            if (ShowCountdown > 0) {

                ResourceCounter.transform.Translate(new Vector3(0f, CounterSpeed * Time.deltaTime, 0f));
                ShowCountdown -= Time.deltaTime;

                if (ShowCountdown <= 0) {

                    HideCount();
                }
            }

            if (HarvestCountdown > 0) {

                HarvestCountdown -= Time.deltaTime;

            }
        }

        /// <summary>
        /// The method is called by any entity harvesting from
        /// this deposit.
        /// Returns a 2-tuple containing the resource type
        /// and amount harvested from the deposit.
        /// </summary>
        public Tuple<ResourceType, int> Harvest(int multiplier) {

            Tuple<ResourceType, int> amountTuple = new Tuple<ResourceType, int>(ResourceType, 0);


            if (HarvestCountdown <= 0) {

                HarvestCountdown = HarvestTimer;

                int harvestAmount = multiplier * (int)ResourceRichness;

                if (harvestAmount > Amount) {

                    ShowCount(Amount);
                    Exhausted();
                    amountTuple = new Tuple<ResourceType, int>(ResourceType, Amount);
                }
                else {
                    
                    ShowCount(harvestAmount);
                    Amount -= harvestAmount;
                    amountTuple = new Tuple<ResourceType, int>(ResourceType, harvestAmount);

                }
            }

            return amountTuple;
        }

        protected void ShowCount(int value) {

            ResourceCounter.text = value.ToString();
            ResourceCounter.enabled = true;
            ShowCountdown = ShowTimer;
        }

        protected void HideCount() {

            ResourceCounter.enabled = false;
            ResourceCounter.transform.localPosition = new Vector3(0f, CounterInitialPosition, 0f);

        }

        protected void Exhausted() {

            Destroy(this.gameObject, 2f);
        }

        public override void Selected() {

            ResourceDisplay.enabled = true;
        }

        public override void Deselected() {

            ResourceDisplay.enabled = false;

            Debug.Log("Resource deposit selected.");

        }

        public override void AddEffect(StatusEffect effect) {

            // Has no effect for this item
        }


    }
}