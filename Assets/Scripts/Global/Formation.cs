using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace MaxGame {

    /// <summary>
    /// This enumerates the various Formation types. Current available types: 
    /// Current: Holds units in their positions relative to each other.
    /// Square: Places units into a square formation.
    /// </summary>

    public enum FormationShape {
        Current,
        Square

    }

    /// <summary>
    /// This class is used to create unit formations - That is, groups of units
    /// that remain in position relative to each other.
    /// It takes a list of units, places the Formation object at the transform
    /// of the first unit on the list, and then assigns relative position values
    /// based on that position. 
    /// If the core unit is destroyed, it assigns itself to a new unit
    /// and recalculates the relative positioning.
    ///
    /// 
    /// </summary>


    public class Formation : GameItem {

        protected FormationShape Shape;

        protected float Spacing = 15.0f;

        protected Dictionary<Unit, Vector3> UnitDict;

        protected Unit CoreUnit;

        protected Vector3 PathTarget = Vector3.zero;

        protected Vector3 Offset = Vector3.zero;

        protected GameObject DisplayArea;

        protected List<GameObject> FormationPositions;

        protected bool IsPopulated = false;

        protected NavMeshAgent Agent;

        void Awake() {

            Agent = GetComponent<NavMeshAgent>();

        }

        void Start() {


        }
        public void CreateFormation(List<Unit> unitList, FormationShape shape) {

            Debug.Log($"Creating formation of size " + unitList.Count);
            CoreUnit = unitList[0];
            Agent.Warp(CoreUnit.transform.position);
            Debug.Log($"Formation NavAgent on navmesh?" + Agent.isOnNavMesh);
            TeamID = CoreUnit.TeamID;

            
            SetShape(shape);

            UnitDict = new Dictionary<Unit, Vector3>();
            

            for (int i = 0; i < unitList.Count; i++) {
                Unit unit = unitList[i];

                unit.SetFormation(this);

                UnitDict.Add(unit, Vector3.zero);
            }

            Generate();
        }

        void Update() {

            if (IsPopulated && (Agent.steeringTarget != null) && (Agent.steeringTarget != PathTarget)) {

                PathTarget = Agent.steeringTarget;

                foreach (KeyValuePair<Unit, Vector3> kvp in UnitDict) {
                    Unit unit = kvp.Key;

                    unit.SetTarget(PathTarget + Quaternion.AngleAxis(DisplayArea.transform.localEulerAngles.y, Vector3.up) * kvp.Value);
                }

            }
        }



        public override void Selected() {

            foreach (KeyValuePair<Unit, Vector3> kvp in UnitDict) {
                kvp.Key.Selected();
            }
        }

        public override void Deselected() {

            foreach (KeyValuePair<Unit, Vector3> kvp in UnitDict) {
                kvp.Key.Deselected();
            }
        }

        public override void SetTarget(GameItem target) {

            AttackTarget = target.gameObject;

        }

        public override void SetTarget(Vector3 target) {

            TargetPoint = target;
            
            Path = new NavMeshPath();
            Debug.Log($"Is formation agent on mesh?: " + Agent.isOnNavMesh);


            //Agent.CalculatePath(target, Path);
            Agent.destination = target;

        }

        public void Remove(Unit unit) {

            UnitDict.Remove(unit);

            if (unit == CoreUnit) {

                foreach (KeyValuePair<Unit,Vector3> kvp in UnitDict) {

                    Unit CoreUnit = kvp.Key;
                    Offset = CoreUnit.transform.position - this.transform.position;
                    break;

                }
            }

            if (UnitDict.Count == 0) {

                GameObject.Destroy(this.gameObject);
            }
        }
        
        public void SetShape(FormationShape shapeIn) {

            Shape = shapeIn;
        }

        protected void Generate() {

            FormationPositions = new List<GameObject>();

            DisplayArea = Instantiate(transform.Find("FormationArea").gameObject);
            GameObject unitArea = DisplayArea.transform.Find("FormationUnit").gameObject;

            if (Shape == FormationShape.Square) {
            
                int size = (int)Mathf.Round(Mathf.Sqrt(UnitDict.Count));

                for (int i = 0; i < size; i++) {

                    for (int j = 0; j < size; j++) {
                       
                        GameObject newUnitArea = Instantiate(unitArea);
                        newUnitArea.transform.SetParent(DisplayArea.transform);
                        newUnitArea.transform.localPosition = new Vector3(i * Spacing, 0.0f, j * Spacing);
                        FormationPositions.Add(newUnitArea);
                        
                    }
                }
            }
            else if (Shape == FormationShape.Current) {

                Vector3 corePosition = Vector3.zero;
                bool coreSet = false;

                foreach (KeyValuePair<Unit, Vector3> kvp in UnitDict) {

                    if (coreSet == false) {
                        corePosition = kvp.Key.transform.position;
                        
                        GameObject newUnitArea = Instantiate(unitArea);
                        newUnitArea.transform.SetParent(DisplayArea.transform);
                        newUnitArea.transform.localPosition = Vector3.zero;
                        FormationPositions.Add(newUnitArea);

                        coreSet = true;
                    }
                    else {
                        GameObject newUnitArea = Instantiate(unitArea);
                        newUnitArea.transform.SetParent(DisplayArea.transform);
                        newUnitArea.transform.localPosition = kvp.Key.transform.position - corePosition;
                        FormationPositions.Add(newUnitArea);
                    }
                }

            }
        }

        public void Finalize() {

            //NavMeshAgent 

            // if (IsPopulated) {

            //     foreach (KeyValuePair<Unit,Vector3> kvp in UnitDict) {
                    
            //         Unit unit = kvp.Key;
            //         unit.SetTarget(this.transform.position + UnitDict[unit]);

            //     }
            // }
            if (!IsPopulated) {

                Dictionary<Unit, Vector3> tempDict = new Dictionary<Unit, Vector3>();
                List<Unit> unitList = new List<Unit>();


                if (Shape == FormationShape.Current) {

                    foreach (KeyValuePair<Unit,Vector3> kvp in UnitDict) {

                        unitList.Add(kvp.Key);
                        
                    }

                    Debug.Log($"Current formation size: " + FormationPositions.Count);
                    Debug.Log($"Number of units: " + unitList.Count);

                    for (int i = 0; i < FormationPositions.Count; i++) {
                        
                        Unit unit = unitList[i];

                        Vector3 offset = FormationPositions[i].transform.localPosition;

                        tempDict.Add(unit, offset);

                    }
                    UnitDict = tempDict;
                    IsPopulated = true;
                }
                else if (Shape == FormationShape.Square) {


                    foreach (KeyValuePair<Unit,Vector3> kvp in UnitDict) {

                        unitList.Add(kvp.Key);
                        
                    }

                    Debug.Log($"Square formation size: " + FormationPositions.Count);
                    Debug.Log($"Number of units: " + unitList.Count);

                    for (int i = 0; i < FormationPositions.Count; i++) {
                        
                        Unit unit = unitList[i];

                        Vector3 offset = FormationPositions[i].transform.localPosition;

                        tempDict.Add(unit, offset);

                    }
                    UnitDict = tempDict;
                    IsPopulated = true;

                }
            }

            DisplayArea.SetActive(false);

        }

        /// <summary>
        /// Returns the DisplayArea so that it can be used to visualize the
        /// new location of the formation.
        /// </summary>
        public GameObject GetArea() {

            return DisplayArea;

        }
    }
}