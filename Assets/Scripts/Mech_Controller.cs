using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using FMODUnity;
using UnityEngine.VFX;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public GameObject bullet, splatMech;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim, gunAnim, camAnim;
    public Transform gun, gunYaw, shotSpawn, chest, waist, heightLines, frontCheck, backCheck, gunRotIndicator, turnIndicator, camBehind;
    public float heightOffset = 0.5f, positionOffset = 1, rotationMultiplierX = 1, rotationMultiplierY = 0.5f, skidStrength = 10, skidDecay = 1, _skidMultiplier;
    public LayerMask groundLayer;
    public bool isAiming = true;
    [SerializeField] private EventReference shootEvent, explodeEvent;
    [HideInInspector] public Vector2 prevPosition;
    public float idleTimer = 0;
    [SerializeField] private VisualEffect gunLaser;

    private PlayerInput playerInput;
    private Vector3 skidDir;
    private int activeLegs = 0, icyLegs = 0, gunDirectionX, gunDirectionY, prevDirX, prevDirY;
    private float _skidDecay, tiltMultiplier, direction, moveDirection = 1, gunAccelX, gunAccelY;
    private bool kneeling, stumbled, blocked;
    private Leg_Animator resistor1, resistor2;
    [SerializeField] private bool isSkidding = false;

    MechGun mechGun;

    void Start()
    {
        playerInput = new PlayerInput();
        mechGun = GetComponent<MechGun>();
    }

    #region //input

    #region Gun

    public void Gun1(InputAction.CallbackContext ctx)
    {
        mechGun.gun1 = ctx.ReadValue<float>();
    }

    public void Gun2(InputAction.CallbackContext ctx)
    {
        mechGun.gun2 = ctx.ReadValue<float>(); 
    }

    public void Gun3(InputAction.CallbackContext ctx)
    {
        mechGun.gun3 = ctx.ReadValue<float>();
    }

    #endregion

    public void Switch_Camera(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            GetComponent<CameraSwitcher>().CycleCamera();
        }
    }

    public void FR(InputAction.CallbackContext ctx)
    {
        if (isAiming)
        {
            if (ctx.performed) { gunDirectionX = 1; }
            if (ctx.canceled) { gunDirectionX = 0; }
            return;
        }

        if (ctx.canceled /*&& FRLeg.isHeld*/) { CheckLegStatus(FRAnim, FRLeg, false); activeLegs--; }

        if (ctx.performed && !isSkidding && activeLegs < 2 && !FLLeg.isHeld && !BRLeg.isHeld) { CheckLegStatus(FRAnim, FRLeg, true); FRLeg.SetTargetFollowState(true); activeLegs++; }
        else if(ctx.performed && isSkidding) { FRLeg.isHeld = true; activeLegs++; }
    }
    public void BR(InputAction.CallbackContext ctx)
    {
        if (isAiming) 
        {
            if (ctx.performed) { gunDirectionX = -1; }
            if (ctx.canceled) { gunDirectionX = 0; }
            return;
        }

        if (ctx.canceled /*&& BRLeg.isHeld*/) { CheckLegStatus(BRAnim, BRLeg, false); activeLegs--; }

        if (ctx.performed && !isSkidding && activeLegs < 2 && !BLLeg.isHeld && !FRLeg.isHeld) { CheckLegStatus(BRAnim, BRLeg, true); BRLeg.SetTargetFollowState(true); activeLegs++; }
        else if (ctx.performed && isSkidding) { BRLeg.isHeld = true; activeLegs++; }
    }
    public void FL(InputAction.CallbackContext ctx)
    {
        if (isAiming)
        {
            if (ctx.performed) { gunDirectionY = 1; }
            if (ctx.canceled) { gunDirectionY = 0; }
            return;
        }

        if (ctx.canceled /*&& FLLeg.isHeld*/) { CheckLegStatus(FLAnim, FLLeg, false); activeLegs--; }

        if (ctx.performed && !isSkidding && activeLegs < 2 && !FRLeg.isHeld && !BLLeg.isHeld) { CheckLegStatus(FLAnim, FLLeg, true); FLLeg.SetTargetFollowState(true); activeLegs++; }
        else if (ctx.performed && isSkidding) { FLLeg.isHeld = true; activeLegs++; }
    }
    public void BL(InputAction.CallbackContext ctx)
    {
        if (isAiming)
        {
            if (ctx.performed) { gunDirectionY = -1; }
            if (ctx.canceled) { gunDirectionY = 0; }
            return;
        }
        if (ctx.canceled /*&& BLLeg.isHeld*/) { CheckLegStatus(BLAnim, BLLeg, false); activeLegs--; }

        if (ctx.performed && !isSkidding && activeLegs < 2 && !BRLeg.isHeld && !FLLeg.isHeld) { CheckLegStatus(BLAnim, BLLeg, true); BLLeg.SetTargetFollowState(true); activeLegs++; }
        else if (ctx.performed && isSkidding) { BLLeg.isHeld = true; activeLegs++; }
    }
    public void Direction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { moveDirection = ctx.ReadValue<float>(); }

        if (moveDirection > 0)
        {
            ChangeGears(true);   
        }

        if (moveDirection < 0)
        {
            ChangeGears(false);
        }
    }
    public void Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { direction = ctx.ReadValue<float>() * -15; /*ChangeDirection(direction / 45);*/ }
        if (ctx.canceled) { direction = 0; }

        if (ctx.canceled && turnIndicator.localPosition.x < 7 && turnIndicator.localPosition.x > -7f)
        { ResetDirectionAndRotations(); }
    }
    
    public void Gun_Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() != 0 && !stumbled) { gunDirectionY = Mathf.RoundToInt(ctx.ReadValue<float>()); }
        if (ctx.canceled) { gunDirectionY = 0; }
    }

    public void Gun_Tilt(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() != 0 && !stumbled) { gunDirectionX = Mathf.RoundToInt(ctx.ReadValue<float>()); }
        if (ctx.canceled) { gunDirectionX = 0; }
    }

    public void Gun_Shoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isSkidding) {
            ShootGun();
            mechGun.GenerateSequence();
            if (mechGun.isReadyToShoot && GetComponent<CameraSwitcher>().cameraList[1].enabled) {
                ShootGun(); 
                //Reset sequence in MechGun
                mechGun.GenerateSequence();
                mechGun.isReadyToShoot = false;
            }
        }
    }

    

    #endregion 

    void Update()
    {
        if (FRLeg.targetPoint.GetComponent<Target_Follow>().reseting) { return; }

        #region //Blocked logic & gun rotation
        RaycastHit hit;
        if (Physics.Raycast(frontCheck.position, frontCheck.forward, out hit, 2, groundLayer) ||
            Physics.Raycast(backCheck.position, backCheck.forward, out hit, 2, groundLayer)) 
        { blocked = true; Debug.Log("Blocked"); }
        else { blocked = false; }

        if (gunDirectionX != 0) { gunAccelX = Mathf.Clamp(gunAccelX + (Mathf.Abs(gunDirectionX) * 30 * Time.deltaTime), 0, 15); prevDirX = gunDirectionX; }
        else { gunAccelX = Mathf.Clamp(gunAccelX - (15 * Time.deltaTime), 0, 15); }
        if (gunDirectionY != 0) { gunAccelY = Mathf.Clamp(gunAccelY + (Mathf.Abs(gunDirectionY) * 60 * Time.deltaTime), 0, 30); prevDirY = gunDirectionY; }
        else { gunAccelY = Mathf.Clamp(gunAccelY - (30 * Time.deltaTime), 0, 30); }

        float accelX = (gunAccelX * prevDirX)  + (gunDirectionX * 15);
        float accelY = (gunAccelY * prevDirY) + (gunDirectionY * 30);

        gun.localEulerAngles = new Vector3(0, gun.localEulerAngles.y + accelY * Time.deltaTime, 90); //Gun Left/Right
        gunYaw.Rotate(new Vector3(-1, 0, 0), accelX * Time.deltaTime); //Gun Up/Down

        float rot = gunYaw.eulerAngles.x - 180;

        if(rot * 1.25f > 0) { heightLines.localPosition = new Vector3(0, (rot * 1.5f) - 270, 0); }
        else { heightLines.localPosition = new Vector3(0, (rot * 1.5f) + 270, 0); }

        if (rot > -170 && rot < 0) { rot = -170; }
        if(rot < 100 && rot > 0) { rot = 100; }
        gunYaw.eulerAngles = new Vector3(rot + 180, gunYaw.eulerAngles.y, gunYaw.eulerAngles.z);

        gunRotIndicator.Rotate(new Vector3(0, 0, 1), gunDirectionY * 60 * Time.deltaTime);
        #endregion

        if (_skidMultiplier > 0) { 
            if(resistor1.isHeld || resistor2.isHeld) { stumbled = false; _skidMultiplier -= Time.deltaTime * _skidDecay * 3; }
            _skidMultiplier -= Time.deltaTime * _skidDecay;
        }
        if(_skidMultiplier < 0) {
            FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; FRLeg.isSkidding = false;
            BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; BRLeg.isSkidding = false;
            FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; FLLeg.isSkidding = false;
            BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; BLLeg.isSkidding = false;
            FRAnim.ResetTrigger("Next_State");
            FLAnim.ResetTrigger("Next_State");
            BRAnim.ResetTrigger("Next_State");
            BLAnim.ResetTrigger("Next_State");

            if (stumbled && icyLegs < 3) { Splat(); }

            _skidMultiplier = 0;
            isSkidding = false; 
        }

        UpdateBodyRotation();

        if (blocked) { return; }
        if (isSkidding) { UpdateBodyHeight(); transform.Translate(skidDir * Time.deltaTime * (_skidMultiplier * _skidDecay), Space.World); return; }

        UpdateBodyPosition();

        if (stumbled) { return; }

        //CheckLegCombos();

        if(direction != 0) { UpdateDirectionRotations(); }
     }

    #region UpdateFunctions
    private void UpdateDirectionRotations()
    {
        chest.localEulerAngles = new Vector3(180, Mathf.Clamp(chest.localEulerAngles.y + (direction * Time.deltaTime) - 180, 75, 105), 0);
        waist.localEulerAngles = new Vector3(180, Mathf.Clamp(waist.localEulerAngles.y + (direction * Time.deltaTime) - 180, 75, 105), 0);

        BLAnim.transform.localEulerAngles = new Vector3(waist.localEulerAngles.x, -waist.localEulerAngles.y - 90, 0);
        BRAnim.transform.localEulerAngles = new Vector3(waist.localEulerAngles.x, -waist.localEulerAngles.y - 90, 0);
        FLAnim.transform.localEulerAngles = new Vector3(chest.localEulerAngles.x, -chest.localEulerAngles.y - 90, 0);
        FRAnim.transform.localEulerAngles = new Vector3(chest.localEulerAngles.x, -chest.localEulerAngles.y - 90, 0);

        camBehind.localEulerAngles = new Vector3(chest.localEulerAngles.x, -chest.localEulerAngles.y - 90, 0);
        turnIndicator.localPosition = new Vector3((chest.localEulerAngles.y - 270) * -3.33f, 0, 0);

        if(turnIndicator.localPosition.x > 0.2f) { camAnim.SetBool("Left", false); }
        if(turnIndicator.localPosition.x < -0.2f) { camAnim.SetBool("Left", true); }

        idleTimer = 0;
    }

    private void ResetDirectionAndRotations()
    {
        chest.localEulerAngles = new Vector3(180, 90, 0);
        waist.localEulerAngles = new Vector3(180, 90, 0);

        BLAnim.transform.localEulerAngles = new Vector3(waist.localEulerAngles.x, -waist.localEulerAngles.y - 90, 0);
        BRAnim.transform.localEulerAngles = new Vector3(waist.localEulerAngles.x, -waist.localEulerAngles.y - 90, 0);
        FLAnim.transform.localEulerAngles = new Vector3(chest.localEulerAngles.x, -chest.localEulerAngles.y - 90, 0);
        FRAnim.transform.localEulerAngles = new Vector3(chest.localEulerAngles.x, -chest.localEulerAngles.y - 90, 0);

        camBehind.localEulerAngles = new Vector3(chest.localEulerAngles.x, 0, 0);
        turnIndicator.localPosition = Vector3.zero;
        idleTimer = 0;
    }

    private void UpdateBodyHeight()
    {
        float averageY = (FRLeg.legHeight + BRLeg.legHeight + FLLeg.legHeight + BLLeg.legHeight) / 4;
        transform.position = new Vector3(transform.position.x, averageY - heightOffset, transform.position.z);
    }

    //private void CheckLegCombos()
    //{
    //    kneeling = false;

    //    #region sitLogic
    //    //If both back legs are active(rising with rise tag) instead force them to sit.
    //    if (BRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && BLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && BRAnim.GetBool("LegDown") && BLAnim.GetBool("LegDown"))
    //    {
    //        BRAnim.SetFloat("Speed_Multiplier", -1);
    //        BLAnim.SetFloat("Speed_Multiplier", -1);
    //        tiltMultiplier += Time.deltaTime * -15;
    //        kneeling = true;
    //    }
    //    else if (FRAnim.GetBool("LegDown") && FLAnim.GetBool("LegDown") && FRAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && FLAnim.GetCurrentAnimatorStateInfo(0).IsTag("Rise"))
    //    {
    //        FRAnim.SetFloat("Speed_Multiplier", -1);
    //        FLAnim.SetFloat("Speed_Multiplier", -1);
    //        tiltMultiplier += Time.deltaTime * 15;
    //        kneeling = true;
    //    }

    //    tiltMultiplier = Mathf.Clamp(tiltMultiplier, -15, 15);
    //    #endregion
    //}

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.targetPoint.position.x + BRLeg.targetPoint.position.x + FLLeg.targetPoint.position.x + BLLeg.targetPoint.position.x) / 4;
        float averageY = (FRLeg.legHeight + BRLeg.legHeight + FLLeg.legHeight + BLLeg.legHeight) / 4;
        float averageZ = (FRLeg.targetPoint.position.z + BRLeg.targetPoint.position.z + FLLeg.targetPoint.position.z + BLLeg.targetPoint.position.z) / 4;

        transform.position = new Vector3(averageX, averageY - heightOffset, averageZ);
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
        float angleY = (waist.localEulerAngles.y - 270) * Vector2.Distance(currentPos, prevPosition);

        float angleZ = (BLLeg.targetPoint.position.y + FLLeg.targetPoint.position.y) - (BRLeg.targetPoint.position.y + FRLeg.targetPoint.position.y);

        //Apply rotations to the mech
        transform.eulerAngles = new Vector3(angleX * rotationMultiplierX, transform.eulerAngles.y - (angleY * rotationMultiplierY), 0/*angleZ * rotationMultiplierX*/);

        //Fix feet rotation
        FLLeg.targetPoint.eulerAngles = new Vector3(0, transform.eulerAngles.y - (waist.localEulerAngles.y + 90), 0);
        FRLeg.targetPoint.eulerAngles = new Vector3(0, transform.eulerAngles.y - (waist.localEulerAngles.y + 90), 0);
        BLLeg.targetPoint.eulerAngles = new Vector3(0, transform.eulerAngles.y - (waist.localEulerAngles.y + 90), 0);
        BRLeg.targetPoint.eulerAngles = new Vector3(0, transform.eulerAngles.y - (waist.localEulerAngles.y + 90), 0);

        //Update pre position
        prevPosition = currentPos; 
    }
    #endregion

    #region CallableFunctions
    public void Respawn()
    {
        stumbled = false;
        
        prevPosition = transform.position;
    }

    private void CheckLegStatus(Animator anim, Leg_Animator script, bool held)
    {
        if(moveDirection == 0) { return; }

        script.isHeld = held;

        if (blocked && anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid")) { anim.SetFloat("Speed_Multiplier", -2f); anim.SetTrigger("Next_State"); script.LegActiveStatus(false); return; }

        //If pressed and leg is idle, move leg up
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") && held && EnoughGroundedLegs()) {
            anim.SetFloat("Speed_Multiplier", 2f); anim.SetTrigger("Next_State"); anim.SetBool("LegDown", true); script.LegActiveStatus(true);
        }

        //If released and leg is idle in air, move leg down
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid") && !held) { 
            anim.SetFloat("Speed_Multiplier", 2f); anim.SetTrigger("Next_State");anim.SetBool("LegDown", false); script.LegActiveStatus(false);
        }

        //If pressed and leg is falling, reverse anim to leg idle
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") && held) { anim.SetFloat("Speed_Multiplier", -2f); anim.SetBool("LegDown", true); script.LegActiveStatus(true); }
        
        //If released and leg is rising, reverse anim to leg idle
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") && !held) { anim.SetFloat("Speed_Multiplier", -2f); anim.SetBool("LegDown", false); script.LegActiveStatus(false); }
    }

    private bool EnoughGroundedLegs()
    {
        int groundedLegs = 0;
        if (FRLeg.grounded) { groundedLegs++; }
        if (FLLeg.grounded) { groundedLegs++; }
        if (BRLeg.grounded) { groundedLegs++; }
        if (BLLeg.grounded) { groundedLegs++; }

        if(groundedLegs > 2) { return true; }
        return false;
    }

    private void ChangeGears(bool newState) {
        FRAnim.SetBool("Forward", newState);
        BRAnim.SetBool("Forward", newState);
        FLAnim.SetBool("Forward", newState);
        BLAnim.SetBool("Forward", newState);
    }

    private void ShootGun()
    {
        gunAnim.SetTrigger("Shoot");
        GetComponent<CameraSwitcher>().CycleCamera();
        isAiming = false;
        isSkidding = true;
        stumbled = true;
        Debug.Log($"Stumbled = {stumbled}");
        Audio_Manager.instance.PlayOneShot(shootEvent, transform.position);
        gunLaser.SendEvent("MechShot");

        //Calculate shot backward angle
        float angle = gun.eulerAngles.y;

        skidDir = new Vector3(-1 * Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle));

        StartCoroutine(ShotDelay());

        if (angle > 240 && angle < 300) { resistor1 = BLLeg; resistor2 = BRLeg; }
        if (angle > 60 && angle < 120) { resistor1 = FLLeg; resistor2 = FRLeg; }
        if (angle > 120 && angle < 240) { resistor1 = FRLeg; resistor2 = BRLeg; }
        if (angle > 300 || angle < 60) { resistor1 = FLLeg; resistor2 = BLLeg; }

        RaycastHit hit;
        if(Physics.Raycast(shotSpawn.position, shotSpawn.forward, out hit, Mathf.Infinity) && hit.collider.GetComponent<Score>() && hit.collider.tag != "Points")
        {
            GetComponent<Point_Getter>().GetPoints(hit.collider.GetComponent<Score>().value, hit.collider.gameObject);
            Destroy(hit.collider.gameObject);
        }


    }

    private void Splat()
    {
        GameObject newMech = Instantiate(splatMech, transform.position + new Vector3(0, 03f, 0), transform.rotation);
        Audio_Manager.instance.PlayOneShot(explodeEvent, transform.position);

        List<Transform> children = new List<Transform>(0);

        for(int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        int c = 0;
        foreach(Transform trans in children)
        {
            newMech.transform.GetChild(c).localPosition = trans.localPosition;
            newMech.transform.GetChild(c).localPosition = trans.localPosition;
            c++;
        }

        transform.parent.GetComponent<Mech_Holder>().MechDie(newMech);
    }

    public void IcyLegUpdate(bool icy)
    {
        if (icy) { icyLegs = Mathf.Clamp(icyLegs + 1, 0, 4); return; }
        icyLegs = Mathf.Clamp(icyLegs - 1, 0, 4);
    }

    public void CheckLegIdleStatus()
    {
        //If all legs are grounded or idling, set all legs to idle state
        if(FRLeg.grounded && FLLeg.grounded && BLLeg.grounded && BRLeg.grounded && !FRLeg.isHeld && !FLLeg.isHeld && !BLLeg.isHeld && !BRLeg.isHeld &&
            !FRLeg.targetPoint.GetComponent<Target_Follow>().reseting)
        {
            IdleAllLegs();
            idleTimer += Time.deltaTime / 4;

            if(idleTimer > 5)
            {
                FRLeg.targetPoint.GetComponent<Target_Follow>().ResetLeg();
                BRLeg.targetPoint.GetComponent<Target_Follow>().ResetLeg();
                FLLeg.targetPoint.GetComponent<Target_Follow>().ResetLeg();
                BLLeg.targetPoint.GetComponent<Target_Follow>().ResetLeg();
                idleTimer = 0;
            }
        }
    }

    private void IdleAllLegs()
    {
        FRAnim.SetTrigger("Idle"); FRAnim.SetFloat("Speed_Multiplier", 1); FRAnim.ResetTrigger("Next_State");
        FLAnim.SetTrigger("Idle"); FLAnim.SetFloat("Speed_Multiplier", 1); FLAnim.ResetTrigger("Next_State");
        BRAnim.SetTrigger("Idle"); BRAnim.SetFloat("Speed_Multiplier", 1); BRAnim.ResetTrigger("Next_State");
        BLAnim.SetTrigger("Idle"); BLAnim.SetFloat("Speed_Multiplier", 1); BLAnim.ResetTrigger("Next_State");
    }

    IEnumerator ShotDelay()
    {
        yield return new WaitForSeconds(0.5f);
        FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true; FRLeg.isSkidding = true;
        BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true; BRLeg.isSkidding = true;
        FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true; FLLeg.isSkidding = true;
        BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = true; BLLeg.isSkidding = true;
        if (icyLegs > 2) { _skidMultiplier = skidStrength * 2; _skidDecay = skidDecay * 2; }
        else { _skidMultiplier = skidStrength; _skidDecay = skidDecay; }
    }
    #endregion
}
