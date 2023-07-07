using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, groundSnap, footBone, ankleBone, kneeBone, hipBone;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;

    public bool isSkidding, showDebug;

    public Mech_Controller controllerRef;
    public bool isHeld, canMove, grounded = true;
    [HideInInspector] public float legSpeed, legHeight;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 prevTargetPos = Vector3.zero, initialDir;
    private float maxLegDistance;

    void Start()
    {
        legHeight = targetPoint.position.y;

        prevTargetPos = targetPoint.position;

        Vector2 hip = new Vector2(hipBone.position.y, hipBone.position.z);
        Vector2 knee = new Vector2(kneeBone.position.y, kneeBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y, ankleBone.position.z);

        maxLegDistance = Vector2.Distance(hip, knee) + Vector2.Distance(knee, ankle);
    }

    void LateUpdate()
    {
        if (!isHeld) { CheckForGround(); return; }

        //RotateAnkleBone();
    }

    private void ApplyGravity()
    {
        targetPoint.position = new Vector3(targetPoint.position.x, targetPoint.position.y - Time.deltaTime, targetPoint.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundSnap.position, groundCheckDistance * 3);
    }

    public void CheckForGround()
    {
        initialDir.Normalize();
        targetPoint.gameObject.GetComponent<Target_Follow>().follow = true;
        
        RaycastHit hit;
        if (Physics.Raycast(groundSnap.position, Vector3.down * groundCheckDistance, out hit, groundCheckDistance, groundLayer))
        {
            legHeight = targetPoint.position.y;
            SetTargetFollowState(false);

            controllerRef.CheckLegIdleStatus();
            grounded = true;
            canMove = false;
        }
        else
        {
            grounded = false;
            ApplyGravity();
        }

        prevTargetPos = targetPoint.position;
    }

    private void RotateAnkleBone()
    {
        if (grounded) { Vector3 rot = new Vector3(0, 0, 0); targetPoint.rotation = Quaternion.Euler(rot); return; }

        Vector2 hip = new Vector2(hipBone.position.y / 2, hipBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y / 2, ankleBone.position.z);

        float currentLegLength = Vector2.Distance(hip, ankle);
        float displacement = currentLegLength / maxLegDistance;

        float rotAmnt = Vector2.Angle(ankle, hip);

        if (ankleBone.position.z - hipBone.position.z > 0) { rotAmnt *= -1; }

        rotAmnt *= rotationMultiplier * ankleCurve.Evaluate(displacement);

        Vector3 ankleRot = new Vector3(0, 90, rotAmnt - 90);

        ankleBone.eulerAngles = ankleRot;
    }

    public void SetTargetFollowState(bool newState) 
    { 
        if (isSkidding) { return; } 
        targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; 
    }
}
