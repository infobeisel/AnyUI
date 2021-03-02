using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetYRotationWithinRange : MonoBehaviour {
    public float angularSpeed;

    private float targetYRotation = 0.0f;
    private float currentAngularSpeed = 0.0f;


    public void setYTargetRotation(float v)
    {
        targetYRotation = v;
        

    }


    void Update()
    {
        Quaternion qTarget = transform.parent.rotation * Quaternion.Euler(0.0f, targetYRotation, 0.0f);
        float deltaRotation = Quaternion.Angle(transform.rotation, qTarget);
        currentAngularSpeed += deltaRotation;
        currentAngularSpeed = Mathf.Clamp(currentAngularSpeed, -angularSpeed, angularSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, qTarget , currentAngularSpeed * Time.deltaTime);
        //Debug.Log(currentYRotation + " " + deltaRotation + " " + currentAngularSpeed + " ");
    }

    
	
}
