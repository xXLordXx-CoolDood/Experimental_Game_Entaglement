using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] Camera[] cameraList;   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            cameraList[0].enabled = false;
            cameraList[1].enabled = true;
        }

        if (Keyboard.current.enterKey.isPressed)
        {
            cameraList[1].enabled = false;
            cameraList[0].enabled = true;
        }
    }
}
