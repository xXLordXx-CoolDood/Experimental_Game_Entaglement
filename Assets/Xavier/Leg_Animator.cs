using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, footBone, ankleBone, kneeBone, hipBone;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1, groundDistanceOffset = 0;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;

    public bool isSkidding;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 initialDir = Vector3.zero, prevTargetPos = Vector3.zero;
    private float maxLegDistance = 0, defaultHeightDif;

    void Start()
    {
        initialDir = footBone.position - ankleBone.position;
        initialDir.Normalize();

        prevTargetPos = targetPoint.position;

        Vector2 hip = new Vector2(hipBone.position.y, hipBone.position.z);
        Vector2 knee = new Vector2(kneeBone.position.y, kneeBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y, ankleBone.position.z);

        maxLegDistance = Vector2.Distance(hip, knee) + 
            Vector2.Distance(knee, ankle);

        defaultHeightDif = hipBone.position.y - ankleBone.position.y;
    }

    void LateUpdate()
    {
        if (!grounded) { TargetGravity(); }
        if (!isHeld) { CheckForGround(); }

        RotateAnkleBone();
    }

    private void TargetGravity()
    {
        //targetPoint.position = new Vector3(targetPoint.position.x, targetPoint.position.y - Time.deltaTime, targetPoint.position.z);
    }

    public void CheckForGround()
    {
        if (targetPoint.position == prevTargetPos) { return; }

        initialDir = footBone.position - ankleBone.position;
        initialDir.Normalize();
        grounded = false;

        RaycastHit hit;
        if (Physics.Raycast(ankleBone.position, initialDir, out hit, groundCheckDistance, groundLayer))
        {
            SetTargetFollowState(false);
            grounded = true;
            canMove = false;
            footBone.position = new Vector3(hit.point.x, hit.point.y + groundDistanceOffset, hit.point.z);
            float ydif = targetPoint.position.y - footBone.position.y;
            targetPoint.position = new Vector3(targetPoint.position.x, hit.point.y + ydif, targetPoint.position.z);
        }
        else
        {
            targetPoint.gameObject.GetComponent<Target_Follow>().follow = true;
        }

        prevTargetPos = targetPoint.position;
    }

    private void RotateAnkleBone()
    {
        if (grounded) { /*Vector3 rot = new Vector3(0, 0, 0); targetPoint.rotation = Quaternion.Euler(rot);*/ return; }

        Vector2 hip = new Vector2(hipBone.position.y / 2, hipBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y / 2, ankleBone.position.z);

        float ZDif = ankleBone.position.z - hipBone.position.z;
        float heightDif = hipBone.position.y - ankleBone.position.y;

        float displacement = (ZDif / maxLegDistance) * -1;

        float rotAmnt = Vector2.Angle(ankle, hip);
        rotAmnt *= defaultHeightDif + 1.1f - heightDif;

        if (ankleBone.position.z - hipBone.position.z > 0) { rotAmnt *= -1.2f; displacement *= -1; }

        rotAmnt *= rotationMultiplier * ankleCurve.Evaluate(displacement);

        Vector3 ankleRot = new Vector3(rotAmnt, targetPoint.GetComponent<Target_Follow>().mech.transform.eulerAngles.y, 0);

        targetPoint.rotation = Quaternion.Euler(ankleRot);
    }

    public void SetTargetFollowState(bool newState) 
    { 
        if (isSkidding) { return; } 

        targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; 
    }
}
