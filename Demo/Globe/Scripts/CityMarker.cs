
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AnyUI.Demo
{
    public class CityMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public enum CITYMARKERSTATE { INACTIVE,ACTIVE }
        public CITYMARKERSTATE CityMarkerState { get { return m_cityMarkerState; } set { m_cityMarkerState = value; ChangeSprite(value); } }
        //-----------------------------------------------------------------------------------------------------
        public string CityName;
        public Popup CityPopup;
        public Sprite ActiveImage;
        public Sprite InactiveImage;
        [HideInInspector]
        public RectTransform RectTransform;
        //-----------------------------------------------------------------------------------------------------
        private Button m_button;
        private Animator m_Animator;
       
        private CITYMARKERSTATE m_cityMarkerState;
        //-----------------------------------------------------------------------------------------------------
        private void Start()
        {
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(OnClick);
            m_Animator = GetComponent<Animator>();
            RectTransform = GetComponent<RectTransform>();
        }
        //-----------------------------------------------------------------------------------------------------
        public void OnClick()
        {

            if (CityMarkerState == CITYMARKERSTATE.INACTIVE&& CityPopup.CanMove) CityPopup.RelocatePopup(this);      
        }
        //-----------------------------------------------------------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_cityMarkerState == CITYMARKERSTATE.INACTIVE)
                m_Animator.SetTrigger("Highlighted");
        }
        //-----------------------------------------------------------------------------------------------------
        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_cityMarkerState == CITYMARKERSTATE.INACTIVE)
                m_Animator.SetTrigger("Normal");
        }
        //-----------------------------------------------------------------------------------------------------
        public void ChangeState()
        {
            CityMarkerState = (m_cityMarkerState == (CITYMARKERSTATE.ACTIVE) ? (CITYMARKERSTATE.INACTIVE) : (CITYMARKERSTATE.ACTIVE));
        }
        //-----------------------------------------------------------------------------------------------------
        private void ChangeSprite(CITYMARKERSTATE _state)
        {
          GetComponent<Image>().sprite = _state==CITYMARKERSTATE.ACTIVE ? ActiveImage : InactiveImage;
            m_Animator.SetTrigger(_state == CITYMARKERSTATE.ACTIVE ? "Active" : "Deactive");
        }
    }

}
