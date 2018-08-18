using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARFurnitureManager : MonoBehaviour
{

    public Camera FirstPersonCamera;

    public GameObject DetectedPlanePrefab;

    public GameObject ARFurniture;

    private List<DetectedPlane> m_AllPlane = new List<DetectedPlane>();

    bool m_IsQuitting;

    void Update()
    {
        _UpdateApplicationLifecycle();

        // ARCoreで検出した面を取得
        Session.GetTrackables<DetectedPlane>(m_AllPlane);
        bool showSerchingUI = true;
        for (int i = 0; i < m_AllPlane.Count; i++)
        {
            showSerchingUI = false;
            break;
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                GameObject prefab;
                prefab = ARFurniture;
            }
            var furniture = Instantiate(DetectedPlanePrefab, hit.Pose.position, hit.Pose.rotation);
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            furniture.transform.parent = anchor.transform;
        }
    }
    private void _UpdateApplicationLifecycle()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }
    private void _DoQuit()
    {
        Application.Quit();
    }
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widset.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
             {
                 AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                     "makeText", unityActivity, message, 0);
                 toastObject.Call("show");
             }));
        }
    }
}
