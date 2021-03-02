using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyUI.Demo
{
    public class Stripe : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Animator>().SetFloat("speed",Random.Range(0.5f, 1f));
        }
    }

}
