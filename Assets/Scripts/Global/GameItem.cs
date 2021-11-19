
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Candlelight;


namespace MaxGame {

    public class GameItem : InteractionItem {

        [SerializeField, PropertyBackingField(nameof(GameID))]
        private int gameID = 0;
        public int GameID { get => gameID; set => gameID = value; }

        [SerializeField, PropertyBackingField(nameof(TeamID))]
        private int teamID = 0;
        public int TeamID { get => teamID; set => teamID = value; }


        [Header("Stats")]
        
        [SerializeField, PropertyBackingField(nameof(MaxHealth))]
        private float maxHealth = 100.0f;
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }

        [SerializeField, PropertyBackingField(nameof(CurrentHealth))]
        private float currentHealth = 100.0f;
        public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

        [SerializeField, PropertyBackingField(nameof(HasShields))]
        private bool hasShields;
        public bool HasShields { get => hasShields; set => hasShields = value; }

        [SerializeField, PropertyBackingField(nameof(MaxShieldStrength))]
        private float maxShieldStrength;
        public float MaxShieldStrength { get => maxShieldStrength; set => maxShieldStrength = value; }

        [SerializeField, PropertyBackingField(nameof(CurrentShieldStrength))]
        private float currentShieldStrength;
        public float CurrentShieldStrength { get => currentShieldStrength; set => currentShieldStrength = value; }
        
        
        [SerializeField, PropertyBackingField(nameof(Name))]
        private String name = "No Name";
        public String Name { get => name; set => name = value; }
        
        [Header("Build Attributes")]
        [SerializeField, PropertyBackingField(nameof(EnergyCost))]
        private int energyCost = 100;
        public int EnergyCost { get => energyCost; set => energyCost = value; }

        [SerializeField, PropertyBackingField(nameof(ElementsCost))]
        private int elementsCost = 75;
        public int ElementsCost { get => elementsCost; set => elementsCost = value; }

        [SerializeField, PropertyBackingField(nameof(CarbonCost))]
        private int carbonCost = 50;
        public int CarbonCost { get => carbonCost; set => carbonCost = value; }

        [SerializeField, PropertyBackingField(nameof(BuildOffset))]
        private Vector3 buildOffset = Vector3.zero;
        public Vector3 BuildOffset { get => buildOffset; set => buildOffset = value; }
        

        [SerializeField, PropertyBackingField(nameof(BuildTime))]
        private float buildTime = 1.0f;
        public float BuildTime { get => buildTime; set => buildTime = value; } 

        [SerializeField, PropertyBackingField(nameof(MapSize))]
        private int mapSize = 2;
        public int MapSize { get => mapSize; set => mapSize = value; }


        [SerializeField, PropertyBackingField(nameof(IsActive))]
        private bool isActive = true;
        public bool IsActive { get => isActive; set => isActive = value; }

        
        [Header("Item Objects")]

        [SerializeField, PropertyBackingField(nameof(RadialMenu))]
        private GameObject radialMenu;
        public GameObject RadialMenu { get => radialMenu; set => radialMenu = value; }
        

        [SerializeField, PropertyBackingField(nameof(DestructionEffect))]
        private GameObject destructionEffect;
        public GameObject DestructionEffect { get => destructionEffect; set => destructionEffect = value; }

        [SerializeField, PropertyBackingField(nameof(AimingPoint))]
        private GameObject aimingPoint;
        public GameObject AimingPoint { get => aimingPoint; set => aimingPoint = value; }


        [SerializeField, PropertyBackingField(nameof(PercentBuilt))]
        private float percentBuilt;
        public float PercentBuilt { get => percentBuilt; set => percentBuilt = value; }
        
        protected GameObject MapIcon;
        
        [HideInInspector]
        public ScanNode ScanNode;

        public GameObject AutoTarget;
        
        public GameObject AttackTarget;
        
        [HideInInspector]
        public Vector3 TargetPoint;

        
        [HideInInspector]
        public NavMeshPath Path;
        
        [HideInInspector]
        public int PathIndex = 0;
    
        protected List<StatusEffect> StatusEffects;

