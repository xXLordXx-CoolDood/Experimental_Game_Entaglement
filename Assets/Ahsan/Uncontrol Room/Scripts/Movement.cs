using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Tooltip("Speed at which the character is moving")][SerializeField] float strafeSpeed = 15;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            transform.Translate(transform.forward * Time.deltaTime * strafeSpeed, Space.World);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.Translate(-transform.right * Time.deltaTime * strafeSpeed, Space.World);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            transform.Translate(-transform.forward * Time.deltaTime * strafeSpeed, Space.World);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            transform.Translate(transform.right * Time.deltaTime * strafeSpeed, Space.World);
        }

        transform.Rotate(transform.up, Mouse.current.delta.ReadValue().x, Space.World);
    }
}
