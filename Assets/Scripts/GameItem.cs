
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;


namespace MaxGame {

    public class GameItem : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(TeamID))]
        private int teamID = 0;
        public int TeamID { get => teamID; set => teamID = value; }
    
        protected int GameID;


        void Start() {

            Initialize();
        }

        protected virtual void Initialize() {

        }

        public virtual void CursorInteract() {

            
        }

        public virtual void Selected() {

            Debug.Log($"Selected");
        }

        public virtual void Deselected() {

        }


    
        protected virtual void Register() {

            //GameID = GameController.Register(this);

        }
        
        protected virtual void RemoveFromRegister() {

            //GameController.RemoveFromRegister(GameID);
        }
    }
}