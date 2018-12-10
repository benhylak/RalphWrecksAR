using UnityEngine.XR.iOS;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using DG.Tweening;

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
            m_HammerSwingingAnim = m_Hammer.transform
                .DOLocalRotate(new Vector3(0, -30, 0), 0.35f)
                .SetEase(Ease.InBack)
                .SetLoops(-1, LoopType.Yoyo);

            m_HammerSwingingAnim.Play();
        }
    }

    public void StopSwingingHammer()
    {   
        if(m_HammerSwingingAnim != null)
        {
            m_HammerSwingingAnim.Rewind();
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
        if(Input.touchCount == 1) 
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if(!placed)
                {
                    placed = true;
                    ArcadeCabinet.SetActive(true);

                    ArcadeCabinet.transform.position = Reticle.transform.position;
                    Ralph.transform.position = Reticle.transform.position;

                    var cameraFloorPos = m_Camera.transform.position;
                    cameraFloorPos.y = ArcadeCabinet.transform.position.y;

                    var lookAtCameraFlattened = cameraFloorPos - ArcadeCabinet.transform.position;

                    ArcadeCabinet.transform.forward = lookAtCameraFlattened;
                    Ralph.transform.forward = lookAtCameraFlattened;
                
                    Reticle.SetActive(false);
                }
                else
                {
                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if(hit.transform.tag == "Cabinet")
                        {
                            Ralph.GetComponent<RalphBehavior>().GrandEntrance();
                            return;
                        }
                    } 

                    placed = false;
                    Reticle.SetActive(true);         
                }
            }
        }
        
        UpdateReticle();
    }

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
