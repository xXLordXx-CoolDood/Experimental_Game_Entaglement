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
    [Tooltip("Point of swaying when the other foot is raised")][SerializeField] Transform LeftFoot;
    [Tooltip("Point of swaying when the other foot is raised")][SerializeField] Transform RightFoot;
    [Tooltip("Makes it wiggle faster")][SerializeField] float swayRate;
    [Tooltip("Makes it wiggle over longer distances")][SerializeField] float swayLength;
    [Tooltip("How fast it comes back in place")][SerializeField] float returnSpeed;
    [Tooltip("How quickly balance is offset to the other side (Needs to be more than swayLength)")][SerializeField] float balanceValue;
    [Tooltip("Only lower leg when Balance is between the positive and negative of this value")][SerializeField] float BalanceMax;

    float Balance = 0;
    float BalanceOffset = 0;


    [Header("Movement Values")]
    [Tooltip("How fast it rotates")][SerializeField] float turnSpeed;

    [Header("Inputs")]
    [Tooltip("Makes the right leg go up and down")][SerializeField] InputAction TriggerRight;
    [Tooltip("Makes the left leg go up and down")][SerializeField] InputAction TriggerLeft;
    [Tooltip("Input for turning")][SerializeField] InputAction TurnInput;
    [Tooltip("Input for balancing when on one leg")][SerializeField] InputAction BalanceInput;

    private void OnEnable()
    {
        TriggerLeft.Enable();
        TriggerRight.Enable();
        TurnInput.Enable();
        BalanceInput.Enable();
    }

    private void OnDisable()
    {
        TriggerLeft.Disable();
        TriggerRight.Disable();
        TurnInput.Disable();
        BalanceInput.Disable();
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
                MakeBalance();
                Sway();
                break;
            default:
                break;
        }
    }

    private void MakeBalance()
    {
        var turnValue = BalanceInput.ReadValue<float>() * Time.deltaTime;
        BalanceOffset -= turnValue * balanceValue;
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
                if (-BalanceMax < Balance && Balance < BalanceMax)
                {
                    animator.SetTrigger("LeftLeg");
                    StartCoroutine("ReturnToPosition");
                }
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
                if (-BalanceMax < Balance && Balance < BalanceMax)
                {
                    animator.SetTrigger("RightLeg");
                    StartCoroutine("ReturnToPosition");

                }
                break;
            default:
                break;
        }
    }
}
