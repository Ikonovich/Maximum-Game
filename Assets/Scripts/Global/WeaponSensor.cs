using System;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    public class WeaponSensor : MonoBehaviour {

        [SerializeField, PropertyBackingField(nameof(GameItem))]
        private GameObject gameItem;
        public GameObject GameItem { get => gameItem; set => gameItem = value; }
        

        public void OnTriggerEnter(Collider collider) {

            Munition munition = collider.GetComponent<Munition>();

            GameItem item = GameItem.GetComponent<GameItem>();

            Debug.Log($"Interceptor sensor triggered");


            if (munition != null) {

                if (munition.TeamID != item.TeamID) {

                    Debug.Log($"Sensor detected munition");

                    item.SetTarget(munition.gameObject);
                }


            }
        }
    }
}