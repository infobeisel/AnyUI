using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnyUI;

namespace AnyUI.Demo
{
    public class TerrainPopupPanel : MonoBehaviour
    {
        public static TerrainPopupPanel Instance;
        public Text[] TextFields;

        //-----------------------------------------------------------------------------------------------------
        private AnyUiMimeTransform m_mimeTransform;
        private Animator m_animator;
        private RectTransform m_nextPosition;
        private bool m_isOpen;
        //-----------------------------------------------------------------------------------------------------
        void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(this);
            }
        }
        //-----------------------------------------------------------------------------------------------------
        private void Start()
        {
            m_animator = GetComponent<Animator>();
            m_mimeTransform = GetComponent<AnyUiMimeTransform>();
        }
        private void Update()
        {
            transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z) * -1);
        }
        //-----------------------------------------------------------------------------------------------------
        public void PositionPanel(RectTransform _newPosition)
        {
   

            if (m_isOpen)
            {
                m_animator.SetTrigger("changePosition");
            }
            else
            {
                m_animator.SetTrigger("openPopup");
                m_isOpen = true;
            }
            m_nextPosition = _newPosition;
        }
        //-----------------------------------------------------------------------------------------------------
        public void ClosePanel()
        {
            m_isOpen = false;
            m_animator.SetTrigger("closePopup");
        }
        //-----------------------------------------------------------------------------------------------------
        public void ChangePosition()
        {
            m_mimeTransform.UIObjectToMime = m_nextPosition;
        }
        //-----------------------------------------------------------------------------------------------------
        public void WriteText()
        {

            for (int i = 0; i < TextFields.Length; i++)
            {
                TextFields[i].text = "";
                StartCoroutine(WriteTextCoroutine(TextFields[i], m_nextPosition.gameObject.GetComponent<TerrainDemoButton>().PanelText[i].ToUpper()));
            }

        }
        //-----------------------------------------------------------------------------------------------------
        IEnumerator WriteTextCoroutine(Text _text, string _textToWrite)
        {
            while (_text.text.Length < _textToWrite.Length)
            {
                _text.text += _textToWrite[_text.text.Length];
                yield return new WaitForEndOfFrame();
            }
        }

    }
}