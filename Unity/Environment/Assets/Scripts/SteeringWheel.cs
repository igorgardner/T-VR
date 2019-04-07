using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Linq;
using System.Text;

public class SteeringWheel : MonoBehaviour
{
    //public GameObject Wheel;
    Quaternion defaultRotation;

    private GestureListener gestureListener;

    void Start()
    {
        defaultRotation = transform.localRotation;
        gestureListener = Camera.main.GetComponent<GestureListener>();

    }

    void Update()
    {
        transform.rotation = defaultRotation * Quaternion.AngleAxis(gestureListener.getWheelAngle(), Vector3.down);
    }


}