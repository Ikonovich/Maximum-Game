using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame { 

    /// <summary>
    /// This is a generic base class for all world items that have to be interacted 
    /// with by the mouse.
    /// </summary>

    public class InteractionItem : MonoBehaviour {

      
        [HideInInspector]
        public GameObject SelectionEffect;
        
        public float DoubleSelectTime = 0.4f;

        public float DoubleSelectCountdown = 0.0f;

        
        protected float InteractionCountdown = 0.0f;

        protected float InteractionTime = 5.0f;

        public bool IsSelectable = false;

        protected bool IsSelected = false;

        protected GameController GameController;



        public virtual void CursorInteract() {}

        public virtual void Selected() {}

        public virtual void Deselected() {}

    }
}