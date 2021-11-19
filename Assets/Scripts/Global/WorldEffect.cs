using System;
using UnityEngine;


namespace MaxGame {

    public class WorldEffect : MonoBehaviour {



        protected bool IsTriggered = false;

        protected ParticleSystem Effect;

        protected AudioSource Sound;


        protected void Update() {

        }

        /// <summary>
        /// Triggers the destruction effect, calls destroy with the effect
        /// duration as a delay, and returns the destruction delay.
        /// </summary>

        public float Trigger() {

            Debug.Log($"World effect triggered");

            
            Effect = GetComponent<ParticleSystem>();
            Sound = GetComponent<AudioSource>();
            
        

            float soundTime = 1.0f;
            float visualTime = 0.0f;

            if (Sound != null) {

                Sound.Play();

            }
            if (Effect != null) {
                
                Effect.Play();
                visualTime = Effect.main.duration;
            }

            IsTriggered = true;

            GameObject.Destroy(this.gameObject, Mathf.Max(visualTime, soundTime));

            return visualTime;
        }



    }
}