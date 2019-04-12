using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectGestures
{

	public interface GestureListenerInterface
	{
		// Invoked when a new user is detected and tracking starts
		// Here you can start gesture detection with KinectManager.DetectGesture()
		void UserDetected(uint userId, int userIndex);
		
		// Invoked when a user is lost
		// Gestures for this user are cleared automatically, but you can free the used resources
		void UserLost(uint userId, int userIndex);
		
		// Invoked when a gesture is in progress 
		void GestureInProgress(uint userId, int userIndex, Gestures gesture, float progress, 
		                       KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos);

		// Invoked if a gesture is completed.
		// Returns true, if the gesture detection must be restarted, false otherwise
		bool GestureCompleted(uint userId, int userIndex, Gestures gesture,
		                      KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos);

		// Invoked if a gesture is cancelled.
		// Returns true, if the gesture detection must be retarted, false otherwise
		bool GestureCancelled(uint userId, int userIndex, Gestures gesture, 
		                      KinectWrapper.NuiSkeletonPositionIndex joint);
	}
	
	
	public enum Gestures
	{
		None = 0,
		SwipeLeft,
		SwipeRight,
		Wheel,
        RightAboveHead,
        LeftAboveHead
	}
	
	
	public struct GestureData
	{
		public uint userId;
		public Gestures gesture;
		public int state;
		public float timestamp;
		public int joint;
		public Vector3 jointPos;
		public Vector3 screenPos;
		public float tagFloat;
		public Vector3 tagVector;
		public Vector3 tagVector2;
		public float progress;
		public bool complete;
		public bool cancelled;
		public List<Gestures> checkForGestures;
		public float startTrackingAtTime;
	}
	

	
	// Gesture related constants, variables and functions
	private const int leftHandIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft;
	private const int rightHandIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight;
		
	private const int leftElbowIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft;
	private const int rightElbowIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight;
		
	private const int leftShoulderIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft;
	private const int rightShoulderIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight;
	
	private const int hipCenterIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter;
	private const int shoulderCenterIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter;
	private const int leftHipIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft;
	private const int rightHipIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HipRight;
    private const int headIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.Head;
	
	
	private static void SetGestureJoint(ref GestureData gestureData, float timestamp, int joint, Vector3 jointPos)
	{
		gestureData.joint = joint;
		gestureData.jointPos = jointPos;
		gestureData.timestamp = timestamp;
		gestureData.state++;
	}
	
	private static void SetGestureCancelled(ref GestureData gestureData)
	{
		gestureData.state = 0;
		gestureData.progress = 0f;
		gestureData.cancelled = true;
	}
	
	private static void CheckPoseComplete(ref GestureData gestureData, float timestamp, Vector3 jointPos, bool isInPose, float durationToComplete)
	{
		if(isInPose)
		{
			float timeLeft = timestamp - gestureData.timestamp;
			gestureData.progress = durationToComplete > 0f ? Mathf.Clamp01(timeLeft / durationToComplete) : 1.0f;
	
			if(timeLeft >= durationToComplete)
			{
				gestureData.timestamp = timestamp;
				gestureData.jointPos = jointPos;
				gestureData.state++;
				gestureData.complete = true;
			}
		}
		else
		{
			SetGestureCancelled(ref gestureData);
		}
	}
	
	private static void SetScreenPos(uint userId, ref GestureData gestureData, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		Vector3 handPos = jointsPos[rightHandIndex];
		bool calculateCoords = false;
		
		if(gestureData.joint == rightHandIndex)
		{
			if(jointsTracked[rightHandIndex])
			{
				calculateCoords = true;
			}
		}
		else if(gestureData.joint == leftHandIndex)
		{
			if(jointsTracked[leftHandIndex])
			{
				handPos = jointsPos[leftHandIndex];
				
				calculateCoords = true;
			}
		}
		
		if(calculateCoords)
		{
			
			if(jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && 
				jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex])
			{
				Vector3 neckToHips = jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex];
				Vector3 rightToLeft = jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex];
				
				gestureData.tagVector2.x = rightToLeft.x; // * 1.2f;
				gestureData.tagVector2.y = neckToHips.y; // * 1.2f;
				
				if(gestureData.joint == rightHandIndex)
				{
					gestureData.tagVector.x = jointsPos[rightShoulderIndex].x - gestureData.tagVector2.x / 2;
					gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
				}
				else
				{
					gestureData.tagVector.x = jointsPos[leftShoulderIndex].x - gestureData.tagVector2.x / 2;
					gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
				}
			}
			
			if(gestureData.tagVector2.x != 0 && gestureData.tagVector2.y != 0)
			{
				Vector3 relHandPos = handPos - gestureData.tagVector;
				gestureData.screenPos.x = Mathf.Clamp01(relHandPos.x / gestureData.tagVector2.x);
				gestureData.screenPos.y = Mathf.Clamp01(relHandPos.y / gestureData.tagVector2.y);
			}
		}
	}
	
	private static void SetZoomFactor(uint userId, ref GestureData gestureData, float initialZoom, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		Vector3 vectorZooming = jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
		
		if(gestureData.tagFloat == 0f || gestureData.userId != userId)
		{
			gestureData.tagFloat = 0.5f; // this is 100%
		}

		float distZooming = vectorZooming.magnitude;
		gestureData.screenPos.z = initialZoom + (distZooming / gestureData.tagFloat);
	}
	
	
	// estimate the next state and completeness of the gesture
	public static void CheckForGesture(uint userId, ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		if(gestureData.complete)
			return;
		
		float bandSize = (jointsPos[shoulderCenterIndex].y - jointsPos[hipCenterIndex].y);
		float gestureTop = jointsPos[shoulderCenterIndex].y + bandSize / 2;
		float gestureBottom = jointsPos[shoulderCenterIndex].y - bandSize;
		float gestureRight = jointsPos[rightHipIndex].x;
		float gestureLeft = jointsPos[leftHipIndex].x;
		
		switch(gestureData.gesture)
		{
			// check for SwipeLeft
			case Gestures.SwipeLeft:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
//						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
//					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
//					       (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0f)
//						{
//							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
//							gestureData.progress = 0.5f;
//						}

						if(jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
							jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
				   			jointsPos[rightHandIndex].x <= gestureRight && jointsPos[rightHandIndex].x > gestureLeft)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.1f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
//							bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
//								Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < 0.1f && 
//								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.08f && 
//								(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < -0.15f;

							bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
									jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
									jointsPos[rightHandIndex].x < gestureLeft;
							
							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
							else if(jointsPos[rightHandIndex].x <= gestureRight)
							{
								float gestureSize = gestureRight - gestureLeft;
								gestureData.progress = gestureSize > 0.01f ? (gestureRight - jointsPos[rightHandIndex].x) / gestureSize : 0f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for SwipeRight
			case Gestures.SwipeRight:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
//						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
//					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
//					            (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < 0f)
//						{
//							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
//							gestureData.progress = 0.5f;
//						}
				
						if(jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
				   			jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
				   			jointsPos[leftHandIndex].x >= gestureLeft && jointsPos[leftHandIndex].x < gestureRight)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.1f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
//							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
//								Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < 0.1f &&
//								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.08f && 
//								(jointsPos[leftHandIndex].x - gestureData.jointPos.x) > 0.15f;

							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
									jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
									jointsPos[leftHandIndex].x > gestureRight;
							
							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
							else if(jointsPos[leftHandIndex].x >= gestureLeft)
							{
								float gestureSize = gestureRight - gestureLeft;
								gestureData.progress = gestureSize > 0.01f ? (jointsPos[leftHandIndex].x - gestureLeft) / gestureSize : 0f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for Wheel
			case Gestures.Wheel:
				Vector3 vectorWheel = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
				float distWheel = vectorWheel.magnitude;
				
//				Debug.Log(string.Format("{0}. Dist: {1:F1}, Tag: {2:F1}, Diff: {3:F1}", gestureData.state,
//				                        distWheel, gestureData.tagFloat, Mathf.Abs(distWheel - gestureData.tagFloat)));

				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightElbowIndex] && jointsTracked[shoulderCenterIndex] &&
						   jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
						   jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           (jointsPos[leftHandIndex].y > jointsPos[leftElbowIndex].y || jointsPos[rightHandIndex].y > jointsPos[rightElbowIndex].y) &&
                           (jointsPos[leftHandIndex].y < jointsPos[headIndex].y && jointsPos[rightHandIndex].y < jointsPos[headIndex].y) &&
                           distWheel >= 0.3f && distWheel < 0.7f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.tagVector = Vector3.right;
							gestureData.tagFloat = distWheel;
							gestureData.progress = 0.3f;
						}
						break;

					case 1:  // gesture phase 2 = turning wheel
						if((timestamp - gestureData.timestamp) < 0.15f)
						{
							float angle = Vector3.Angle(gestureData.tagVector, vectorWheel) * Mathf.Sign(vectorWheel.y - gestureData.tagVector.y);
							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightElbowIndex] && jointsTracked[shoulderCenterIndex] &&
                                            jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                            jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                            (jointsPos[leftHandIndex].y > jointsPos[leftElbowIndex].y || jointsPos[rightHandIndex].y > jointsPos[rightElbowIndex].y) &&
                                            (jointsPos[leftHandIndex].y < jointsPos[headIndex].y && jointsPos[rightHandIndex].y < jointsPos[headIndex].y) &&
                                            distWheel >= 0.3f && distWheel < 0.7f && 
								            Mathf.Abs(distWheel - gestureData.tagFloat) < 0.1f;
							
							if(isInPose)
							{
								//SetWheelRotation(userId, ref gestureData, gestureData.tagVector, vectorWheel);
								gestureData.screenPos.z = angle;  // wheel angle
								gestureData.timestamp = timestamp;
								gestureData.tagFloat = distWheel;
								gestureData.progress = 0.7f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;

				}
				break;

            case Gestures.RightAboveHead:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[headIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[headIndex].y) > 0.1f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = (jointsTracked[rightHandIndex] && jointsTracked[headIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[headIndex].y) > 0.05f);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0.1f);
                        break;
                }
                break;

            case Gestures.LeftAboveHead:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[leftHandIndex] && jointsTracked[headIndex] &&
                           (jointsPos[leftHandIndex].y - jointsPos[headIndex].y) > 0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = (jointsTracked[leftHandIndex] && jointsTracked[headIndex] &&
                           (jointsPos[leftHandIndex].y - jointsPos[headIndex].y) > 0.05f);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0.1f);
                        break;
                }
                break;
                
                // here come more gesture-cases
        }
	}

}
