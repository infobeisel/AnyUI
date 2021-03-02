using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyUI.Library
{
    public class LookAt : MonoBehaviour
    {
        
        public Transform Target;
        //-----------------------------------------------------------------------------------------------------
        void Update()
        {
            transform.LookAt((2 * transform.position - Target.position));
            
        }

    }
}

