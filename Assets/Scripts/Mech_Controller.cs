using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public GameObject bullet;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim;
    public Transform gun, shotSpawn;
    public float heightOffset = 0.5f, positionOffset = 1, rotationMultiplierX = 1, rotationMultiplierY = 0.5f, skidStrength = 10;
    public LayerMask groundLayer;

    private PlayerInput playerInput;
    private Vector2 prevPosition;
    private Vector3 skidDir;
    private int activeLegs = 0, direction, gunDirection;
    private float _skidMultiplier, tiltMultiplier, timer;
    private bool kneeling, stumbled, recovering;
    private Leg_Animator resistor1, resistor2;
    [SerializeField] private bool isSkidding = false;

    void Start()
    {
        playerInput = new PlayerInput();
    }

    #region //input

    public void FR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& FRLeg.isHeld*/) { CheckLegStatus(FRAnim, FRLeg, false); FRLeg.isHeld = false; activeLegs--; }

        if (ctx.performed /*&& !FRLeg.isHeld*/) { CheckLegStatus(FRAnim, FRLeg, true); FRLeg.isHeld = true; FRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BR(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& BRLeg.isHeld*/) { CheckLegStatus(BRAnim, BRLeg, false); BRLeg.isHeld = false; activeLegs--; }

        if (ctx.performed /*&& !BRLeg.isHeld*/) { CheckLegStatus(BRAnim, BRLeg, true); BRLeg.isHeld = true; BRLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void FL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& FLLeg.isHeld*/) { CheckLegStatus(FLAnim, FLLeg, false); FLLeg.isHeld = false; activeLegs--; }

        if (ctx.performed /*&& !FLLeg.isHeld*/) { CheckLegStatus(FLAnim, FLLeg, true); FLLeg.isHeld = true; FLLeg.SetTargetFollowState(true); activeLegs++; }
    }
    public void BL(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled /*&& BLLeg.isHeld*/) { CheckLegStatus(BLAnim, BLLeg, false); BLLeg.isHeld = false; activeLegs--; }

        if (ctx.performed /*&& !BLLeg.isHeld*/) { CheckLegStatus(BLAnim, BLLeg, true); BLLeg.isHeld = true; BLLeg.SetTargetFollowState(true); activeLegs++; }
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
        if (ctx.ReadValue<float>() != 0 && !stumbled) { gunDirection = Mathf.RoundToInt(ctx.ReadValue<float>()); }
        if(ctx.ReadValue<float>() != 0 && stumbled) { recovering = true; }
        if (ctx.canceled) { gunDirection = 0; }
    }

    public void Gun_Shoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isSkidding) { ShootGun(); }
    }

    #endregion 

    void Update()
    {

        gun.localEulerAngles = new Vector3(0, gun.localEulerAngles.y + (gunDirection * 60 * Time.deltaTime), 90);

        if(_skidMultiplier > 0) { 
            if(resistor1.isHeld || resistor2.isHeld) { stumbled = false; _skidMultiplier -= Time.deltaTime * skidStrength * 3; }
            _skidMultiplier -= Time.deltaTime * skidStrength / 2; 
        }
        if(_skidMultiplier < 0) {
            FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false;
            BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false;
            FRAnim.SetBool("Stumbling", false);
            FRAnim.ResetTrigger("Next_State");
            FLAnim.SetBool("Stumbling", false);
            FLAnim.ResetTrigger("Next_State");
            BRAnim.SetBool("Stumbling", false);
            BRAnim.ResetTrigger("Next_State");
            BLAnim.SetBool("Stumbling", false);
            BLAnim.ResetTrigger("Next_State");

            if (stumbled)
            {
                Splat();
                float averageX = (FRLeg.footBone.position.x + BRLeg.footBone.position.x + FLLeg.footBone.position.x + BLLeg.footBone.position.x) / 4;
                float averageY = (FRLeg.footBone.position.y + BRLeg.footBone.position.y + FLLeg.footBone.position.y + BLLeg.footBone.position.y) / 6;
                float averageZ = (FRLeg.footBone.position.z + BRLeg.footBone.position.z + FLLeg.footBone.position.z + BLLeg.footBone.position.z) / 4;
                skidDir = new Vector3(averageX, averageY, averageZ);
                timer = 0;
            }

            _skidMultiplier = 0;
            isSkidding = false; 
        }

        if(isSkidding) { transform.Translate(skidDir * Time.deltaTime * _skidMultiplier, Space.World); return; }

        if (recovering)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, 1);

            transform.position = Vector3.Lerp(prevPosition, skidDir, timer);
            transform.eulerAngles = new Vector3(0, 0, 90 - (timer * 90));

            if(timer == 1) { recovering = false; stumbled = false; prevPosition = transform.position; }
        }

        if (stumbled) { return; }

        CheckLegCombos();
        UpdateBodyPosition();
        UpdateBodyRotation();
        CheckForStumbling();
    }

    #region UpdateFunctions
    private void CheckLegCombos()
    {
        kneeling = false;

        #region sitLogic
        //If both back legs are active(rising with rise tag) instead force them to sit.
        if (BRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && BLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && BRAnim.GetBool("LegDown") && BLAnim.GetBool("LegDown"))
        {
            BRAnim.SetFloat("Speed_Multiplier", -1);
            BLAnim.SetFloat("Speed_Multiplier", -1);
            tiltMultiplier += Time.deltaTime * -15;
            kneeling = true;
        }
        else if (FRAnim.GetBool("LegDown") && FLAnim.GetBool("LegDown") && FRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && FLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise"))
        {
            FRAnim.SetFloat("Speed_Multiplier", -1);
            FLAnim.SetFloat("Speed_Multiplier", -1);
            tiltMultiplier += Time.deltaTime * 15;
            kneeling = true;
        }

        tiltMultiplier = Mathf.Clamp(tiltMultiplier, -15, 15);
        #endregion
    }

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.targetPoint.position.x + BRLeg.targetPoint.position.x + FLLeg.targetPoint.position.x + BLLeg.targetPoint.position.x) / 4;
        float averageY = (FRLeg.legHeight + BRLeg.legHeight + FLLeg.legHeight + BLLeg.legHeight) / 4;
        float averageZ = (FRLeg.targetPoint.position.z + BRLeg.targetPoint.position.z + FLLeg.targetPoint.position.z + BLLeg.targetPoint.position.z) / 4;

        transform.position = new Vector3(transform.position.x, averageY - heightOffset, averageZ);
    }

    private void UpdateBodyRotation()
    {
        float angleX = 0;
        if (!kneeling)
        {
            if(tiltMultiplier > 0) { tiltMultiplier = Mathf.Clamp(tiltMultiplier - (Time.deltaTime  * 15), 0, 15); angleX = tiltMultiplier; }
            if(tiltMultiplier < 0) { tiltMultiplier = Mathf.Clamp(tiltMultiplier + (Time.deltaTime * 15), -15, 0); angleX = tiltMultiplier; }
            if(tiltMultiplier == 0) { angleX = (BLLeg.targetPoint.position.y + BRLeg.targetPoint.position.y) - (FLLeg.targetPoint.position.y + FRLeg.targetPoint.position.y); }   
        }
        else
        {
            angleX = tiltMultiplier;
            BRLeg.CheckForGround();
            FRLeg.CheckForGround();
            BLLeg.CheckForGround();
            FLLeg.CheckForGround();
        }

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        float angleY = Vector2.Angle(prevPosition, currentPos);

        float angleZ = (BLLeg.targetPoint.position.y + FLLeg.targetPoint.position.y) - (BRLeg.targetPoint.position.y + FRLeg.targetPoint.position.y);

        //Apply rotations to the mech
        transform.eulerAngles = new Vector3(angleX * rotationMultiplierX, transform.eulerAngles.y + (angleY * rotationMultiplierY * (direction / 45)), 0/*angleZ * rotationMultiplierX*/);

        //Update pre position
        prevPosition = currentPos; 
    }

    private void CheckForStumbling()
    {

    }
    #endregion

    #region CallableFunctions
    private void CheckLegStatus(Animator anim, Leg_Animator script, bool held)
    {
        //If pressed and leg is idle, move leg up
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") && held) { 
            anim.SetFloat("Speed_Multiplier", 1f); anim.SetTrigger("Next_State"); anim.SetBool("LegDown", true); }

        //If released and leg is idle in air, move leg down
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid") && !held) { 
            anim.SetFloat("Speed_Multiplier", 1f); anim.SetTrigger("Next_State");anim.SetBool("LegDown", false); }

        //If pressed and leg is falling, reverse anim to leg idle
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") && held) { anim.SetFloat("Speed_Multiplier", -1f); anim.SetBool("LegDown", true); }

        //If released and leg is rising, reverse anim to leg idle
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && !held) { anim.SetFloat("Speed_Multiplier", -1f); anim.SetBool("LegDown", false); }
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
        stumbled = true;

        FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        FRAnim.SetBool("Stumbling", true);
        FRAnim.SetFloat("CycleOffset", Random.Range(0f, 1f));

        BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        BRAnim.SetBool("Stumbling", true);
        BRAnim.SetFloat("CycleOffset", Random.Range(0f, 1f));

        FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        FLAnim.SetBool("Stumbling", true);
        FLAnim.SetFloat("CycleOffset", Random.Range(0f, 1f));

        BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true;
        BLAnim.SetBool("Stumbling", true);
        BLAnim.SetFloat("CycleOffset", Random.Range(0f, 1f));

        angle += 90;

        if (angle < 30) { resistor1 = BLLeg; resistor2 = BRLeg;  }
        if (angle > 30 && angle < 150) { resistor1 = FLLeg; resistor2 = BLLeg; /*stumbleIndicatorR.SetActive(true);*/ }
        if (angle > 150 && angle < 240) { resistor1 = FLLeg; resistor2 = FRLeg; }
        if (angle > 240) { resistor1 = FRLeg; resistor2 = BRLeg; /*stumbleIndicatorL.SetActive(true);*/ }
    }

    private void Splat()
    {
        transform.eulerAngles = new Vector3(0, 0, 90);
        prevPosition = transform.position;
    }

    #endregion

}
