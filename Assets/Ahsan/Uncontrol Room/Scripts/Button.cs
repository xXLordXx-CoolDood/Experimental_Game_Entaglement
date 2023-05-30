using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
    [SerializeField] UnityEvent TargetFunction;
    [SerializeField] InputAction buttonInput;
    bool isActive;

    private void OnEnable()
    {
        buttonInput.Enable();
    }
    private void OnDisable()
    {
        buttonInput.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && buttonInput.IsPressed())
        {
            transform.Find("ButtonSwitch").position = new Vector3(0, 0.1f, 0);
            TargetFunction.Invoke();
        }
        else
        {
            transform.Find("ButtonSwitch").position = new Vector3(0, 0.3f, 0);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }
}
