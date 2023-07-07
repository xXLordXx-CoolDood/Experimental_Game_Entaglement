using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using FMODUnity;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private EventReference camSwitchSound;
    public Camera[] cameraList;
    [SerializeField] GameObject gunUI;

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            CycleCamera();
        }
    }

    public void CycleCamera() {
        Audio_Manager.instance.PlayOneShot(camSwitchSound, transform.position);

        int index = Array.IndexOf(cameraList, Array.Find(cameraList, x => x.enabled));
        cameraList[index].enabled = false;
        cameraList[(index + 1)  % cameraList.Length].enabled = true;

        if (cameraList[1].enabled) { gunUI.SetActive(true); GetComponent<Mech_Controller>().isAiming = true; Debug.Log("Aiming enabled"); }
        else { gunUI.SetActive(false); GetComponent<Mech_Controller>().isAiming = false; }
    }
}
