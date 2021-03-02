using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AnyUI.Demo;

namespace AnyUI.Demo
{
    public class TerrainDemoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public enum TERRAINBUTTONSTATE { INACTIVE, ACTIVE };
        public TERRAINBUTTONSTATE TerrainButtonState { get { return m_terrainButtonState; } set { changeState(value); } }
        private TERRAINBUTTONSTATE m_terrainButtonState;
        //--------------------------------------------------------------------------------------
        public GameObject ObjectInScene;
        public bool IsActive;
        public string[] PanelText;
        //--------------------------------------------------------------------------------------
        private Button m_button;
        private Color m_activeColor;
        private Color m_inactiveColor;
        private Material m_objectMaterial;
        //--------------------------------------------------------------------------------------
        void Start()
        {
            m_button = GetComponent<Button>();
            m_activeColor = m_button.colors.highlightedColor;
            m_inactiveColor = m_button.colors.normalColor;
            m_objectMaterial = ObjectInScene.GetComponent<Renderer>().material;
            m_button.onClick.AddListener(OnClick);
        }
        //--------------------------------------------------------------------------------------
        public void OnClick()
        {
            TerrainButtonState = (m_terrainButtonState == TERRAINBUTTONSTATE.ACTIVE) ? TERRAINBUTTONSTATE.INACTIVE : TERRAINBUTTONSTATE.ACTIVE;
            TerrainDemoUIController.Instance.SelectionChanged(this.gameObject);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            BroadcastMessage("ShowText");
        }
        //-----------------------------------------------------------------------------------------------------
        public void OnPointerExit(PointerEventData eventData)
        {

            if (TerrainButtonState != TERRAINBUTTONSTATE.ACTIVE)
            {
                BroadcastMessage("HideText");
            }
        }
        //--------------------------------------------------------------------------------------
        public void BuildingActivatedEvent(GameObject _newSelection)
        {

            if (_newSelection == this.gameObject) return;
            if (TerrainButtonState == TERRAINBUTTONSTATE.ACTIVE)
            {
                TerrainButtonState = TERRAINBUTTONSTATE.INACTIVE;
                BroadcastMessage("HideText");
            }
        }
        //--------------------------------------------------------------------------------------
        private void changeState(TERRAINBUTTONSTATE _value)
        {
            if (m_terrainButtonState == _value) return;
            Color _newColor;

            if (_value == TERRAINBUTTONSTATE.ACTIVE)
            {
                _newColor = m_activeColor;
            }
            else
            {
                _newColor = m_inactiveColor;
            }

            StartCoroutine(changeObjectMaterialColor(_newColor));
            m_terrainButtonState = _value;


        }
        //--------------------------------------------------------------------------------------
        private IEnumerator changeObjectMaterialColor(Color _newColor)
        {
            float _tween = 0;
            while (_tween <= 1)
            {
                m_objectMaterial.color = Color.Lerp(m_objectMaterial.color, _newColor, _tween);
                _tween += Time.deltaTime * 1.5f;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}