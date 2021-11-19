using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;



namespace MaxGame {

    /// <summary>
    /// This defines a deployer for a projectile weapon with an artillery, cannon, or realistic
    /// gun-like firing pattern. 
    /// </summary>

    public class ShellDeployer : MunitionDeployer { 

		
        public List<GameObject> FiringPoints;

		public int CurrentFiringPoint = 0;

		public override void Fire(GameItem target) {

            if (ReloadCountdown <= 0) { 

                ReloadCountdown = ReloadTime;

                
                GameObject newMunition = Instantiate(Munition);
                Munition  munition = newMunition.GetComponent<Munition>();
                GameItem parent = ParentObj.GetComponent<GameItem>();

                newMunition.transform.position = FiringPoints[CurrentFiringPoint].transform.position;
                newMunition.transform.rotation = FiringPoints[CurrentFiringPoint].transform.rotation;


                newMunition.transform.Translate(FiringOffset);
                munition.ApplyImpulse(LaunchImpulse);
				
				FiringEffect.Play(FiringPoints[CurrentFiringPoint]);

				
                CurrentFiringPoint += 1;

                
                if (CurrentFiringPoint == FiringPoints.Count) {

                    CurrentFiringPoint = 0;
                }
				

            }
            else {

                ReloadCountdown -= Time.deltaTime;
            }
		}

        public override Vector2 GetFiringVector(Vector3 targetPoint) {

			
			// Solving for the angle the turret needs to be at to hit the target.
			// Uses the formula theta = arctan((v^2 +- sqrt(v^4 - g(gd^2 + 2hv^2))) / gd)
			// Where v = velocity, g = gravity, d = distance, and h = height.

            TargetHelper.transform.LookAt(targetPoint, Vector3.up);

			// float velocity = LaunchImpulse.z;
			// float gravity = 9.8f;
			// float distance = Vector3.Distance(transform.position, targetPoint);
			// float height = targetPoint.y;


			// float formulaInner = Mathf.Pow(velocity, 4) - gravity * ((gravity * distance * distance) + (2 * height * velocity * velocity));
			// float formulaOuter = (velocity * velocity - Mathf.Sqrt(formulaInner)) / (gravity * distance);

			// Debug.Log($"Launch formula result: " + formulaOuter.ToString());


			// float theta = Mathf.Atan(formulaOuter) * (180 / Mathf.PI);

			// Debug.Log($"Desired angle theta: " + theta.ToString());


            return new Vector2(TargetHelper.transform.rotation.eulerAngles.x, TargetHelper.transform.rotation.eulerAngles.y);
            
        }
    }
}