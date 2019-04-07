using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Linq;
using System.Text;
using System.IO.Ports;

public class SteeringWheel : MonoBehaviour
{
    //public GameObject Wheel;
    Quaternion defaultRotation;

    private GestureListener gestureListener;
    private SerialPort port = new SerialPort("COM3",
      9600, Parity.None, 8, StopBits.One);

    void Start()
    {
        defaultRotation = transform.localRotation;
        gestureListener = Camera.main.GetComponent<GestureListener>();
        port.Open();

    }

    void Update()
    {
        transform.rotation = defaultRotation * Quaternion.AngleAxis(gestureListener.getWheelAngle(), Vector3.down);
        if (defaultRotation.z < transform.localRotation.z)
        {
            port.Write("1000");
        }
    }


}