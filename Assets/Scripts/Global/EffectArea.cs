using System;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    public class EffectArea : MonoBehaviour {


        protected Collider Collider; 
        protected void Start() {


            Collider = GetComponent<Collider>();

        }
    }
}