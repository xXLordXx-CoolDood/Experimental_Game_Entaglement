using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim;
    public float heightOffset = 0.5f;

    private PlayerInput playerInput;

    void Start()
    {
        playerInput = new PlayerInput();
    }

    #region

    public void FR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !FRLeg.isHeld) { CheckLegStatus(FRAnim, true); FRLeg.isHeld = true; FRLeg.SetTargetFollowState(true); }

        if (ctx.canceled && FRLeg.isHeld) { CheckLegStatus(FRAnim, false); FRLeg.isHeld = false; }
    }
    public void BR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !BRLeg.isHeld) { BRAnim.SetTrigger("Next_State"); BRLeg.isHeld = true; BRLeg.SetTargetFollowState(true); }

        if (ctx.canceled && BRLeg.isHeld) { BRLeg.isHeld = false; }
    }
    public void FL(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !FLLeg.isHeld) { FLAnim.SetTrigger("Next_State"); FLLeg.isHeld = true; FLLeg.SetTargetFollowState(true); }

        if (ctx.canceled && FLLeg.isHeld) { FLLeg.isHeld = false; }
    }
    public void BL(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !BLLeg.isHeld) { BLAnim.SetTrigger("Next_State"); BLLeg.isHeld = true; BLLeg.SetTargetFollowState(true); }

        if (ctx.canceled && BLLeg.isHeld) { BLLeg.isHeld = false; }
    }

    #endregion

    void Update()
    {
        UpdateBodyPosition();
    }

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.footBone.position.x + BRLeg.footBone.position.x + FLLeg.footBone.position.x + BLLeg.footBone.position.x) / 4;
        float averageY = (FRLeg.footBone.position.y + BRLeg.footBone.position.y + FLLeg.footBone.position.y + BLLeg.footBone.position.y) / 4;
        float averageZ = (FRLeg.footBone.position.z + BRLeg.footBone.position.z + FLLeg.footBone.position.z + BLLeg.footBone.position.z) / 4;

        transform.position = new Vector3(averageX, averageY + heightOffset, averageZ);
    }


    private void CheckLegStatus(Animator anim, bool held)
    {
        //If pressed and leg is idle, move leg up
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") && held) { anim.SetTrigger("Next_State"); }

        //If released and leg is idle in air, move leg down
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid") && !held) { anim.SetTrigger("Next_State"); }

        //If pressed and leg is falling, reverse anim to leg idle
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") && held) { anim.SetFloat("Speed_Multiplier", -1f); }

        //If released and leg is rising, reverse anim to leg idle
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && !held) { anim.SetFloat("Speed_Multiplier", -1f); }
    }
}
