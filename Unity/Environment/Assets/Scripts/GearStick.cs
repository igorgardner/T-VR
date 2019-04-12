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
        switch(position)
        {
            case 2:
                return "7";

            case 1:
                return "6";

            case 0:
                return "5";

            case -1:
                return "4";

            default:
                return "0";
        }
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
