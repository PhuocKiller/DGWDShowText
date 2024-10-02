using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        Singleton<CinemachineBrain>.Instance.m_ShowDebugText = true;
    }

    public void SetFollowRobo(Transform roboTransform)
    {
        virtualCamera.Follow = roboTransform;
    }
    public void RemoveFollowRobo()
    {
        virtualCamera.Follow = null;
    }
}
