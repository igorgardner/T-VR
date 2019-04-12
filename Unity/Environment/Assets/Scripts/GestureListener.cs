using UnityEngine;
using System.Collections;
using System;

public class GestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	private bool push;
	private bool pull;
    private float wheelAngle;
	
	public bool IsPush()
	{
		if(push)
		{
			push = false;
			return true;
		}
		
		return false;
	}
	
	public bool IsPull()
	{
		if(pull)
		{
			pull = false;
			return true;
		}
		
		return false;
	}

    public float getWheelAngle()
    {
        return wheelAngle;
    }

	
	public void UserDetected(uint userId, int userIndex)
	{
		// detect these user specific gestures
		KinectManager manager = KinectManager.Instance;
		
        manager.DetectGesture(userId, KinectGestures.Gestures.Wheel);
        manager.DetectGesture(userId, KinectGestures.Gestures.RightAboveHead);
        manager.DetectGesture(userId, KinectGestures.Gestures.LeftAboveHead);

	}
	
	public void UserLost(uint userId, int userIndex)
	{

	}

	public void GestureInProgress(uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
        if (gesture == KinectGestures.Gestures.Wheel)
            wheelAngle = screenPos.z;

    }

	public bool GestureCompleted (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		Debug.Log(gesture + " detected");
		
		
		if(gesture == KinectGestures.Gestures.LeftAboveHead)
			pull = true;
		else if(gesture == KinectGestures.Gestures.RightAboveHead)
			push = true;

		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint)
	{
		if (gesture == KinectGestures.Gestures.Wheel)
            wheelAngle = 0;

		return true;
	}
	
}
