using System;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    public enum StatusType {
        Damage,
        Repair,
        Shield,
        Speed
    }


    [CreateAssetMenu(fileName = "StatusEffect", menuName = "StatusEffects", order = 1)]
    public class StatusEffect : ScriptableObject {

        /// <summary>
        /// Indicates the type of the effect caused.
        /// </summary>

        [SerializeField, PropertyBackingField(nameof(StatusType))]
        private StatusType statusType = StatusType.Damage;
        public StatusType StatusType { get => statusType; set => statusType = value; }
        

        /// <summary>
        /// When IsInstantaneous is true, the effect is applied all at one, with no time variation,
        /// regardless of other settings.
        /// </summary>

        [SerializeField, PropertyBackingField(nameof(IsInstantaneous))]
        private bool isInstantaneous = true;
        public bool IsInstantaneous { get => isInstantaneous; set => isInstantaneous = value; }
        

        /// <summary>
        /// The strength of the effect in numbers, either per second or instantly depending on 
        /// whether or not IsInstantaneous is true.
        /// Health and shield effects are denoted in terms of actual number removed.
        /// Other effects are denoted in terms of percentage. For these, applying them
        /// instantaneously will have relatively little effect.
        /// </summary>

        [SerializeField, PropertyBackingField(nameof(Magnitude))]
        private float magnitude;
        public float Magnitude { get => magnitude; set => magnitude = value; }


        /// <summary>
        /// This field denotes the percentage that the effect decays each second. 
        /// 1 indicates 100% decay after 1 second. Higher numbers indicate faster decay.
        /// </summary>
        [SerializeField, PropertyBackingField(nameof(Decay))]
        private float decay = 0.0f;
        public float Decay { get => decay; set => decay = value; }

        /// <summary>
        /// This field denotes the time that the effect will last. Note that the effect may be operating 
        /// without change due to the decay.  
        /// </summary>
        [SerializeField, PropertyBackingField(nameof(Duration))]
        private float duration = 1.0f;
        public float Duration { get => duration; set => duration = value; }

        public GameItem Parent;


        public void StartEffect() {


        }

        /// <summary>
        /// This conducts the status effect operations and returns the remaining lifetime of the status
        /// effect. If the remaining lifetime is below zero, the parent game item calls EndEffect().
        /// </summary>
        public float UpdateEffect() {

            Debug.Log($"Status effect being updated");

            Duration -= Time.deltaTime;

            if (StatusType == StatusType.Damage) {

                if (IsInstantaneous) {
                    
                    Parent.TakeDamage(Magnitude);
                    return 0.0f;
                }
                else {
                    Parent.TakeDamage(Magnitude * Time.deltaTime);
                }

            }
            else if (StatusType == StatusType.Repair) {

                if (IsInstantaneous) {

                    Parent.Repair((int)Magnitude);
                }
                else {

                    Parent.Repair((int)(Magnitude * Time.deltaTime));
                }
                    
            }


 
            return Duration;
            
        }

        /// <summary>
        /// If the effect reverts upon completion, restores the original effects. 
        /// </summary>

        public void EndEffect() {

        }
        
    }
}