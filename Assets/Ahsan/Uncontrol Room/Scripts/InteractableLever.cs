using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableLever : MonoBehaviour, IInteractable
{
    public float Value { get; set; }
    public bool IsOutlined { get; set; }

    public void Interact()
    {
        if (Keyboard.current.eKey.isPressed)
        {
            Value = 1;
        }

        else if (Keyboard.current.qKey.isPressed)
        {
            Value = -1;
        }

        else
        {
            Value = 0;
        }
    }

    public void ToggleOutline(bool value)
    {
        IsOutlined = value;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Interact();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ToggleOutline(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        ToggleOutline(false);
    }
}
