using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechMovement : MonoBehaviour
{
    MechStates state = MechStates.Based;
    Quaternion baseRotation;

    [SerializeField] Animator animator;

    [Header("Feet")]
    [SerializeField] Transform LeftFoot;
    [SerializeField] Transform RightFoot;
    [SerializeField] float swayRate;
    [SerializeField] float swayLength;
    [SerializeField] float returnSpeed;
    float Balance = 0;
    float BalanceOffset = 0;


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
        baseRotation = transform.rotation;

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

    public IEnumerator ReturnToPosition()
    {
        switch (state)
        {
            case MechStates.LeftUp:
                state = MechStates.Orienting;
                while (transform.rotation != baseRotation)
                {
                    transform.RotateAround(RightFoot.position, transform.forward, Time.deltaTime * ((transform.eulerAngles.z > 180) ? 1 : -1) * returnSpeed);
                    yield return new WaitForEndOfFrame();
                }
                break;
            case MechStates.RightUp:
                state = MechStates.Orienting;
                while (transform.rotation != baseRotation)
                {
                    transform.RotateAround(LeftFoot.position, transform.forward, Time.deltaTime * ((transform.eulerAngles.z > 180) ? 1 : -1) * returnSpeed);
                    yield return new WaitForEndOfFrame();
                }
                break;
        }
        state = MechStates.Based;
    }

    private void Sway()
    {
        switch (state)
        {
            case MechStates.LeftUp:
                transform.RotateAround(RightFoot.position, transform.forward, Balance * Time.deltaTime);
                break;
            case MechStates.RightUp:
                transform.RotateAround(LeftFoot.position, transform.forward, Balance * Time.deltaTime);
                break;
        }
    }

    private void OffBalance()
    {
        Balance = Mathf.Sin(Time.realtimeSinceStartup * swayRate) * BalanceOffset;
        BalanceOffset += Time.deltaTime * swayLength;
    }

    private void ResetBalance()
    {
        Balance = 0;
        BalanceOffset = 0;
    }

    private void Turn()
    {
        var turnValue = TurnInput.ReadValue<float>() * Time.deltaTime * turnSpeed;
        transform.Rotate(transform.up, turnValue);
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
                StartCoroutine("ReturnToPosition");
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
                StartCoroutine("ReturnToPosition");
                break;
            default:
                break;
        }
    }
}
