using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Candlelight;


[RequireComponent(typeof(Animator))]
public class TankController : MonoBehaviour {

    [SerializeField, PropertyBackingField(nameof(TurretControlNode))]
    private GameObject turretControlNode;
    public GameObject TurretControlNode { get => turretControlNode; set => turretControlNode = value; }


    [SerializeField, PropertyBackingField(nameof(BarrelControlNode))]
    private GameObject barrelControlNode;
    public GameObject BarrelControlNode { get => barrelControlNode; set => barrelControlNode = value; }


    [SerializeField, PropertyBackingField(nameof(RocketLauncherFrameControlNode))]
    private GameObject rocketLauncherFrameControlNode;
    public GameObject RocketLauncherFrameControlNode { get => rocketLauncherFrameControlNode; set => rocketLauncherFrameControlNode = value; }


    [SerializeField, PropertyBackingField(nameof(RocketLauncherControlNode))]
    private GameObject rocketLauncherControlNode;
    public GameObject RocketLauncherControlNode { get => rocketLauncherControlNode; set => rocketLauncherControlNode = value; }
            
    
    [SerializeField, PropertyBackingField(nameof(TurretRotation))]
    private float turretRotation;
    public float TurretRotation { get => turretRotation; set { turretRotation = value; UpdateRotations(); } }
    

    [SerializeField, PropertyBackingField(nameof(BarrelRotation))]
    private float barrelRotation;
    public float BarrelRotation { get => barrelRotation; set { barrelRotation = value; UpdateRotations(); } }
    
    [SerializeField, PropertyBackingField(nameof(LauncherFrameRotation))]
    private float launcherFrameRotation;
    public float LauncherFrameRotation { get => launcherFrameRotation; set { launcherFrameRotation = value; UpdateRotations(); } }
    

    [SerializeField, PropertyBackingField(nameof(RocketLauncherRotation))]
    private float rocketLauncherRotation;
    public float RocketLauncherRotation { get => rocketLauncherRotation; set { rocketLauncherRotation = value; UpdateRotations(); } }
    

    [SerializeField, PropertyBackingField(nameof(IsMoving))]
    private bool isMoving;
    public bool IsMoving { 
        get => isMoving; 
        set { 
            isMoving = value; 
            if(animator != null) animator.SetBool("IsMoving", isMoving); 
        }
    }
    

    protected Animator animator;
    public void Start() {
        if(TurretControlNode == null || BarrelControlNode == null || RocketLauncherFrameControlNode == null || RocketLauncherControlNode) {
            Debug.LogError($"TankController: One or more control nodes are not set!");
        }
        animator = GetComponent<Animator>();
    }
    
    public void Update() {
    
    }

    protected void UpdateRotations() {
        TurretControlNode.transform.localEulerAngles = new Vector3(0f, TurretRotation, 0f);
        BarrelControlNode.transform.localEulerAngles = new Vector3(0f, 0f, BarrelRotation);
        RocketLauncherFrameControlNode.transform.localEulerAngles = new Vector3(0f, LauncherFrameRotation, 0f);
        RocketLauncherControlNode.transform.localEulerAngles = new Vector3(0f, 0f, RocketLauncherRotation);
    }
}
