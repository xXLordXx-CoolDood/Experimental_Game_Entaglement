using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim;
    public Transform body, dirIndicator;
    public float heightOffset = 0.5f, rotationMultiplier = 1;
    public LayerMask groundLayer;

    private PlayerInput playerInput;
    private int activeLegs = 0;
    private int direction;

    void Start()
    {
        playerInput = new PlayerInput();
    }

    #region //input

    public void FR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && FRLeg.isHeld) { CheckLegStatus(FRAnim, false); FRLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !BRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed && !FRLeg.isHeld) { CheckLegStatus(FRAnim, true); FRLeg.isHeld = true; FRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && BRLeg.isHeld) { CheckLegStatus(BRAnim, false); BRLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !FRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed && !BRLeg.isHeld) { CheckLegStatus(BRAnim, true); BRLeg.isHeld = true; BRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void FL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && FLLeg.isHeld) { CheckLegStatus(FLAnim, false); FLLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !BLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed && !FLLeg.isHeld) { CheckLegStatus(FLAnim, true); FLLeg.isHeld = true; FLLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && BLLeg.isHeld) { CheckLegStatus(BLAnim, false); BLLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !FLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed && !BLLeg.isHeld) { CheckLegStatus(BLAnim, true); BLLeg.isHeld = true; BLLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void Reverse(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { ChangeGears(!FRAnim.GetBool("Forward")); }
    }
    public void Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { direction = Mathf.RoundToInt(ctx.ReadValue<float>()) * 45; ChangeDirection(direction / 45); }
        if (ctx.canceled) { direction = 0; ChangeDirection(0); }
    }

    #endregion 

    void Update()
    {
        UpdateBodyPosition();
        UpdateBodyRotation();
    }

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.footBone.position.x + BRLeg.footBone.position.x + FLLeg.footBone.position.x + BLLeg.footBone.position.x) / 4;
        float averageY = (FRLeg.footBone.position.y + BRLeg.footBone.position.y + FLLeg.footBone.position.y + BLLeg.footBone.position.y) / 6;
        float averageZ = (FRLeg.footBone.position.z + BRLeg.footBone.position.z + FLLeg.footBone.position.z + BLLeg.footBone.position.z) / 4;

        transform.position = new Vector3(averageX, averageY, averageZ);
    }

    private void UpdateBodyRotation() {
        float angleX = (BLLeg.footBone.position.y + BRLeg.footBone.position.y) - (FLLeg.footBone.position.y + FRLeg.footBone.position.y);

        dirIndicator.eulerAngles = new Vector3(0, direction, 0);
        transform.eulerAngles = new Vector3(angleX * rotationMultiplier, transform.eulerAngles.y, 0);
    }

    private void CheckLegStatus(Animator anim, bool held)
    {
        //If pressed and leg is idle, move leg up
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") && held) { 
            anim.SetFloat("Speed_Multiplier", 1f); anim.SetTrigger("Next_State"); }

        //If released and leg is idle in air, move leg down
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid") && !held) { 
            anim.SetFloat("Speed_Multiplier", 1f); anim.SetTrigger("Next_State"); }

        //If pressed and leg is falling, reverse anim to leg idle
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") && held) { anim.SetFloat("Speed_Multiplier", -1f); }

        //If released and leg is rising, reverse anim to leg idle
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && !held) { anim.SetFloat("Speed_Multiplier", -1f); }
    }

    private void ChangeGears(bool newState) {
        FRAnim.SetBool("Forward", newState);
        BRAnim.SetBool("Forward", newState);
        FLAnim.SetBool("Forward", newState);
        BLAnim.SetBool("Forward", newState);
    }

    private void ChangeDirection(int newDirection) {
        FRAnim.SetInteger("Turn", newDirection);
        BRAnim.SetInteger("Turn", newDirection);
        FLAnim.SetInteger("Turn", newDirection);
        BLAnim.SetInteger("Turn", newDirection);
    }
}
