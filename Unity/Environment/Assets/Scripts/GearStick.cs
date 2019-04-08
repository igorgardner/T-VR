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

    public string GetPosition()
    {
        return position.ToString();
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.W)) // above head guesture
        {
            if (position < 2)
            {
                stick.transform.RotateAround(axis.transform.position, Vector3.right, 20);
                position++;
            }
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if(position > -1)
            {
                stick.transform.RotateAround(axis.transform.position, -Vector3.right, 20);
                position--;
            }
        }

        if (gestureListener.IsPush()) // above head guesture
        {
            if (position < 2)
            {
                stick.transform.RotateAround(axis.transform.position, Vector3.right, 20);
                position++;
            }
        }
        else if (gestureListener.IsPull())
        {
            if (position > -1)
            {
                stick.transform.RotateAround(axis.transform.position, -Vector3.right, 20);
                position--;
            }
        }

    }
}
