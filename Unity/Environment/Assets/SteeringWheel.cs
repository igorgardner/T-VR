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

    void Start()
    {
        defaultRotation = transform.localRotation;

    }

    public void SetRotation(float rotation)
    {
        transform.rotation = defaultRotation * Quaternion.AngleAxis(rotation, Vector3.up);
    }


    void Update()
    {

    }
}