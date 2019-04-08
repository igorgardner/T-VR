using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;

public class SerialComs : MonoBehaviour
{
    public string comPort;
    
    public SteeringWheel wheel;
    public GearStick stick;

    private SerialPort port;

    // Start is called before the first frame update
    void Start()
    {
        port = new SerialPort(comPort, 9600);
        port.Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (port.IsOpen)
        {
            port.WriteLine("instruct");
            port.WriteLine(wheel.GetAngle());
            port.WriteLine(stick.GetPosition());
        }
    }
}
