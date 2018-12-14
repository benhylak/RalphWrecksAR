using UnityEngine.XR.iOS;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer

    public GameObject Reticle;

    public GameObject ArcadeCabinet;

    public bool placed = false;

    public Transform m_Camera;

    public UnityARCameraManager cameraManager;

    public GameObject m_Hammer;

    public Tween m_HammerSwingingAnim = null;

    public GameObject Ralph;

    public Toggle setupToggle;

    public GameObject windowPrefab;

    void Start()
    {
        ArcadeCabinet.SetActive(false);
    }

    void SetDebug(bool enabled)
    {
        cameraManager.getPointCloud = enabled;
    }

    public void StartSwingingHammer()
    {
        if(m_HammerSwingingAnim == null)
        {
            Debug.Log("GameManager: Start swingin!");

            m_HammerSwingingAnim = m_Hammer.transform
                .DOLocalRotate(new Vector3(0, -30, 0), 0.125f)
                .SetEase(Ease.InQuad)
                .SetLoops(-1, LoopType.Yoyo);

            m_HammerSwingingAnim.Play();
        }
        // else
        // {
        //     StopSwingingHammer();
        //     Debug.Log("GameManager: Stop swingin dummy.");
        // }
    }

    public void StopSwingingHammer()
    {   
        if(m_HammerSwingingAnim != null)
        {
            m_HammerSwingingAnim.SmoothRewind();
            m_HammerSwingingAnim = null;
        }
    }

    bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
                Debug.Log ("Got hit!");
                Reticle.transform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
                Reticle.transform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
                Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", Reticle.transform.position.x, Reticle.transform.position.y, Reticle.transform.position.z));
                return true;
            }
        }
        return false;
    }
    
    // Update is called once per frame
    void Update () {

        Reticle.SetActive(setupToggle.isOn);

        if(Input.touchCount == 1) 
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if(setupToggle.isOn && Reticle.transform.up == Vector3.up)
                {
                    ArcadeCabinet.SetActive(true);

                    ArcadeCabinet.transform.position = Reticle.transform.position;
                    Ralph.transform.position = Reticle.transform.position;

                    var cameraFloorPos = m_Camera.transform.position;
                    cameraFloorPos.y = ArcadeCabinet.transform.position.y;

                    var lookAtCameraFlattened = cameraFloorPos - ArcadeCabinet.transform.position;

                    ArcadeCabinet.transform.forward = lookAtCameraFlattened;
                    Ralph.transform.forward = lookAtCameraFlattened;
                }
                else
                {
                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);

                    if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Cabinet")
                    {
                        Ralph.GetComponent<RalphBehavior>().GrandEntrance();
                    }
                    else
                    {
                        var viewportPosition = Camera.main.ScreenToViewportPoint(touch.position);

                        ARPoint point = new ARPoint {
                            x = viewportPosition.x,
                            y = viewportPosition.y
                        };

                        WindowHitTest(point);
                    }        
                }
            }
        }
        
        UpdateReticle();
    }

    bool WindowHitTest(ARPoint point)
    {
        Debug.Log("GameManager: Window hit test");

        // prioritize reults types
        ARHitTestResultType[] resultTypes = {
            ARHitTestResultType.ARHitTestResultTypeExistingPlane,
            ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
        }; 

        List<ARHitTestResult> hitResults = null;

        foreach(var resType in resultTypes)
        {
            hitResults = UnityARSessionNativeInterface
                .GetARSessionNativeInterface ()
                .HitTest (point, ARHitTestResultType.ARHitTestResultTypeExistingPlane);

            if(hitResults.Count > 0) break;
        }
        
        if (hitResults.Count > 0) {
            var hitResult = hitResults.First();

            ARPlaneAnchorGameObject arAnchorGameObj = 
                UnityARAnchorManager
                    .Instance
                    .planeAnchorMap[hitResult.anchorIdentifier];
            
            if(arAnchorGameObj != null && arAnchorGameObj.planeAnchor.alignment == ARPlaneAnchorAlignment.ARPlaneAnchorAlignmentVertical)
            {
                var newWindow = Instantiate(windowPrefab);

                Debug.Log("GameManager: Made window!");

                newWindow.transform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
                
                //var rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform) * Quaternion.AngleAxis(90, Vector3.left);
                newWindow.transform.forward = arAnchorGameObj.gameObject.transform.up;
                newWindow.transform.parent = arAnchorGameObj.gameObject.GetComponentInChildren<MeshFilter>().transform;
               
                return true;
            }
        }

        return false;
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

    public void UpdateReticle()
    {
         ARPoint point = new ARPoint {
            x = 0.5f,
            y = 0.5f
        };

        // prioritize reults types
        ARHitTestResultType[] resultTypes = {
            ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
            ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
            ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane
        }; 
        
        foreach (ARHitTestResultType resultType in resultTypes)
        {
            if (HitTestWithResultType (point, resultType))
            {
                return;
            }
        }
    }
}
