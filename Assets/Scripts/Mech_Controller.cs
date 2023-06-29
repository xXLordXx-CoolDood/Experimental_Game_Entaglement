using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public GameObject bullet;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim, MechAnim;
    public Transform body, dirIndicator, gun, shotSpawn;
    public float heightOffset = 0.5f, rotationMultiplierX = 1, rotationMultiplierY = 0.5f, skidStrength = 10;
    public LayerMask groundLayer;

    private PlayerInput playerInput;
    private Vector2 prevPosition;
    private Vector3 skidDir;
    private int activeLegs = 0, direction, gunDirection;
    private float _skidMultiplier;
    private Leg_Animator resistor1, resistor2;
    [SerializeField] private bool isSkidding = false;

    void Start()
    {
        playerInput = new PlayerInput();
    }

    #region //input

    public void FR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& FRLeg.isHeld*/) { CheckLegStatus(FRAnim, false); FRLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !BRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed /*&& !FRLeg.isHeld*/) { CheckLegStatus(FRAnim, true); FRLeg.isHeld = true; FRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& BRLeg.isHeld*/) { CheckLegStatus(BRAnim, false); BRLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !FRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed /*&& !BRLeg.isHeld*/) { CheckLegStatus(BRAnim, true); BRLeg.isHeld = true; BRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void FL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& FLLeg.isHeld*/) { CheckLegStatus(FLAnim, false); FLLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !BLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed /*&& !FLLeg.isHeld*/) { CheckLegStatus(FLAnim, true); FLLeg.isHeld = true; FLLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& BLLeg.isHeld*/) { CheckLegStatus(BLAnim, false); BLLeg.isHeld = false; activeLegs--; }

        //If more than 2 legs are engaged or the other right leg is engaged, stop any input
        if (activeLegs > 1 || !FLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) { return; }

        if (ctx.performed /*&& !BLLeg.isHeld*/) { CheckLegStatus(BLAnim, true); BLLeg.isHeld = true; BLLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void Reverse(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { ChangeGears(!FRAnim.GetBool("Forward")); }
    }
    public void Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { direction = Mathf.RoundToInt(ctx.ReadValue<float>()) * -45; ChangeDirection(direction / 45); }
        if (ctx.canceled) { direction = 0; ChangeDirection(0); }
    }
    
    public void Gun_Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() != 0) { gunDirection = Mathf.RoundToInt(ctx.ReadValue<float>()); }
        if (ctx.canceled) { gunDirection = 0; }
    }

    public void Gun_Shoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isSkidding) { ShootGun(); }
    }

    #endregion 

    void Update()
    {
        gun.localEulerAngles = new Vector3(0, gun.localEulerAngles.y + (gunDirection * 60 * Time.deltaTime), 0);

        if(_skidMultiplier > 0) { 
            if(resistor1.isHeld && resistor2.isHeld) { _skidMultiplier -= Time.deltaTime * skidStrength; }
            _skidMultiplier -= Time.deltaTime * skidStrength; 
        }
        if(_skidMultiplier < 0) {
            FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false;
            BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            _skidMultiplier = 0;
            isSkidding = false; 
        }

        if(isSkidding) {
            transform.Translate(skidDir * Time.deltaTime * _skidMultiplier, Space.World);
            return; 
        }

        CheckLegCombos();
        UpdateBodyPosition();
        UpdateBodyRotation();
    }

    #region UpdateFunctions
    private void CheckLegCombos()
    {
        #region sitLogic
        //If both back legs are active(rising with rise tag) instead force them to sit.
        if (BRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && BLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise"))
        {
            MechAnim.SetBool("Sit", true);
            MechAnim.SetFloat("Speed", 1);
        }

        if (MechAnim.GetCurrentAnimatorStateInfo(0).IsTag("Sit") && (
            !BRLeg.isHeld || !BLLeg.isHeld))
        {
            MechAnim.SetBool("Sit", false);
            MechAnim.SetFloat("Speed", -1);
        }

        if (MechAnim.GetBool("Sit"))
        {
            BRLeg.CheckForGround();
            BLLeg.CheckForGround();
        }
        #endregion

        #region kneelLogic
        if (FRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && FLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise"))
        {
            MechAnim.SetBool("Kneel", true);
            MechAnim.SetFloat("Speed", 1);
        }

        if (MechAnim.GetCurrentAnimatorStateInfo(0).IsTag("Kneel") && (
            !FRLeg.isHeld || !FLLeg.isHeld))
        {
            MechAnim.SetBool("Kneel", false);
            MechAnim.SetFloat("Speed", -1);
        }

        if (MechAnim.GetBool("Kneel"))
        {
            FRLeg.CheckForGround();
            FLLeg.CheckForGround();
        }
        #endregion
    }

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.footBone.position.x + BRLeg.footBone.position.x + FLLeg.footBone.position.x + BLLeg.footBone.position.x) / 4;
        float averageY = (FRLeg.footBone.position.y + BRLeg.footBone.position.y + FLLeg.footBone.position.y + BLLeg.footBone.position.y) / 6;
        float averageZ = (FRLeg.footBone.position.z + BRLeg.footBone.position.z + FLLeg.footBone.position.z + BLLeg.footBone.position.z) / 4;

        transform.position = new Vector3(averageX, averageY, averageZ);
    }

    private void UpdateBodyRotation()
    {
        float angleX = (BLLeg.footBone.position.y + BRLeg.footBone.position.y) - (FLLeg.footBone.position.y + FRLeg.footBone.position.y);

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        float angleY = Vector2.Angle(prevPosition, currentPos);

        dirIndicator.localEulerAngles = new Vector3(0, direction / 2, 0);
        transform.eulerAngles = new Vector3(angleX * rotationMultiplierX, transform.eulerAngles.y + (angleY * rotationMultiplierY * (direction / 45)), 0);

        prevPosition = currentPos;
    }
    #endregion

    #region CallableFunctions
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

    private void ShootGun()
    {
        GameObject _bullet = Instantiate(bullet, shotSpawn.position, shotSpawn.rotation);
        _bullet.GetComponent<Rigidbody>().AddForce(shotSpawn.forward * 10, ForceMode.Impulse);

        //Calculate shot backward angle
        float angle = gun.eulerAngles.y - 90;

        _skidMultiplier = skidStrength;
        skidDir = new Vector3(-1 * Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle));
        isSkidding = true;

        FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;

        angle += 90;

        if (angle < 30) { resistor1 = BLLeg; resistor2 = BRLeg; Debug.Log("Brace back!"); }
        if (angle > 30 && angle < 150) { resistor1 = FLLeg; resistor2 = BLLeg; Debug.Log("Brace left!"); }
        if (angle > 150 && angle < 240) { resistor1 = FLLeg; resistor2 = FRLeg; Debug.Log("Brace front!"); }
        if (angle > 240) { resistor1 = FRLeg; resistor2 = BRLeg; Debug.Log("Brace right!"); }
    }

    #endregion

}
