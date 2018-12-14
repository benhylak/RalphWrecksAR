using UnityEngine.XR.iOS;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

public class WallManager : MonoBehaviour
{
    public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer

    public GameObject wallPrefab;

    GameObject[] walls;
    
    float nextUpdateTime = 0f;

    float updateInterval = 0.5f;

    public bool shouldUpdate = true;


    public Dictionary<string, GameObject> anchorWallMap = new Dictionary<string, GameObject>();

    void Start()
    {
         UnityARSessionNativeInterface.ARAnchorUpdatedEvent += EvaluateWall;
    }

    void EvaluateWall(ARPlaneAnchor anchor)
    {
        if(anchorWallMap.ContainsKey(anchor.identifier))
        {
            UpdateWall(anchor);
        }
        else if(anchor.alignment == ARPlaneAnchorAlignment.ARPlaneAnchorAlignmentVertical && anchor.extent.magnitude > 2)
        {
            BuildTheWall(anchor);
        }
    }

    void UpdateWall(ARPlaneAnchor anchor)
    {
        if(shouldUpdate)
        {
            GameObject wall = anchorWallMap[anchor.identifier];

            ARPlaneAnchorGameObject arAnchorGameObj = 
                UnityARAnchorManager
                    .Instance
                    .planeAnchorMap[anchor.identifier];

            if (arAnchorGameObj != null) {
                
                var mf = arAnchorGameObj.gameObject.GetComponentInChildren<MeshFilter>();

                wall.transform.localScale = new Vector3(
                    mf.transform.localScale.x * 10,
                    mf.transform.localScale.z * 10,
                    0.2f
                );

                wall.transform.position = mf.transform.position;
                wall.transform.forward = mf.transform.up;

                wall.transform.position -= wall.transform.forward * wall.transform.localScale.z/2;   
            }
        }
    }

    void BuildTheWall(ARPlaneAnchor anchor)
    {
        var newWall = Instantiate(wallPrefab);
        
        anchorWallMap.Add(anchor.identifier, newWall);

        UpdateWall(anchor);
    }

    // void Update()
    // {
    //     if(Input.touchCount > 0) 
    //     {
    //         var touch = Input.GetTouch(0);

    //         if (touch.phase == TouchPhase.Ended)
    //         {
    //             var viewportPosition = Camera.main.ScreenToViewportPoint(touch.position);

    //             ARPoint point = new ARPoint {
    //                 x = viewportPosition.x,
    //                 y = viewportPosition.y
    //             };

    //             // prioritize reults types
    //             ARHitTestResultType[] resultTypes = {
    //                 ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
    //                 ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
    //             }; 
                
    //             foreach (ARHitTestResultType resultType in resultTypes)
    //             {
    //                 if (HitTestWithResultType (point, resultType))
    //                 {
    //                     return;
    //                 }
    //             }
    //         }
    //     }
    // }

    // bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
    // {
    //     List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
    //     if (hitResults.Count > 0) {
    //         var hitResult = hitResults.First();
    //         //foreach (var hitResult in hitResults) {
    //             Debug.Log ("Got hit!");

    //             //instantiate the brick walls

                

    //             // transform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
    //             // transform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
    //             //Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", Reticle.transform.position.x, Reticle.transform.position.y, Reticle.transform.position.z));
    //             return true;
    //        // }
    //     }
    //     return false;
    // }

    // public void UpdateAnchor(ARPlaneAnchor anchor)
    // {
    //     if(wallAnchorMap.ContainsKey(anchor.identifier))
    //     {
    //         var wall = wallAnchorMap[anchor.identifier];
    //         //update wall
    //     }
    // }

//    public void UpdateWall(anchor)
}