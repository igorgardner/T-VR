using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearStick : MonoBehaviour {

    public GameObject stick;
    public GameObject axis;
    public int turnSpeed;
    public int position;

	// Use this for initialization
	void Start () {
		
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
    void Update () {

        if (Input.GetKey(KeyCode.W))
        {
            while(Input.GetKey(KeyCode.W))
            {
                stick.transform.RotateAround(axis.transform.position, Vector3.right, 5);
                position++;

                if (position>5)
                {
                    
                }
            }
            
        }
        else if(Input.GetKey(KeyCode.S))
        {
            while(Input.GetKey(KeyCode.S))
            {
                stick.transform.RotateAround(axis.transform.position, -Vector3.right, 5);
                position--;

                if (position < -5)
                {

                }
            }
        }
    }
}
