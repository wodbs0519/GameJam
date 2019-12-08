using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Transform target;

    public CinemachineImpulseSource SoftImpulse;
    public CinemachineImpulseSource HardImpulse;
    

    private Camera mainCam;

    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(CameraManager)) as CameraManager;

                if (instance == null)
                    Debug.LogError("There's no active ManagerClass object");
            }
            return instance;
        }
    }

    private void Awake()
    {
        mainCam = Camera.main;
        target = player;
    }

}

