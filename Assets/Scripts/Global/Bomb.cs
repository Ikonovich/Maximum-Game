using System;
using UnityEngine;
using Candlelight;

namespace MaxGame { 

    public class Bomb : Munition {



        protected void Awake() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

        }
    }
}