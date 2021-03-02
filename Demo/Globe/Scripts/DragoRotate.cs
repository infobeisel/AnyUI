using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyUI.Library
{
    public class DragoRotate : MonoBehaviour
    {
        public enum RotationState { AUTO, MANUAL, STOP };
        public RotationState Rotation
        {
            get
            {
                return m_rotationState;
            }
            set
            {
                m_rotationState = value;
            }
        }
        //--------------------------------------------------------------------------------------------
        public float AutoRotationSpeed = 0.1f;
        //--------------------------------------------------------------------------------------------
        private RotationState m_rotationState;
        private float m_prevAxis = 0.0f;
        private float m_currAxis = 0.5f;
        private int m_direction = 1;
        //--------------------------------------------------------------------------------------------
        void Start()
        {
            Rotation = RotationState.AUTO;
            m_currAxis = AutoRotationSpeed;
        }
        //--------------------------------------------------------------------------------------------
        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                m_currAxis = 0.0f;
                Rotation = RotationState.MANUAL;
            }
            else if (Input.GetMouseButton(0))
            {
                m_currAxis = Mathf.Abs(Input.GetAxis("Mouse X"));

                if (m_prevAxis < Input.GetAxis("Mouse X"))
                {
                    m_direction = -1;
                }

                if (m_prevAxis > Input.GetAxis("Mouse X"))
                {
                    m_direction = 1;
                }

                transform.Rotate(Vector3.up, -Input.GetAxis("Mouse X"));
                m_prevAxis = -Input.GetAxis("Mouse X");
            }

            if (Input.GetMouseButtonUp(0))
            {
                Rotation = RotationState.AUTO;
            }

            if (m_rotationState == RotationState.AUTO)
            {
                if (m_currAxis > AutoRotationSpeed) m_currAxis -= 0.05f;
                if (m_rotationState == RotationState.STOP) return;
                if (m_currAxis < AutoRotationSpeed) m_currAxis += 0.05f;
            }
            transform.Rotate(Vector3.up, m_currAxis * m_direction);

        }
    }
}

