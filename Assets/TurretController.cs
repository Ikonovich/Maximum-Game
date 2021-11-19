using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;


namespace MaxGame {

    /// <summary>
    /// This class defines a turret controller that controls a base that rotates about the y
    /// axis and a barrel that rotates about the x axis.
    /// </summary>

    public class TurretController : MonoBehaviour {


        [SerializeField, PropertyBackingField(nameof(Turret))]
        private GameObject turret;
        public GameObject Turret { get => turret; set => turret = value; }

        [SerializeField, PropertyBackingField(nameof(Barrel))]
        private GameObject barrel;
        public GameObject Barrel { get => barrel; set => barrel = value; }

        [SerializeField, PropertyBackingField(nameof(Parent))]
        private GameObject parent;
        public GameObject Parent { get => parent; set => parent = value; }

        [SerializeField, PropertyBackingField(nameof(TurretSpeed))]
        private float turretSpeed = 1.0f;
        public float TurretSpeed { get => turretSpeed; set => turretSpeed = value; }

        [SerializeField, PropertyBackingField(nameof(BarrelSpeed))]
        private float barrelSpeed = 1.0f;
        public float BarrelSpeed { get => barrelSpeed; set => barrelSpeed = value; }

        protected GameObject AttackTarget;

        protected Weapon[] Weapons;

        protected Weapon CurrentWeapon;

        protected int ActiveWeapon = 0;
        

        protected GameItem ParentItem;

        protected Transform RotationHelper;


        
        
        // Start is called before the first frame update
        void Start() {

            
            ParentItem = Parent.GetComponent<GameItem>();
            
            RotationHelper = transform.Find("RotationHelper");

            Weapons = this.GetComponentsInChildren<Weapon>();

            CurrentWeapon = Weapons[0];

        }

        // Update is called once per frame
        void Update() {

            
            Vector3 turretRot = Turret.transform.eulerAngles;
            Vector3 barrelRot = Barrel.transform.localEulerAngles;

        

            if ((ParentItem.AttackTarget != null) || (ParentItem.AutoTarget != null)) {

                if (ParentItem.AttackTarget != null) {
                    AttackTarget = ParentItem.AttackTarget;

                }
                else {
                    AttackTarget = ParentItem.AutoTarget;
                }



                //Debug.Log($"Turret " + ParentItem.GetID() + " has target");

                Vector2 rotation = Vector3.zero;

                GameItem targetItem = AttackTarget.GetComponent<GameItem>();

                if ((targetItem != null) && (targetItem.AimingPoint != null)) {

                    AttackTarget = targetItem.AimingPoint.gameObject;
                    rotation = Weapons[ActiveWeapon].GetFiringVector(AttackTarget.transform.position);

                }
                else {

                    rotation = Weapons[ActiveWeapon].GetFiringVector(AttackTarget.transform.position);
                }
 

                //Debug.Log($"Rotating turret " + ParentItem.GetID() + " to " + rotation);

                float turretDifference = rotation.y - turretRot.y;
                float barrelDifference = (rotation.x - barrelRot.x);



                float yRot = Mathf.Sign(turretDifference) * Time.deltaTime * TurretSpeed;
                float xRot = (Mathf.Sign(barrelDifference) * Time.deltaTime * BarrelSpeed);

                //Debug.Log("Turret difference in turret " + ParentItem.GetID() + ": " + turretDifference);
                //Debug.Log("Barrel difference in turret " + ParentItem.GetID() + ": " + barrelDifference);
                

                float AimConstraint = CurrentWeapon.AimConstraint;

                if (Mathf.Abs(turretDifference) > 0.02f) {

                    if (Mathf.Abs(turretDifference) > Mathf.Abs(yRot)) {

                        //Turret.transform.Rotate(new Vector3(0.0f, yRot, 0.0f));


                        float angle = Mathf.LerpAngle(turretRot.y, rotation.y, Time.deltaTime * TurretSpeed);
                        Turret.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);

                        
                        //Vector3 yVec = Vector3.RotateTowards(turretRot, new Vector3(turretRot.x, rotation.y, turretRot.z), 0.1f * TurretSpeed, 1.0f);

                        //Turret.transform.rotation = Quaternion.Euler(yVec);

                    }
                    else {

                        
                        float angle = Mathf.LerpAngle(turretRot.y, rotation.y, Time.deltaTime * TurretSpeed);
                        Turret.transform.eulerAngles = new Vector3(0.0F, angle, 0.0f);

                        //Vector3 yVec = Vector3.RotateTowards(turretRot, new Vector3(turretRot.x, rotation.y, turretRot.z), 0.1f * TurretSpeed, 1.0f);

                        //Turret.transform.rotation = Quaternion.Euler(yVec);


                    }
                }

                if (Mathf.Abs(barrelDifference) > 0.02f) {


                    if (Mathf.Abs(barrelDifference) > Mathf.Abs(xRot)) {

                        //Barrel.transform.Rotate(new Vector3(xRot, 0.0f, 0.0f));

                        float angle = Mathf.LerpAngle(barrelRot.x, rotation.x, Time.deltaTime * BarrelSpeed);
                        Barrel.transform.localEulerAngles = new Vector3(angle, 0.0f, 0.0f);



                    }
                    else {

                        //Barrel.transform.Rotate(new Vector3(barrelDifference, 0.0f, 0.0f));

                        float angle = Mathf.LerpAngle(barrelRot.x, rotation.x, Time.deltaTime * BarrelSpeed);
                    
                        
                        Barrel.transform.localEulerAngles = new Vector3(angle, 0.0f, 0.0f);


                        //Vector3 xVec = Vector3.RotateTowards(new Vector3(barrelRot.x, 0f, 0f), new Vector3(rotation.x, 0f, 0f), 0.1f, 0.1f);

                        //Barrel.transform.localRotation = Quaternion.Euler(xVec);
                    }
                }

                
                float range = (AttackTarget.transform.position - this.transform.position).magnitude;
                if ((((Mathf.Abs(turretDifference) < AimConstraint) && (Mathf.Abs(barrelDifference) < AimConstraint)) && (range < CurrentWeapon.Range) && CurrentWeapon.IsSafe(ParentItem.TeamID))) {
                    
                    if (AttackTarget != null) {
                        CurrentWeapon.Fire(targetItem);
                    }
                }
            } 
            else {

                 
                float angle = Mathf.LerpAngle(turretRot.y, Parent.transform.rotation.eulerAngles.y, Time.deltaTime * TurretSpeed / 10.0f);
                Turret.transform.eulerAngles = new Vector3(0.0F, angle, 0.0f);

                //Debug.Log("New turret angle: " + angle + ". Parent ID: " + ParentItem.GetID());



                angle = Mathf.LerpAngle(barrelRot.x, 0f, Time.deltaTime * BarrelSpeed / 10.0f);
                Barrel.transform.localEulerAngles = new Vector3(angle, 0.0f, 0.0f);



            }
        }
    }
}