using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnyUI.Demo
{
    public class WriteText : MonoBehaviour
    {
        
        public float WritingSpeed = 1;
        //-----------------------------------------------------------------------------------------------------
        private Text LabelText;
        private string m_originalText;
        private bool m_isWriting;
        //-----------------------------------------------------------------------------------------------------
        void Start()
        {
            
            LabelText = GetComponent<Text>();
            m_originalText = LabelText.text;
            LabelText.text = "";
        }
        //-----------------------------------------------------------------------------------------------------
        public void ShowText()
        {
            if (LabelText.text == m_originalText||m_isWriting) return;          
            StartCoroutine(WriteTextCoroutine());
        }
        //-----------------------------------------------------------------------------------------------------
        public void HideText()
        {
            if (m_isWriting) return;
            m_isWriting = true;
            StartCoroutine(RemoveTextCoroutine());
        }
        //-----------------------------------------------------------------------------------------------------
        private IEnumerator RemoveTextCoroutine()
        {
            int _index = LabelText.text.Length;
            while (_index >= 0)
            {
                
                LabelText.text= LabelText.text.Substring(0,_index);
                _index--;
                yield return new WaitForSeconds(0.1f * WritingSpeed);
            }
            m_isWriting = false;
        }
        //-----------------------------------------------------------------------------------------------------
        private IEnumerator WriteTextCoroutine()
        {
            int _index = 0;
            m_isWriting = true;
            while(_index < m_originalText.Length)
            {
                LabelText.text += m_originalText[_index];
                _index++;
                yield return new WaitForSeconds(0.1f * WritingSpeed);
            }
            m_isWriting = false;
        }

    }

}
