using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SteeringWheel : MonoBehaviour
{
    public string comPort;
    private Quaternion defaultRotation;

    private GestureListener gestureListener;
    private float angle;

    void Start()
    {
        defaultRotation = transform.localRotation;
        gestureListener = Camera.main.GetComponent<GestureListener>();
    }

    public string GetAngle()
    {
        Debug.Log(angle);
        if (angle < -10.0)
            return "1";
        else if (angle > 10.0)
            return "-1";
        else
            return "0";
    }

    void Update()
    {
        angle = gestureListener.getWheelAngle();
        transform.rotation = defaultRotation * Quaternion.AngleAxis(angle, Vector3.down);
    }


}