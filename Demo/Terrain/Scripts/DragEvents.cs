using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace AnyUI.Demo
{
    public class DragEvents : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {

        public Image Marker;
        public Text Coordinate;

        private string m_default = "Drag";
        void Start()
        {
            Coordinate.text = m_default;
            Marker.color = TerrainDemoController.ActiveColor;
        }
  
        public void OnBeginDrag(PointerEventData data)
        {
            Coordinate.color = TerrainDemoController.ActiveColor;
           
        }
        public void OnDrag(PointerEventData data)
        {
            //PointerEventData.position is always in window-coordinates. (0,0) = bottom left corner. 
            //translate to canvas coords
            Canvas hostCanvas = GetComponentInParent<Canvas>();
            RectTransform hostCanvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 pointOnCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                hostCanvasRectTransform, data.position, hostCanvas.worldCamera, out pointOnCanvas);
            RectTransform myRectTransform = gameObject.GetComponent<RectTransform>();
            myRectTransform.localPosition = pointOnCanvas;
            //Debug.Log("Dragging:" + data.position);
            Coordinate.text = (int)(data.position.x / 10) + "," + (int)(data.position.y / 10);
        }
        public void OnEndDrag(PointerEventData data)
        {
            Coordinate.color = TerrainDemoController.ActiveColor;
            Coordinate.text = m_default;
        }
        public void OnPointerEnter(PointerEventData data)
        {
            Marker.color = TerrainDemoController.ActiveColor;
        }
        public void OnPointerExit(PointerEventData data)
        {
            Marker.color = TerrainDemoController.InactiveColor;
        }

    }
}

