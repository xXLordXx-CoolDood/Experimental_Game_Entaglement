using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_Dog_Controller : MonoBehaviour
{
    public PROTO_Leg_Animator LF, LB, RF, RB;
    public Transform direction;
    public float legSpeed = 1, maxLegHeight = 1, heightOffset = 1, rotationMultiplier, maxRotation = 33f;

    [HideInInspector] public int engagedLegs;
    [HideInInspector] public bool opposites;

    private PlayerInput playerInput;
    private int directionChange;
    private Vector2 prevPosition;

    void Start()
    {
        //Initialize vars and legs
        playerInput = new PlayerInput();
        LF.legSpeed = legSpeed; LF.controllerRef = this;
        LB.legSpeed = legSpeed; LB.controllerRef = this;
        RF.legSpeed = legSpeed; RF.controllerRef = this;
        RB.legSpeed = legSpeed; RB.controllerRef = this;

        prevPosition = new Vector2(transform.position.x, transform.position.z);
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
    public void Reverse(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) 
        { 
            LF.forwardMultiplier *= -1; LB.forwardMultiplier *= -1; RF.forwardMultiplier *= -1; RB.forwardMultiplier *= -1;
        }
    }
    #endregion //Input Handling

    void Update()
    {
        opposites = false;

        Vector3 delta = new Vector3(0, directionChange, 0);
        direction.Rotate(delta, Time.deltaTime * 20);
        LF.Rotate(delta); LB.Rotate(delta); RF.Rotate(delta); RB.Rotate(delta);

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
        float averageX = (LF.legTarget.position.x + LB.legTarget.position.x + RF.legTarget.position.x + RB.legTarget.position.x) / 4;
        float averageY = (LF.legTarget.position.y + LB.legTarget.position.y + RF.legTarget.position.y + RB.legTarget.position.y) / 4;

        maxLegHeight = (averageY + heightOffset) * 0.9f;

        transform.position = new Vector3(averageX, averageY + heightOffset, averageZ);
    }

    void RotateBody()
    {
        float angleX = (LB.legTarget.position.y + RB.legTarget.position.y) - (LF.legTarget.position.y + RF.legTarget.position.y);

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        float angleY = Vector2.Angle(prevPosition, currentPos);
        Debug.Log(angleY);

        transform.eulerAngles = new Vector3(angleX * rotationMultiplier, transform.eulerAngles.y - angleY, 0);

        prevPosition = currentPos;
    }

    public void AddMoveRotation()
    {
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, direction.eulerAngles.y / 2 * Time.deltaTime, 0);
    }
}
