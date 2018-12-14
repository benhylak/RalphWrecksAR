using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
public class WalkingStick : MonoBehaviour {
	public Transform camera;

  	//public Text DistanceText;
	public AudioSource audioData;
	public Toggle stickToggle;
	public float nextActionTime = 0.0f;
	public float nextActionTime2 = 0.0f;
	public float period = 0.25f;
	public float dist = 0.0f;
	public float bValue = 0.0f;

	// Use this for initialization
	void Start () 
	{
		//audioData.Play(0);
	}

	 bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
              //  Debug.Log ("purple");
                var pos = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
				dist = Vector3.Distance(pos,camera.transform.position);
				Debug.Log ("hey QT");

				var min = 0f;
				var max = 5f;

				float normal = Mathf.InverseLerp(min, max, dist);
				bValue = Mathf.Lerp(max, min, normal);
				
                return true;
            }
        }
        return false;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if (Time.time > nextActionTime ) 
		{ 
			nextActionTime = Time.time + period; 
			
			ARPoint point = new ARPoint 
			{
              	x = 0.5f,
              	y = 0.5f
           	};

           	// prioritize reults types
           	ARHitTestResultType[] resultTypes = 
			{
              	ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
              	ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
              	ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
               	ARHitTestResultType.ARHitTestResultTypeFeaturePoint
            }; 
            
            foreach (ARHitTestResultType resultType in resultTypes)
            {
              	if (HitTestWithResultType (point, resultType))
               	{
                return;
                }
			}
		}
		
		if (stickToggle.isOn) 
		{
			if (Time.time > nextActionTime2)
			{
				nextActionTime2 = Time.time + dist/2;
				if (audioData.isPlaying) audioData.Stop ();
				audioData.pitch = bValue;
				if (!audioData.isPlaying) audioData.Play(0);
			}
		}
		else audioData.Stop();

		//DistanceText.text = "hellooooo " + dist;

    }
}