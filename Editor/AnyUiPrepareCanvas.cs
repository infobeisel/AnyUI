using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnyUI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnyUiPrepareCanvas : EditorWindow
{
    private const int canvasEdgeLength = 1024;
    private Object anyuimeshobject;
    private bool placeTestCheckerboard = true;

    [MenuItem("Window/Any UI/Setup AnyUI Canvas")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AnyUiPrepareCanvas window = (AnyUiPrepareCanvas)EditorWindow.GetWindow(typeof(AnyUiPrepareCanvas));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.Space(4);
        GUILayout.Label("Set up your mesh for Any UI.\nThe canvas is created automatically.", EditorStyles.boldLabel);
        anyuimeshobject = EditorGUILayout.ObjectField("AnyUI Mesh to set up", anyuimeshobject, typeof(GameObject), true);
        GUILayout.Space(4);
        placeTestCheckerboard = EditorGUILayout.Toggle("Place test texture?",placeTestCheckerboard);
        GUILayout.Space(8);

        AnyUiMesh mesh = null;
        if (anyuimeshobject != null)
        {

            var anyUiMeshGO = (GameObject)anyuimeshobject;
            if (anyUiMeshGO.GetComponent<AnyUiMesh>() == null)
                mesh = anyUiMeshGO.AddComponent<AnyUiMesh>();
            mesh = anyUiMeshGO.GetComponent<AnyUiMesh>();
        }
        var rect = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(rect, GUIContent.none))
        {
            if(mesh != null && mesh.CanvasToProject == null)
                createAnyUiCanvasFor(mesh);
        }
        GUILayout.Label("Ok");
        EditorGUILayout.EndHorizontal();
    }

    private void createAnyUiCanvasFor(AnyUiMesh mesh)
    {
        GameObject canvasGameObject = new GameObject();
        if(placeTestCheckerboard)CreateCheckerboardTexture(canvasGameObject);
        canvasGameObject.transform.parent = mesh.transform.parent;     
        canvasGameObject.name = "AnyUiCanvasFor" + mesh.gameObject.name;
        canvasGameObject.layer = 5; //ui layer

        mesh.CanvasToProject = canvasGameObject.AddComponent<Canvas>();
        mesh.CanvasToProject.renderMode = RenderMode.WorldSpace;
        canvasGameObject.AddComponent<CanvasScaler>();
        canvasGameObject.AddComponent<AnyUiCanvas>();

        RectTransform canvasRectTransform = canvasGameObject.GetComponent<RectTransform>();
        //try to determine the size and adapt created canvas size
        var col = mesh.GetComponent<Collider>();
        bool artificiallyCreatedCollider = false;
        if (col == null)
        {
            artificiallyCreatedCollider = true;
            col = mesh.gameObject.AddComponent<BoxCollider>();
        }
        float diagonalLength = Vector3.Distance( col.bounds.max , col.bounds.min);
        canvasRectTransform.localScale = Vector3.one * (diagonalLength / (float)canvasEdgeLength);
        canvasRectTransform.sizeDelta = new Vector2(canvasEdgeLength, canvasEdgeLength);
        mesh.ProjectionResolution = AnyUiResolution._1k;

        //place canvas next to object on the world y axis and let it look into negative z
        canvasRectTransform.position = col.bounds.center + new Vector3(0,  diagonalLength, 0);
        canvasRectTransform.transform.forward = -Vector3.forward;
        if (artificiallyCreatedCollider)
            DestroyImmediate(col);
        

        //add event system if needed
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject();
            eventSystem.name = "EventSystem";
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("Setup AnyUi Canvas: created an EventSystem because there wasn't one in this scene yet");
        }

       

    }
    
    private void CreateCheckerboardTexture(GameObject _canvasRoot)
    {
        // creates a checkerboard test image
        GameObject checkerBoardTexture = new GameObject();
        checkerBoardTexture.name = "testTexture";
        checkerBoardTexture.transform.parent = _canvasRoot.transform;
        checkerBoardTexture.AddComponent<Image>();

        if (!Resources.Load<Sprite>("TestCheckerboard")) return;
        Image _checkerBoardImage = checkerBoardTexture.GetComponent<Image>();
        _checkerBoardImage.sprite = Resources.Load<Sprite>("TestCheckerboard");
        _checkerBoardImage.SetNativeSize();
    }

}