using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Candlelight;

namespace MaxGame {

    /// <summary>
    /// This class allows sensor attributes to be shown or
    /// hidden depending on whether or not those sensors
    /// are enabled in the particular unit.
    ///
    /// </summary>


    [CustomEditor(typeof(SensorSphere))]
    public class CustomSensorEditor : Editor {

        
        SerializedProperty HasRadar;
        SerializedProperty RadarRange;
        SerializedProperty RadarLevel;



        void OnEnable() {

            HasRadar = serializedObject.FindProperty("HasRadar");
            RadarRange = serializedObject.FindProperty("RadarRange");

        }

        override public void OnInspectorGUI() {

            serializedObject.UpdateIfRequiredOrScript();

            List<string> excludedProperties = new List<string>();

            if (HasRadar.boolValue == false) {
                excludedProperties.Add("RadarRange");
                excludedProperties.Add("RadarLevel");

            }

            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            serializedObject.ApplyModifiedProperties();

        }
        
    

    }
}