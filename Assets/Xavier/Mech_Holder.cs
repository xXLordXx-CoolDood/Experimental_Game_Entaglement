using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Mech_Holder : MonoBehaviour
{
    [SerializeField] private GameObject mech, cameraFollow, initialCamera;
    private GameObject debris;

    public void MechDie(GameObject _debris)
    {
        mech.SetActive(false);
        debris = _debris;
        GameObject cam = Instantiate(cameraFollow, debris.transform.position, debris.transform.rotation);
        Debug.Log(cam.name);
        cam.transform.Translate(Vector3.back * 10 + Vector3.right * 10 + Vector3.up * 20, Space.World);
        cam.transform.LookAt(debris.transform);
    }

    private void Update()
    {
        if (!ArduinoDevice.current.gun1down.isPressed && !ArduinoDevice.current.gun1up.isPressed &&
            !ArduinoDevice.current.gun2down.isPressed && !ArduinoDevice.current.gun2up.isPressed &&
            !ArduinoDevice.current.gun3down.isPressed && !ArduinoDevice.current.gun3up.isPressed &&
            ArduinoDevice.current.shootLeft.isPressed && ArduinoDevice.current.shootRight.isPressed && !mech.activeInHierarchy)
        {
            if(initialCamera != null) { Destroy(initialCamera); }
            mech.SetActive(true);
            mech.GetComponent<Mech_Controller>().Respawn();
            Destroy(debris);
        }

        if(Keyboard.current.spaceKey.wasPressedThisFrame && !mech.activeInHierarchy)
        {
            if (initialCamera != null) { Destroy(initialCamera); }
            mech.SetActive(true);
            mech.GetComponent<Mech_Controller>().Respawn();
            Destroy(debris);
        }
    }

}
