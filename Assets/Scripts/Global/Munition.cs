using System;
using UnityEngine;
using Candlelight;

namespace MaxGame { 

    public class Munition : MonoBehaviour {

        public int TeamID = 0;

        [SerializeField, PropertyBackingField(nameof(IsInterceptable))]
        private bool isInterceptable = true;
        public bool IsInterceptable { get => isInterceptable; set => isInterceptable = value; }

        [SerializeField, PropertyBackingField(nameof(InterceptTime))]
        private float interceptTime = 0.2f;
        public float InterceptTime { get => interceptTime; set => interceptTime = value; }
        
        


        [SerializeField, PropertyBackingField(nameof(LaunchEffect))]
        private GameObject launchEffect;
        public GameObject LaunchEffect { get => launchEffect; set => launchEffect = value; }
        


        [SerializeField, PropertyBackingField(nameof(ImpactEffect))]
        private GameObject impactEffect;
        public GameObject ImpactEffect { get => impactEffect; set => impactEffect = value; }

        [SerializeField, PropertyBackingField(nameof(StatusEffect))]
        private StatusEffect statusEffect;
        public StatusEffect StatusEffect { get => statusEffect; set => statusEffect = value; }
        

        [SerializeField, PropertyBackingField(nameof(EffectRadius))]
        private float effectRadius = 100.0f;
        public float EffectRadius { get => effectRadius; set => effectRadius = value; }

        [SerializeField, PropertyBackingField(nameof(Force))]
        private float force = 10.0f;
        public float Force { get => force; set => force = value; }
        
        
        protected Rigidbody Body;


        protected GameController GameController;



        void Awake() {

            Body = this.GetComponent<Rigidbody>();

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

        }

        /// <summary>
        /// This method allows the originating object, generally a MunitionDeployer, to
        /// apply an initial launch impulse to the object.
        /// </summary>

        /// <param impulse="A vector 3 containing the desired initial impulse."></param>
        public void ApplyImpulse(Vector3 impulse) {

            Body.AddForce(impulse.z * transform.forward, ForceMode.Impulse);

        }

        public void Launch(Vector3 impulse) {

            if (LaunchEffect != null) {

                Debug.Log($"Playing munition launch effect");

                GameObject tempEffect = Instantiate(LaunchEffect);
                tempEffect.transform.position = this.transform.position;
                tempEffect.GetComponent<WorldEffect>().Trigger();
            }

            ApplyImpulse(impulse);
        }


        /// <summary>
        /// Upon detecting a collision, this method activates the destruction effect,
        /// changes the destruction effect parent to the GameController, and then destroys itself.
        /// </summary>

        public virtual void OnCollisionEnter(Collision collision) {

            Detonate();

        }

        public virtual bool Intercept(bool oneShot) {

            if (IsInterceptable == true) {

                if (oneShot == true) {
                    Detonate();
                    return true;
                }
                else {
                    interceptTime -= Time.deltaTime;

                    if (InterceptTime <= 0f) {

                        Detonate();
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual void Kill() {

            GameObject.Destroy(this.gameObject);

        }

        protected void Detonate() {

            Debug.Log($"Collision detected by munition");


             if (ImpactEffect != null) {
                GameObject tempObject = Instantiate(ImpactEffect);
                
                WorldEffect effect = tempObject.GetComponent<WorldEffect>();
                effect.Trigger();
                tempObject.transform.position = this.transform.position;
            }

            ApplyEffect();

            GameObject.Destroy(this.gameObject);

        }

        /// <summary>
        /// This method effectively "detonates" the munition, applying its effect
        /// to anything within the physics sphere radius.
        /// The effect falls off with the radius. 
        /// </summary>

        protected virtual void ApplyEffect() {

            Debug.Log($"Applying effect method");


            Collider[] colliders = Physics.OverlapSphere(transform.position, EffectRadius);

            foreach (Collider collider in colliders) {

                Debug.Log($"Applying effect to collider");


                if (collider.gameObject.GetComponent<GameItem>() == true) {

                    Debug.Log($"Applying effect to game item");


                    float distanceNormalized = (collider.ClosestPoint(transform.position) - transform.position).magnitude / EffectRadius;
                    float strength = 1f - (distanceNormalized * 0.5f);

                    StatusEffect tempEffect = Instantiate(StatusEffect);
                    tempEffect.Magnitude *= strength;

                    GameItem item = collider.gameObject.GetComponent<GameItem>();
                    Rigidbody body = collider.gameObject.GetComponent<Rigidbody>();

                    body.AddExplosionForce(Force, transform.position, EffectRadius, 0);

                    item.AddEffect(tempEffect);
                }
            }
        }
        
    }
}