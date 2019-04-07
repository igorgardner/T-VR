using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearStick : MonoBehaviour {

    public GameObject stick;
    public GameObject axis;

    private GestureListener gestureListener;
    private int position;

    void Start ()
    {
        gestureListener = Camera.main.GetComponent<GestureListener>();
    }

    public void MoveForward()
    {
        if (position < 5)
        {
            stick.transform.RotateAround(axis.transform.position, Vector3.right, 10);
            position++;
        }
    }

    public void MoveBack()
    {
        if (position > -5)
        {
            stick.transform.RotateAround(axis.transform.position, -Vector3.right, 10);
            position--;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(gestureListener.IsPush())
        {
            if (position < 5)
            {
                stick.transform.RotateAround(axis.transform.position, Vector3.right, 10);
                position++;
            }
        }

        if(gestureListener.IsPull())
        {
            if(position > -5)
            {
                stick.transform.RotateAround(axis.transform.position, -Vector3.right, 10);
                position--;
            }
        }
        
    }
}
