using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;


using UnityEngine.UI;

namespace AnyUI
{
    /// <summary>
    /// GraphicRaycaster which only accepts input events that have a certain hash value
    /// </summary>
    [ExecuteInEditMode]
    public class AnyUiCanvas : GraphicRaycaster {


        private int pointerEventDataHashMask;
        public override Camera eventCamera
        {
            get
            {
                return GetComponent<Canvas>().worldCamera;
            }
        }
        public void setPointerEventDataHashMask(int h)
        {
            pointerEventDataHashMask = h;
        }

        public bool InputPossible
        {
            get;
            set;
        }
        

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (eventData.GetHashCode() == pointerEventDataHashMask && InputPossible)
            {

                base.Raycast(eventData, resultAppendList);
            }
        }
        protected override void OnDestroy()
        {
            //TODO only the case if whole game object is destroyed and not the component alone
            if (Event.current != null && (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete"))
            {
                //destroy rendertexture,  camera
                //AssetDatabase.DeleteAsset("Assets/AnyUIRenderTexturesAndMaterials/" + eventCamera.targetTexture.name + ".renderTexture");
                DestroyImmediate(eventCamera);
            }
            base.OnDestroy();

        }
        
    }

}