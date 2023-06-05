using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_Dog_Controller : MonoBehaviour
{
    public PROTO_Leg_Animator LF, LB, RF, RB;
    public Transform direction;
    public float legSpeed = 1, maxLegHeight = 1, heightOffset = 1, rotationMultiplier;

    [HideInInspector] public int engagedLegs;
    [HideInInspector] public bool opposites;

    private PlayerInput playerInput;
    private int directionChange;
    private bool LFHeld, LBHeld, RFHeld, RBHeld;

    void Start()
    {
        //Initialize vars and legs
        playerInput = new PlayerInput();
        LF.legSpeed = legSpeed; LF.controllerRef = this;
        LB.legSpeed = legSpeed; LB.controllerRef = this;
        RF.legSpeed = legSpeed; RF.controllerRef = this;
        RB.legSpeed = legSpeed; RB.controllerRef = this;
    }

    #region
    public void ForwardLeft(InputAction.CallbackContext ctx) 
    {
        if (ctx.performed) { LF.isHeld = true; engagedLegs++; }
        if (ctx.canceled) { LF.isHeld = false; engagedLegs--; }
    }
    public void BackwardLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { LB.isHeld = true; engagedLegs++; }
        if (ctx.canceled) { LB.isHeld = false; engagedLegs--; }
    }
    public void ForwardRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { RF.isHeld = true; engagedLegs++; }
        if (ctx.canceled) { RF.isHeld = false; engagedLegs--; }
    }
    public void BackwardRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { RB.isHeld = true; engagedLegs++; }
        if (ctx.canceled) { RB.isHeld = false; engagedLegs--; }
    }
    public void Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { directionChange = Mathf.RoundToInt(ctx.ReadValue<float>()); }
        if (ctx.canceled) { directionChange = 0; }
    }
    #endregion //Input Handling

    void Update()
    {
        opposites = false;

        Vector3 delta = new Vector3(0, directionChange, 0);
        direction.Rotate(delta, Time.deltaTime * 20);

        //Check if opposite legs are active
        if (RB.isHeld && LF.isHeld && !RF.isHeld && !LB.isHeld) {
            opposites = true;
            RB.canMove = true;
            LF.canMove = true;
        }

        if ((!RB.isHeld && !LF.isHeld && RF.isHeld && LB.isHeld)) {
            opposites = true;
            RF.canMove = true;
            LB.canMove = true;
        }

        MoveBody();
        RotateBody();
    }

    void MoveBody()
    {
        float averageZ = (LF.legTarget.position.z + LB.legTarget.position.z + RF.legTarget.position.z + RB.legTarget.position.z) / 4;
        float averageY = (LF.legTarget.position.y + LB.legTarget.position.y + RF.legTarget.position.y + RB.legTarget.position.y) / 4;
        maxLegHeight = (averageY + heightOffset) * 0.9f;
        transform.position = new Vector3(transform.position.x, averageY + heightOffset, averageZ);
    }

    void RotateBody()
    {
        float angle = (LB.legTarget.position.y + RB.legTarget.position.y) - (LF.legTarget.position.y + RF.legTarget.position.y);
        transform.eulerAngles = new Vector3(angle * rotationMultiplier, 0, 0);
    }
}
