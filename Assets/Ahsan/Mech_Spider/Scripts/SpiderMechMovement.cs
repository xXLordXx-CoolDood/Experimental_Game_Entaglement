using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderMechMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputAction frontLeftLegInput;
    [SerializeField] InputAction frontRightLegInput;
    [SerializeField] InputAction backLeftLegInput;
    [SerializeField] InputAction backRightLegInput;
    [SerializeField] InputAction leftTurnInput;
    [SerializeField] InputAction rightTurnInput;

    [Header("Targets")]
    [SerializeField] Transform frontLeftTarget;
    [SerializeField] Transform frontRightTarget;
    [SerializeField] Transform backLeftTarget;
    [SerializeField] Transform backRightTarget;

    [Header("Body")]
    [SerializeField] GameObject body;

    [Header("Values")]
    [SerializeField] float stepLength;

    private void OnEnable()
    {
        frontLeftLegInput.Enable();
        frontRightLegInput.Enable();
        backLeftLegInput.Enable();
        backRightLegInput.Enable();
        leftTurnInput.Enable();
        rightTurnInput.Enable();
    }

    private void OnDisable()
    {
        frontLeftLegInput.Disable();
        frontRightLegInput.Disable();
        backLeftLegInput.Disable();
        backRightLegInput.Disable();
        leftTurnInput.Disable();
        rightTurnInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        frontLeftLegInput.performed += FrontLeftLeg;
        frontRightLegInput.performed += FrontRightLeg;
        backLeftLegInput.performed += BackLeftLeg;
        backRightLegInput.performed += BackRightLeg;
    }

    private void BackRightLeg(InputAction.CallbackContext obj)
    {
        backRightTarget.transform.position += transform.forward * stepLength;
    }

    private void BackLeftLeg(InputAction.CallbackContext obj)
    {
        backLeftTarget.transform.position += transform.forward * stepLength;
    }

    private void FrontRightLeg(InputAction.CallbackContext obj)
    {
        frontRightTarget.transform.position += transform.forward * stepLength;
    }

    private void FrontLeftLeg(InputAction.CallbackContext obj)
    {
        frontLeftTarget.transform.position += transform.forward * stepLength;
    }

    // Update is called once per frame
    void Update()
    {
        var bodyPosition =(
            frontLeftTarget.position +
            frontRightTarget.position +
            backLeftTarget.position +
            backRightTarget.position
        ) / 4;

        body.transform.position = bodyPosition;
    }
}
