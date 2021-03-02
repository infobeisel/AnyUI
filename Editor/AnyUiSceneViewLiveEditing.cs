using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;


namespace AnyUI
{

    [InitializeOnLoad]
    public class AnyUiSceneViewLiveEditing : Editor
    {
        private static bool CallbacksRegistered = false;
        static AnyUiSceneViewLiveEditing()
        {
            if (!CallbacksRegistered)
            {
                EditorApplication.update += lookForAddedAnyUiMeshComponentsAndInitialize;
                CallbacksRegistered = true;
            }
        }
      
        public static void initializeAnAnyUi(AnyUiMesh anyui)
        {
            DestroyImmediate(anyui.GetComponent<Collider>());
            anyui.gameObject.AddComponent<MeshCollider>();

            createCameraAndRendertextureFor(anyui);
            createMaterialInMaterialsList(anyui);
        }

        private static void createCameraAndRendertextureFor(AnyUiMesh tTarget)
        {
            //configure canvas
            Canvas canvasToProject = tTarget.CanvasToProject;
            canvasToProject.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRectangle = canvasToProject.GetComponent<RectTransform>();
            //replace Graphic raycaster
            AnyUiCanvas currentReceiver = canvasToProject.GetComponent<AnyUiCanvas>();
            if (currentReceiver == null)
            {
                GraphicRaycaster oldReceiver = canvasToProject.GetComponent<GraphicRaycaster>();
                AnyUiCanvas newReceiverRayCaster = canvasToProject.gameObject.AddComponent<AnyUiCanvas>();
                newReceiverRayCaster.ignoreReversedGraphics = oldReceiver.ignoreReversedGraphics;
                newReceiverRayCaster.blockingObjects = oldReceiver.blockingObjects;
                DestroyImmediate(oldReceiver);
            }

            RenderTexture canvasToTexture = null;
            Camera canvasCam = null;

            //create camera and render texture if necessary
            if (canvasToProject.worldCamera == null)
            {
                //create camera looking at canvas
                canvasCam = new GameObject().AddComponent<Camera>();
                //editor: make camera invisible
                canvasCam.transform.parent = canvasToProject.transform;
                //if canvasToProject is scaled, adapt!
                canvasCam.transform.localScale = Vector3.one;

                canvasCam.name = "AnyUI_CanvasCam" + canvasToProject.gameObject.name + System.Guid.NewGuid().ToString();
                canvasCam.clearFlags = CameraClearFlags.SolidColor;
                canvasCam.backgroundColor = Color.clear;
                canvasCam.cullingMask = 1 << canvasToProject.gameObject.layer;
                canvasCam.depth = -1;

                canvasCam.orthographic = true;
                canvasCam.orthographicSize = canvasRectangle.rect.height * canvasRectangle.localScale.y / 2.0f; //vertical
                canvasCam.nearClipPlane = 0.9f;
                canvasCam.farClipPlane = 1.1f;
                canvasCam.projectionMatrix = Matrix4x4.Ortho(-canvasRectangle.rect.width * canvasRectangle.localScale.x  / 2.0f,
                                                                canvasRectangle.rect.width * canvasRectangle.localScale.x / 2.0f,
                                                                -canvasRectangle.rect.height * canvasRectangle.localScale.y / 2.0f,
                                                                 canvasRectangle.rect.height * canvasRectangle.localScale.y / 2.0f,
                                                                0.5f, 1.5f);
                
                canvasCam.transform.localPosition = Vector3.zero;
                canvasCam.transform.Translate(-canvasRectangle.forward,Space.World);
                canvasCam.transform.rotation = canvasRectangle.rotation;

                canvasToProject.worldCamera = canvasCam;


            }
            else
            {
                canvasCam = canvasToProject.worldCamera;
            }

            if(canvasToProject.worldCamera.targetTexture == null)
            {
                //new RenderTexture
                canvasToTexture = new RenderTexture((int)tTarget.ProjectionResolution, (int)((float)tTarget.ProjectionResolution * (canvasRectangle.rect.height / canvasRectangle.rect.width)), 0);
                canvasToTexture.name = "AnyUI_RenderTexture" + canvasToProject.gameObject.name + System.Guid.NewGuid().ToString();
                if (!AssetDatabase.IsValidFolder("Assets/AnyUI/AnyUIRenderTexturesAndMaterials"))
                {
                    AssetDatabase.CreateFolder("Assets/AnyUI", "AnyUIRenderTexturesAndMaterials");
                }
                AssetDatabase.CreateAsset(canvasToTexture, "Assets/AnyUI/AnyUIRenderTexturesAndMaterials/" + canvasToTexture.name + ".renderTexture");
                canvasCam.targetTexture = canvasToTexture;
            } else
            {
                canvasToTexture = canvasCam.targetTexture;
            }

        }

