using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace AnyUI
{
    [CustomEditor(typeof(AnyUiMesh))]
    public class AnyUiMeshInspector : Editor
    {
        SerializedProperty useMaterialLayering;
        SerializedProperty useMaterial;
        SerializedProperty canvasToProject;
        SerializedProperty camera;
        SerializedProperty projectionResolution;

        bool currentMaterialLayeringEnabled;
        Object currentUseMaterial;
        void OnEnable() 
            //recalled when inspector shows component, EVERY frame.
            //whole script is REINSTANCIATED when inspector shows component and didnt show it in LAST frame
        {
            
            // Setup the SerializedProperties
            useMaterialLayering = serializedObject.FindProperty("UseMaterialLayering");
            useMaterial = serializedObject.FindProperty("UseMaterial");
            canvasToProject = serializedObject.FindProperty("CanvasToProject");
            camera = serializedObject.FindProperty("UseCamera");
            projectionResolution = serializedObject.FindProperty("ProjectionResolution");
            currentMaterialLayeringEnabled = useMaterialLayering.boolValue;
            currentUseMaterial = useMaterial.objectReferenceValue;
            

        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(canvasToProject, new GUIContent("Canvas To Project"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(projectionResolution, new GUIContent("Projection Resolution"), GUILayout.Height(20));

            EditorGUILayout.PropertyField(useMaterialLayering, new GUIContent("Use Material Layering"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(useMaterial, new GUIContent("Use Material"), GUILayout.Height(20));
           EditorGUILayout.PropertyField(camera, new GUIContent("Use Camera"), GUILayout.Height(20));


            

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            AnyUiMesh anyUiMesh = (AnyUiMesh)serializedObject.targetObject;
            //currentMaterialLayeringEnabled changed
            if (currentMaterialLayeringEnabled != useMaterialLayering.boolValue)
            {
                Debug.Log("change material layering");
                AnyUiSceneViewLiveEditing.createMaterialInMaterialsList(anyUiMesh);
            }

            //currentUseMaterial changed
            if (currentUseMaterial != useMaterial.objectReferenceValue)
            {
                Debug.Log("change material");
                if (useMaterialLayering.boolValue)
                    AnyUiSceneViewLiveEditing.deleteMaterialListEntry(anyUiMesh,anyUiMesh.UseMaterial);
                AnyUiSceneViewLiveEditing.createMaterialInMaterialsList(anyUiMesh);
            }
            currentMaterialLayeringEnabled = useMaterialLayering.boolValue;
            currentUseMaterial = useMaterial.objectReferenceValue;

        }
       
        
    }
}