using System;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    public class WorldButton : InteractionItem {

        [SerializeField, PropertyBackingField(nameof(Parent))]
        private GameObject parent;
        public GameObject Parent { get => parent; set => parent = value; }

        protected GameObject Player;

    }
}