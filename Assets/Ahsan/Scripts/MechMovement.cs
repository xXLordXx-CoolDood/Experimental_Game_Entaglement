using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechMovement : MonoBehaviour
{
    MechStates state = MechStates.Based;
    Animator animator;
    Vector2 m_rotation;
    float Balance = 0;
    float BalanceOffset = 0;

    [Header("Feet")]
    [SerializeField] Transform LeftFoot;
    [SerializeField] Transform RightFoot;


    [Header("Movement Values")]
    [SerializeField] float turnSpeed;

    [Header("Inputs")]
    [SerializeField] InputAction TriggerRight;
    [SerializeField] InputAction TriggerLeft;
    [SerializeField] InputAction TurnInput;


    private void OnEnable()
    {
        TriggerLeft.Enable();
        TriggerRight.Enable();
        TurnInput.Enable();
    }

    private void OnDisable()
    {
        TriggerLeft.Disable();
        TriggerRight.Disable();
        TurnInput.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        TriggerRight.performed += TriggerRight_performed;
        TriggerLeft.performed += TriggerLeft_performed;
    }



    private void TriggerLeft_performed(InputAction.CallbackContext ctx)
    {
        LeftLeg();
    }

    private void TriggerRight_performed(InputAction.CallbackContext ctx)
    {
        RightLeg();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case MechStates.Based:
                ResetBalance();
                Turn(); 
                break;
            case MechStates.LeftUp:
            case MechStates.RightUp:
                OffBalance();
                Sway();
                break;
            default:
                break;
        }
    }

    private void Sway()
    {
        Transform rotationPoint = state switch
        {
            MechStates.LeftUp => RightFoot.transform,
            MechStates.RightUp => LeftFoot.transform,
            MechStates.Based => transform,
            _ => transform,
        };

        transform.RotateAround(rotationPoint.position, transform.forward, Balance * Time.deltaTime);
    }

    private void OffBalance()
    {
        Balance = Mathf.Sin(Time.realtimeSinceStartup) * BalanceOffset;
        BalanceOffset += Time.deltaTime;
    }

    private void ResetBalance()
    {
        Balance = 0;
        BalanceOffset = 0;
    }

    private void Turn()
    {
        var turnValue = TurnInput.ReadValue<float>();
        m_rotation.y += turnValue * Time.deltaTime * turnSpeed;
        transform.eulerAngles = m_rotation;
    }

    void LeftLeg()
    {
        switch (state)
        {
            case MechStates.Based:
                animator.SetTrigger("LeftLeg");
                state = MechStates.LeftUp;
                break;
            case MechStates.LeftUp:
                animator.SetTrigger("LeftLeg");
                state = MechStates.Based;
                break;
            default:
                break;
        }
    }

    void RightLeg()
    {
        switch (state)
        {
            case MechStates.Based:
                animator.SetTrigger("RightLeg");
                state = MechStates.RightUp;
                break;
            case MechStates.RightUp:
                animator.SetTrigger("RightLeg");
                state = MechStates.Based;
                break;
            default:
                break;
        }
    }

}
