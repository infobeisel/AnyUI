using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

    public float IntensityFactor = 0.01f;
    public bool UseHitCollidersForwardVectorForWindDir = false;
    private float intensity = 0.0f;
    public void setIntensity(float i)
    {
        intensity = IntensityFactor * i;
    }

	void OnTriggerStay(Collider other)
    {
        Rigidbody body = other.GetComponent<Rigidbody>();

        if (body != null && !body.isKinematic)
        {
            body.AddForce( (!UseHitCollidersForwardVectorForWindDir ? transform : body.transform).forward * intensity);
        }
    }
	
}
