using System;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    public class FiringEffect : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(PlayDuration))]
        private float playDuration;
        public float PlayDuration { get => playDuration; set => playDuration = value; }

        protected float PlayCountdown;

        protected bool IsSoundPlayed = false;

        public virtual void Play(GameObject firePoint) {

            ParticleSystem effect = firePoint.GetComponentInChildren<ParticleSystem>();
            effect.Play();

        }


        public virtual void Play(Vector3 startPosition, Vector3 endPosition) {

        }

        public virtual void Play(Vector3 startPosition, Vector3 endPosition, bool repeatSound) {



        }

        public virtual void Stop() {

        }


    }
}