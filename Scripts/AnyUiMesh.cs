using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


using UnityEngine.UI;

namespace AnyUI
{
    public enum AnyUiResolution
    {
        _1k = 1024,
        _2k = 2048,
        _4k = 4096,
        _8k = 8192
    }
    /// <summary>
    /// A Raycaster to convey input events (occuring on a 3d model) to a given canvas
    /// </summary>
    public class AnyUiMesh : BaseRaycaster 
    {

        [Tooltip("Which canvas should be projected on this object?")]
        public Canvas CanvasToProject;
		[Tooltip("Show the camera which is used for the canvas (e.g. for additional camera effects / post processing)")]
        public bool ShowCanvasCamera = false;
        [Tooltip("_1k means the original canvas will be projected onto a 1024 x Y texture on the mesh, whereas Y depends on the original canvas' aspect ratio")]
        public AnyUiResolution ProjectionResolution = AnyUiResolution._1k;
        [Tooltip("Material to use for the projected UI. If null, a Standard Material will be used.")]
        public Material UseMaterial;
        [Tooltip("If you want to add the canvas-material to this object's material list instead of replacing it, set the check mark")]
        public bool UseMaterialLayering = true;
		[Tooltip("If you need a camera other than the 'Main Camera' to interact with the projected canvas, set it here")]
        public Camera UseCamera;
		
        
        
        public override Camera eventCamera
        {
            get
            {
                if (UseCamera == null)
                    return Camera.main;
                else return UseCamera;
            }
        }
        /// <summary>
        /// the Graphic Raycaster on the GameObject which holds the original Unity Canvas
        /// </summary>
        private AnyUiCanvas receiver;

#if VRTK_VERSION_3_2_1_OR_NEWER

        private Vector3 vrtkLastPointerPos;
        private Vector3 vrtkLastPointerDir;
#endif

        protected override void Start()
        {
            base.Start();
            receiver = CanvasToProject != null ? CanvasToProject.GetComponent<AnyUiCanvas>() : null;
        }



        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            Collider c = GetComponent<Collider>();
#if VRTK_VERSION_3_2_1_OR_NEWER
            
            RaycastResult current = eventData.pointerCurrentRaycast;
            RaycastResult press = eventData.pointerPressRaycast;
            



            Ray rCurrent = new Ray(current.worldPosition, current.worldNormal);
            Ray rLast = new Ray(vrtkLastPointerPos, vrtkLastPointerDir);
            Ray rPress = new Ray(press.worldPosition, press.worldNormal);

            vrtkLastPointerPos = current.worldPosition == Vector3.zero ? vrtkLastPointerPos : current.worldPosition;
            vrtkLastPointerDir = current.worldNormal == Vector3.zero ? vrtkLastPointerDir : current.worldNormal;


            //make the canvas "valid" for VRTK_VRInputModule.ValidElement(...) method
            if (CanvasToProject.GetComponent<AnyUiFakeVRTKCanvas>() == null)
                CanvasToProject.gameObject.AddComponent<AnyUiFakeVRTKCanvas>();
#else
            Ray rCurrent = eventCamera.ScreenPointToRay(eventData.position);
            Ray rLast = eventCamera.ScreenPointToRay(eventData.position - eventData.delta);
            Ray rPress = eventCamera.ScreenPointToRay(eventData.pressPosition);
#endif
            RaycastHit i;

            receiver.InputPossible = false;
            //perform raycast against this object, append results
            if (rCurrent.direction != Vector3.zero &&  c.Raycast(rCurrent, out i, float.MaxValue))
            {
                //hit, tell canvas
                receiver.InputPossible = true;

                PointerEventData pData = eventData;

                Vector2 guiPos = i.textureCoord;
                Vector3 screenPoint = receiver.eventCamera.ViewportToScreenPoint(guiPos);
                //update hover position
                pData.position = new Vector2(screenPoint.x, screenPoint.y);

                //update  delta
                if (rLast.direction != Vector3.zero && c.Raycast(rLast, out i, float.MaxValue))
                {
                    guiPos = i.textureCoord;
                    Vector3 lastScreenPoint = receiver.eventCamera.ViewportToScreenPoint(guiPos);
                    pData.delta = new Vector2(screenPoint.x - lastScreenPoint.x, screenPoint.y - lastScreenPoint.y);
                    //Debug.Log(pData.delta);
                }

                //update press position
                if (rPress.direction != Vector3.zero && c.Raycast(rPress, out i, float.MaxValue))
                {
                    guiPos = i.textureCoord;
                    screenPoint = receiver.eventCamera.ViewportToScreenPoint(guiPos);
                    pData.pressPosition = new Vector2(screenPoint.x, screenPoint.y);
                }
                
                List<RaycastResult> results = new List<RaycastResult>();
                receiver.setPointerEventDataHashMask(pData.GetHashCode());
                //continue raycast on GUI
                receiver.Raycast( pData, results);
                resultAppendList.AddRange(results);
            }
        }

    }
#if VRTK_VERSION_3_2_1_OR_NEWER

    public class AnyUiFakeVRTKCanvas : VRTK.VRTK_UICanvas
    {

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnTriggerEnter(Collider collider)
        {
           
        }

        protected override void OnTriggerExit(Collider collider)
        {
           
        }

        protected override void SetupCanvas()
        {
        }

        protected override IEnumerator CreateDraggablePanel(Canvas canvas, Vector2 canvasSize)
        {
            yield return null;
        }

        protected override void CreateActivator(Canvas canvas, Vector2 canvasSize)
        {
            
        }

        protected override void RemoveCanvas()
        {
            
        }
    }
#endif

}