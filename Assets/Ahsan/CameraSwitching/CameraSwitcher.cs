using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] Camera[] cameraList;   

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            CycleCamera();
        }
    }

    public void CycleCamera() {
        int index = Array.IndexOf(cameraList, Array.Find(cameraList, x => x.enabled));
        cameraList[index].enabled = false;
        cameraList[(index + 1)  % cameraList.Length].enabled = true;
    }
}
