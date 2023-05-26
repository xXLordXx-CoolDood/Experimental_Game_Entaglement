using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechMovement : MonoBehaviour
{
    MechStates state = MechStates.Based;
    Animator animator;
    Vector2 m_rotation;
    float Balance;

    [Header("Movement Values")]
    [SerializeField] float turnSpeed;

    [Header("Inputs")]
    [SerializeField] InputAction TriggerRight;
    [SerializeField] InputAction TriggerLeft;
    [SerializeField] InputAction Turn;


    private void OnEnable()
    {
        TriggerLeft.Enable();
        TriggerRight.Enable();
        Turn.Enable();
    }

    private void OnDisable()
    {
        TriggerLeft.Disable();
        TriggerRight.Disable();
        Turn.Disable();
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
        if (state == MechStates.Based)
        {
            var turnValue = Turn.ReadValue<float>();
            m_rotation.y += turnValue * Time.deltaTime * turnSpeed;
            transform.eulerAngles = m_rotation;
        }
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
