using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechMovement : MonoBehaviour
{
    Animator animator;
    MechStates state = MechStates.Based;
    [SerializeField] InputAction TriggerRight;
    [SerializeField] InputAction TriggerLeft;

    private void OnEnable()
    {
        TriggerLeft.Enable();
        TriggerRight.Enable();
    }

    private void OnDisable()
    {
        TriggerLeft.Disable();
        TriggerRight.Disable();
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
        Left();
    }

    private void TriggerRight_performed(InputAction.CallbackContext ctx)
    {
        Right();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Left()
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
        }
    }

    void Right()
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
        }
    }

}
