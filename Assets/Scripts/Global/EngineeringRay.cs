using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class EngineeringRay : Weapon {

        [SerializeField, PropertyBackingField(nameof(HarvestingSpeed))]
        private int harvestingSpeed = 1;
        public int HarvestingSpeed { get => harvestingSpeed; set => harvestingSpeed = value; }
        

        [SerializeField, PropertyBackingField(nameof(RepairEffect))]
        private StatusEffect repairEffect;
        public StatusEffect RepairEffect { get => repairEffect; set => repairEffect = value; }
        
        protected GameController GameController;

        protected TeamState TeamState;

        protected float FireCountdown = 0.0f;

        protected float FireTime = 3f;

        protected GameItem ParentItem;

        void Awake() {

            FiringEffect = GetComponent<FiringEffect>();

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            
            ParentItem = ParentObj.GetComponent<GameItem>();

            TeamState = GameController.GetTeamState(ParentItem.TeamID);
        }
        void Update() {

            if (FireCountdown > 0) { 
                FireCountdown -= Time.deltaTime;

                if (FireCountdown <= 0) {
                    CeaseFire();
                }
            }

            if (ReloadCountdown > 0) { 
                ReloadCountdown -= Time.deltaTime;
            }
        }
        

        public override void Fire(GameItem targetItem) {

            FireCountdown = FireTime;

            if (ReloadCountdown <= 0) {


                if (targetItem is ResourceDeposit deposit) {  
                    

                    ReloadCountdown = ReloadTime;
                    FiringEffect.Play(targetItem.gameObject);

                    Tuple<ResourceType, int> harvestTuple = deposit.Harvest(HarvestingSpeed);


                    if (harvestTuple.Item2 != 0) {
                       TeamState.UpdateResource(harvestTuple.Item1, harvestTuple.Item2);
                    }
                }

                else if (targetItem.TeamID == ParentItem.TeamID) {

                    if (targetItem.CurrentHealth < targetItem.MaxHealth) {

            
                        ReloadCountdown = ReloadTime;
                        targetItem.AddEffect(RepairEffect);;

                        FiringEffect.Play(targetItem.gameObject);

                    }
                    else {
                        CeaseFire();
                        ParentItem.ClearAutoTarget();
                    }
                }
            }
        }

        public void CeaseFire() {

            FiringEffect.Stop();

        }

        public override Vector2 GetFiringVector(Vector3 targetPoint) {


            TargetHelper.transform.LookAt(targetPoint, Vector3.up);
        
            return new Vector2(TargetHelper.transform.rotation.eulerAngles.x, TargetHelper.transform.rotation.eulerAngles.y);

        }
    }   
}

