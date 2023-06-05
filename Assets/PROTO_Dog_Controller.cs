using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_Dog_Controller : MonoBehaviour
{
    public PROTO_Leg_Animator LF, LB, RF, RB;
    public float legSpeed = 1, maxLegHeight = 1;

    private PlayerInput playerInput;
    private bool LFHeld, LBHeld, RFHeld, RBHeld;
    [SerializeField] private bool opposites;

    void Start()
    {
        playerInput = new PlayerInput();
        LF.legSpeed = legSpeed; LF.maxLegHeight = maxLegHeight;
        LB.legSpeed = legSpeed; LB.maxLegHeight = maxLegHeight;
        RF.legSpeed = legSpeed; RF.maxLegHeight = maxLegHeight;
        RB.legSpeed = legSpeed; RB.maxLegHeight = maxLegHeight;
    }

    #region
    public void ForwardLeft(InputAction.CallbackContext ctx) 
    {
        if (ctx.performed) { LF.isHeld = true; }
        if (ctx.canceled) { LF.isHeld = false; }
    }
    public void BackwardLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { LB.isHeld = true; }
        if (ctx.canceled) { LB.isHeld = false; }
    }
    public void ForwardRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { RF.isHeld = true; }
        if (ctx.canceled) { RF.isHeld = false; }
    }
    public void BackwardRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { RB.isHeld = true; }
        if (ctx.canceled) { RB.isHeld = false; }
    }
    #endregion //Input Handling

    void Update()
    {
        opposites = false;

        //Check if opposite legs are active
        if(RB.isHeld && LF.isHeld && !RF.isHeld && !LB.isHeld) {
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
    }

    void MoveBody()
    {
        float averageZ = (LF.legTarget.position.z + LB.legTarget.position.z + RF.legTarget.position.z + RB.legTarget.position.z) / 4;

        transform.position = new Vector3(transform.position.x, transform.position.y, averageZ);
    }
}
