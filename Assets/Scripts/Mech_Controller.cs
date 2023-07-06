using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mech_Controller : MonoBehaviour
{
    public Leg_Animator FRLeg, BRLeg, FLLeg, BLLeg;
    public GameObject bullet, splatMech;
    public Animator FRAnim, BRAnim, FLAnim, BLAnim;
    public Transform gun, gunYaw, shotSpawn, chest, waist;
    public float heightOffset = 0.5f, positionOffset = 1, rotationMultiplierX = 1, rotationMultiplierY = 0.5f, skidStrength = 10;
    public LayerMask groundLayer;
    public bool isAiming = true;

    [HideInInspector] public Vector2 prevPosition;

    private PlayerInput playerInput;
    private Vector3 skidDir;
    private int activeLegs = 0, gunDirectionX, gunDirectionY;
    private float _skidMultiplier, tiltMultiplier, timer, direction;
    private bool kneeling, stumbled;
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

    public void FR(InputAction.CallbackContext ctx)
    {
        if (isAiming)
        {
            if (ctx.performed) { gunDirectionX = 1; }
            if (ctx.canceled) { gunDirectionX = 0; }
            return;
        }

        if (ctx.canceled /*&& FRLeg.isHeld*/) { CheckLegStatus(FRAnim, FRLeg, false); FRLeg.isHeld = false; activeLegs--; }

        if (ctx.performed && !isSkidding) { CheckLegStatus(FRAnim, FRLeg, true); FRLeg.isHeld = true; FRLeg.SetTargetFollowState(true); activeLegs++; }
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

        if (ctx.canceled /*&& BRLeg.isHeld*/) { CheckLegStatus(BRAnim, BRLeg, false); BRLeg.isHeld = false; activeLegs--; }

        if (ctx.performed && !isSkidding) { CheckLegStatus(BRAnim, BRLeg, true); BRLeg.isHeld = true; BRLeg.SetTargetFollowState(true); activeLegs++; }
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

        if (ctx.canceled /*&& FLLeg.isHeld*/) { CheckLegStatus(FLAnim, FLLeg, false); FLLeg.isHeld = false; activeLegs--; }

        if (ctx.performed && !isSkidding) { CheckLegStatus(FLAnim, FLLeg, true); FLLeg.isHeld = true; FLLeg.SetTargetFollowState(true); activeLegs++; }
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
        if (ctx.canceled /*&& BLLeg.isHeld*/) { CheckLegStatus(BLAnim, BLLeg, false); BLLeg.isHeld = false; activeLegs--; }

        if (ctx.performed && !isSkidding) { CheckLegStatus(BLAnim, BLLeg, true); BLLeg.isHeld = true; BLLeg.SetTargetFollowState(true); activeLegs++; }
        else if (ctx.performed && isSkidding) { BLLeg.isHeld = true; activeLegs++; }
    }
    public void Reverse(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { ChangeGears(!FRAnim.GetBool("Forward")); }
    }
    public void Turn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { direction = ctx.ReadValue<float>() * -15; /*ChangeDirection(direction / 45);*/ }
        if (ctx.canceled) { direction = 0;/* ChangeDirection(0);*/ }
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
            if(mechGun.isReadyToShoot) {
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
        gun.localEulerAngles = new Vector3(0, gun.localEulerAngles.y + (gunDirectionY * 60 * Time.deltaTime), 90); //Gun Left/Right
        gunYaw.Rotate(new Vector3(-1, 0, 0), gunDirectionX * 30 * Time.deltaTime); //Gun Up/Down

        float rot = gunYaw.eulerAngles.x - 180;

        if(rot > -170 && rot < 0) { rot = -170; }
        if(rot < 100 && rot > 0) { rot = 100; }
        gunYaw.eulerAngles = new Vector3(rot + 180, gunYaw.eulerAngles.y, gunYaw.eulerAngles.z);
        

        if (_skidMultiplier > 0) { 
            if(resistor1.isHeld || resistor2.isHeld) { stumbled = false; _skidMultiplier -= Time.deltaTime * skidStrength * 3; }
            _skidMultiplier -= Time.deltaTime * skidStrength / 2;
        }
        if(_skidMultiplier < 0) {
            FRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false;
            BRLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            FLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false; 
            BLLeg.targetPoint.GetComponent<Target_Follow>().isSkidding = false;
            FRAnim.ResetTrigger("Next_State");
            FLAnim.ResetTrigger("Next_State");
            BRAnim.ResetTrigger("Next_State");
            BLAnim.ResetTrigger("Next_State");

            if (stumbled) { Splat(); }

            _skidMultiplier = 0;
            isSkidding = false; 
        }

        if(isSkidding) { transform.Translate(skidDir * Time.deltaTime * _skidMultiplier, Space.World); return; }

        if (stumbled) { return; }

        CheckLegCombos();
        UpdateBodyPosition();
        UpdateBodyRotation();

        if(direction != 0) { UpdateDirectionRotations(); }
 
        CheckForStumbling();
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
    }

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

    private void CheckForStumbling()
    {

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

    private void ShootGun()
    {
        GameObject _bullet = Instantiate(bullet, shotSpawn.position, shotSpawn.rotation);
        _bullet.GetComponent<Rigidbody>().AddForce(shotSpawn.forward * 10, ForceMode.Impulse);

        //Calculate shot backward angle
        float angle = gun.eulerAngles.y;

        _skidMultiplier = skidStrength;
        skidDir = new Vector3(-1 * Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle));
        isSkidding = true;
        stumbled = true;

        if (angle > 240 && angle < 300) { resistor1 = BLLeg; resistor2 = BRLeg; Debug.Log("Brace Back"); }
        if (angle > 60 && angle < 120) { resistor1 = FLLeg; resistor2 = FRLeg; Debug.Log("Brace Front"); }
        if (angle > 120 && angle < 240) { resistor1 = FRLeg; resistor2 = BRLeg; Debug.Log("Brace Right"); }
        if (angle > 300 || angle < 60) { resistor1 = FLLeg; resistor2 = BLLeg; Debug.Log("Brace Left"); }

        

    }

    private void Splat()
    {
        GameObject newMech = Instantiate(splatMech, transform.position, transform.rotation);

        List<Transform> children = new List<Transform>(0);

        for(int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        int c = 0;
        foreach(Transform trans in children)
        {
            newMech.transform.GetChild(c).position = trans.position;
            newMech.transform.GetChild(c).rotation = trans.rotation;
            c++;
        }

        transform.parent.GetComponent<Mech_Holder>().MechDie(newMech);
    }

    #endregion
}