        protected Dictionary<ResourceType, int> Cost;

        protected TeamState TeamState;

        protected List<Color> Colors;

        protected List<Material> Materials;

        protected bool IsRegistered = false;

        protected bool MaterialsSet = false;
        
        protected bool IsDetected = false;

        void Awake() {


        }

        void Start() {

        }

        protected virtual void Initialize() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();
            StatusEffects = new List<StatusEffect>();

            MapIcon = GetComponentInChildren<MapIcon>(true).gameObject;
        
        }

        public override void CursorInteract() {

            if (RadialMenu != null) {
                ShowMenu();
            }
        }

        public override void Selected() {

            Debug.Log($"Selected");
        }

        public override void Deselected() {

        }

        public void Detected() {

            MapIcon.SetActive(true);
            IsDetected = true;

        }

        public void Undetected() {

            MapIcon.SetActive(false);
            IsDetected = true;


        }

        public void LOSDetected() {


        }

        public void LOSUndetected() {


        }
        
        public virtual void SetTarget(GameItem target) {

            Debug.Log($"Target object set in generic item");

        }

        public virtual void SetTarget(GameObject target) {

            Debug.Log($"Target object set in generic item");

        }

            
        public virtual void SetTarget(Vector3 target) {
            
            Debug.Log($"Target object set in generic item");

        }

        public virtual void SetAutoTarget(GameObject target) {

            Debug.Log($"Auto target set in item: " + GameID);

            AutoTarget = target;
        }

        public virtual void ClearTarget() {


            Debug.Log($"Unsetting target point in unit.");

            AttackTarget = null;
            TargetPoint = Vector3.zero;

        }

        public virtual void ClearAutoTarget() {

            Debug.Log($"Clearing auto target in unit.");


            AutoTarget = null;
        }



        public Dictionary<ResourceType, int> GetCost() {

            Dictionary<ResourceType, int> cost = new Dictionary<ResourceType, int>();

            cost.Add(ResourceType.Energy, EnergyCost);
            cost.Add(ResourceType.Elements, ElementsCost);
            cost.Add(ResourceType.Carbon, CarbonCost);

            return cost;
        }


    
        public void Register() {

            GameID = GameController.Register(this);
            IsRegistered = true;

        }
        
        public virtual void RemoveFromRegister() {

            GameController.RemoveFromRegister(GameID);
        }

        public void ShowMenu() {

            RadialMenu menu = RadialMenu.GetComponent<RadialMenu>();

            RadialMenu.SetActive(true);
            menu.OnInteract();
        }

        public void HideMenu() {

            RadialMenu menu = RadialMenu.GetComponent<RadialMenu>();
            menu.Close(menu.InterpolateSpeed);
        }

        public int GetID() {

            return GameID;
        }

        /// <summary>
        /// This applies damage, removing strength from the shield before effecting health.
        /// </summary>

        public void TakeDamage(float damageIn) {

            Debug.Log($"Taking damage of " + damageIn);

            float damage = damageIn;

            if ((HasShields == true) && (CurrentShieldStrength > 0)) {

                if (CurrentShieldStrength > damage) {

                    CurrentShieldStrength -= damage;
                }
                else {
                    damage -= CurrentShieldStrength;
                    CurrentShieldStrength = 0.0f;
                    TakeDirectDamage(damage);
                }
            }
            else {
                TakeDirectDamage(damage);
            }
        }

        /// <summary>
        /// This applies damage directly to the health, ignoring any shield effects.
        /// It is also used by TakeDamage to apply damage if the shields have failed/none exist.
        /// </summary>

        public void TakeDirectDamage(float damageIn) {

            Debug.Log($"Taking direct damage of " + damageIn);


            CurrentHealth -= damageIn;

            if (CurrentHealth <= 0) {

                Destroyed();
            }
        }



        /// <summary>
        /// Every update cycle this class processes the status effects operating on the unit.
        /// When statusEffect.UpdateEffect() is called, it processes the status effect and
        /// returns it's remaining lifetime. If that lifetime is below zero, the game item
        /// removes the process.
        /// </summary>

        protected virtual void ProcessEffects() {

            //Debug.Log($"Number of status effects to process by ID " + GameID + ": " + StatusEffects.Count);


            for (int i = 0; i < StatusEffects.Count; i++)
            {
                StatusEffect effect = StatusEffects[i];

                //Debug.Log($"Processing status effect");

                if (effect.UpdateEffect() <= 0.0f) {

                    effect.EndEffect();
                    StatusEffects.RemoveAt(i);
                }
            }
        }



        public virtual void AddEffect(StatusEffect effect) {


            effect.Parent = this;
            StatusEffects.Add(effect);
            Debug.Log($"Status effect added to ID " + GameID);

        }

        public void Repair(int healthIn) {     

            CurrentHealth += healthIn;
            if (CurrentHealth > MaxHealth) {
                CurrentHealth = MaxHealth;
            }
    
            if (PercentBuilt < 1.0) {

                PercentBuilt += ((float)healthIn / (float)MaxHealth);
                if (PercentBuilt >= 1.0f) {
                    PercentBuilt = 1.0f;
                    IsActive = true;

                    TurretController turretController = GetComponent<TurretController>();

                    if (turretController != null) {

                        turretController.enabled = true;
                    }
                }

                SetTransparency();
            } 
        }

        public void Destroyed() {

            Debug.Log($"Item destroyed by damage.");
            
            GameController.RemoveFromRegister(GameID);
        
            if (ScanNode != null) {
                ScanNode.RemoveItem(this);
            }


            
            if (DestructionEffect != null) {
                GameObject tempObject = Instantiate(DestructionEffect);
                
                WorldEffect effect = tempObject.GetComponent<WorldEffect>();
                effect.Trigger();
                tempObject.transform.position = this.transform.position;
            }

            if (GetComponent<TurretController>() != null) {

                GetComponent<TurretController>().enabled = false;
            }
            if (GetComponent<NavMeshAgent>() != null) {

                GetComponent<NavMeshAgent>().enabled = false;
            }

            if (this is Unit unit) {

                if (unit.GetFormation() != null) { 
                    unit.GetFormation().Remove(unit);
                }
            }

            Destroy(this.gameObject, 0.5f);

        }

        public void SetTransparency() {


            
            float alpha = 0.0f;

            if (PercentBuilt == 1.0f) {

                alpha = 1.0f;
            }
            else {
                alpha = 0.1f + (PercentBuilt / 2f);
            }

            

            if (MaterialsSet == false) {

                Colors = new List<Color>();

                MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
                Materials = new List<Material>();

                for (int i = 0; i < renderers.Length; i++) {

                    
                    Material material = renderers[i].material;
                    Materials.Add(material);
                    Colors.Add(material.GetColor("_Color"));

                    Debug.Log($"Colors size in loop: " + Colors.Count);

                    
                    material.SetOverrideTag("RenderType", "Fade");
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                }
                
                MaterialsSet = true;
            }

            Debug.Log($"Colors size after loop: {Colors.Count} Renderers count: {Materials}");


            for (int i = 0; i < Colors.Count; i++) {
                
                Color color = Colors[i];
                Material tempMaterial = Materials[i];
                
                tempMaterial.SetColor("_Color", new Color(color.r * PercentBuilt, color.g * PercentBuilt, color.b, alpha));

                if (PercentBuilt == 1.0f) {

                    tempMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                }
            }
        }



        /// <summary>
        /// Every update cycle this class processes the status effects operating on the unit.
        /// When statusEffect.UpdateEffect() is called, it processes the status effect and
        /// returns it's remaining lifetime. If that lifetime is below zero, the game item
        /// removes the process.
        /// </summary>

        // protected virtual void ProcessEffects() {

        //     Debug.Log($"Number of status effects to process: " + StatusEffects.Count);


        //     foreach (StatusEffect effect in StatusEffects) {

        //         Debug.Log($"Processing status effect");


        //         if (effect.UpdateEffect() <= 0.0f) {

        //             effect.EndEffect();
        //             StatusEffects.Remove(effect);
        //         }
        //     }
        // }
    }
}