        public static Material createMaterialInMaterialsList(AnyUiMesh tTarget)
        {
            Material mat;
            var renderer = tTarget.GetComponent<Renderer>();
         
            if (tTarget.UseMaterial == null)
            {
                //new Material
                mat = new Material(Shader.Find("Unlit/Transparent"));
                mat.mainTexture = tTarget.CanvasToProject.worldCamera.targetTexture;
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                mat.name = "AnyUI_Material" + tTarget.gameObject.name + System.Guid.NewGuid().ToString();
                tTarget.UseMaterial = mat;
                if (!AssetDatabase.IsValidFolder("Assets/AnyUI/AnyUIRenderTexturesAndMaterials"))
                {
                    AssetDatabase.CreateFolder("Assets/AnyUI", "AnyUIRenderTexturesAndMaterials");
                }
                AssetDatabase.CreateAsset(mat, "Assets/AnyUI/AnyUIRenderTexturesAndMaterials/" + mat.name + ".mat");
            }
            else
            {
                mat = tTarget.UseMaterial;
            }

            //new materials array

            if (tTarget.UseMaterialLayering )
            {
                Material[] tNewMaterials;
                //search for already existent rendertexture material, assign updated material, else add as new material
                bool isAlreadyAdded = false;
                int j = 0;
                foreach (var t in renderer.sharedMaterials)
                {
                    if (t == mat)
                    {
                        //Debug.Log("already added render texture");
                        isAlreadyAdded = true;
                        break;
                    }
                    j++;
                }
                if (isAlreadyAdded)
                {
                    tNewMaterials = renderer.sharedMaterials;
                    tNewMaterials[j] = mat;
                }
                else
                {
                    tNewMaterials = new Material[renderer.sharedMaterials.Length + 1];
                    int i = 0;
                    foreach (var t in renderer.sharedMaterials)
                    {
                        tNewMaterials[i] = t;
                        i++;
                    }
                    tNewMaterials[i] = mat;
                }
                //assign new materials array
                renderer.sharedMaterials = tNewMaterials;
            }
            else
            {
                Material[] tNewMaterials = new Material[1];
                tNewMaterials[0] = mat;
                renderer.sharedMaterials = tNewMaterials;
            }

            return mat;
        }
        public static void deleteMaterialListEntry(AnyUiMesh tTarget, Material toDelete)
        {
            Material[] newMaterialsList = new Material[tTarget.GetComponent<Renderer>().sharedMaterials.Length - 1];
            var renderer = tTarget.GetComponent<Renderer>();

            int i = 0;
            int j = 0;
            while (j < newMaterialsList.Length && i < renderer.sharedMaterials.Length)
            {
                if (renderer.sharedMaterials[i] != toDelete)
                {
                    newMaterialsList[j] = renderer.sharedMaterials[i];
                    j++;
                }
                i++; //go through the old materials

            }
            renderer.sharedMaterials = newMaterialsList;
        }


        private static void lookForAddedAnyUiMeshComponentsAndInitialize()
        {

            //if the component is added the first time and we are in edit mode, call the live editing init
            if (Selection.activeGameObject != null
                && Selection.activeGameObject.GetComponent<AnyUiMesh>() != null
                && Selection.activeGameObject.GetComponent<AnyUiMesh>().CanvasToProject != null
                && Selection.activeGameObject.GetComponent<AnyUiMesh>().CanvasToProject.GetComponent<AnyUiCanvas>() != null)
            {

                if (
                //if camera or texture is missing, repair it
                (Selection.activeGameObject.GetComponent<AnyUiMesh>().CanvasToProject.GetComponent<AnyUiCanvas>().eventCamera == null
                || (Selection.activeGameObject.GetComponent<AnyUiMesh>().CanvasToProject.GetComponent<AnyUiCanvas>().eventCamera != null
                && Selection.activeGameObject.GetComponent<AnyUiMesh>().CanvasToProject.GetComponent<AnyUiCanvas>().eventCamera.targetTexture == null))
                )
                {
                    //edit mode anyui not yet initialized
                    initializeAnAnyUi(Selection.activeGameObject.GetComponent<AnyUiMesh>());
                }
             
            }

        }

    }
}