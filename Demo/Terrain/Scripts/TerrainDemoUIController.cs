using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnyUI.Demo
{
    public class TerrainDemoUIController : MonoBehaviour
    {
        public static TerrainDemoUIController Instance;
        //-----------------------------------------------------------------------------------------------------
        [HideInInspector]
        public GameObject CurrentSelection;
        public Image Grid;
        public Slider GridSlider;
        //-----------------------------------------------------------------------------------------------------
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        //-----------------------------------------------------------------------------------------------------
        public void GridOppacity()
        {
            Grid.color = new Color(1, 1, 1, GridSlider.value);
        }
        //-----------------------------------------------------------------------------------------------------
        public void SelectionChanged(GameObject _newSelection)
        {
            BroadcastMessage("BuildingActivatedEvent", _newSelection);

            if(_newSelection != CurrentSelection)
            {
            
                TerrainPopupPanel.Instance.PositionPanel(_newSelection.GetComponent<RectTransform>());
                CurrentSelection = _newSelection;
                return;
            }
            else
            {
                TerrainPopupPanel.Instance.ClosePanel();              
            }
            CurrentSelection = null;
        }
        //-----------------------------------------------------------------------------------------------------
    }

}
