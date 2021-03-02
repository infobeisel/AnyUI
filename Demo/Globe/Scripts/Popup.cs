using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AnyUI.Demo
{
    public class Popup : MonoBehaviour
    {
        public enum POPUPSTATE { CLOSED, OPEN };
        public POPUPSTATE PopupState
        {
            get
            {
                return m_popupState;
            }
            set
            {
                m_popupState = value;
                ChangeState();
            }
        }
        //-----------------------------------------------------------------------------------------------------
        public Text PopupLabel;
        public Animator Animations;
        public Canvas Canvas;
        public bool CanMove { get; set; }
        //-----------------------------------------------------------------------------------------------------
        private POPUPSTATE m_popupState;
        private AnyUiMimeTransform m_mineTransform;
        private RectTransform m_currentLocation;
        private CityMarker m_currentMarker;
        private string m_cityName;
        //-----------------------------------------------------------------------------------------------------
        private void Start()
        {
            Canvas.enabled = false;
            m_mineTransform = GetComponent<AnyUiMimeTransform>();
            PopupState = POPUPSTATE.CLOSED;
            CanMove = true;
        }
        //-----------------------------------------------------------------------------------------------------
        public void RelocatePopup(CityMarker _newMarker)
        {
            CanMove = false;
            StartCoroutine(RelocatePopupCoroutine(_newMarker));       
        }
        //-----------------------------------------------------------------------------------------------------
        private void ChangeState()
        {
            switch (m_popupState)
            {
                case POPUPSTATE.OPEN:
                    Canvas.enabled = true;
                    ChangeText(m_cityName);
                    Animations.ResetTrigger("Close");
                    Animations.SetTrigger("Open");                
                    m_mineTransform.UIObjectToMime = m_currentLocation;
                    break;
                case POPUPSTATE.CLOSED:
                    Animations.SetTrigger("Close");
                    Animations.SetBool("PopupClosed", true);
                    break;
                default:
                    break;
            }
        }
        //-----------------------------------------------------------------------------------------------------
        private void ChangeText(string _text)
        {
            PopupLabel.text = _text;
        }
        //-----------------------------------------------------------------------------------------------------
        private IEnumerator RelocatePopupCoroutine(CityMarker _newCityMarker)
        {
            if (m_currentMarker != null) m_currentMarker.ChangeState();
            if (PopupState == POPUPSTATE.OPEN) {
                PopupState = POPUPSTATE.CLOSED;
                while (Animations.GetBool("PopupClosed") == true) { yield return null; }
            };
            _newCityMarker.ChangeState();
            m_currentMarker = _newCityMarker;
            CanMove = true;
            m_cityName = m_currentMarker.CityName;        
            m_currentLocation = m_currentMarker.RectTransform;
            PopupState = POPUPSTATE.OPEN;
            
        }
    }


}





