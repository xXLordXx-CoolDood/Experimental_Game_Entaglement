using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, groundSnap, footBone, ankleBone, kneeBone, hipBone, mechHolder;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;

    public bool isSkidding, showDebug;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed, heightOffset = 0;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 prevTargetPos = Vector3.zero, initialDir;
    private float defaultHeightDif, maxLegDistance;

    void Start()
    {
        defaultHeightDif = ankleBone.position.y;
        heightOffset = -defaultHeightDif;

        prevTargetPos = targetPoint.position;

        Vector2 hip = new Vector2(hipBone.position.y, hipBone.position.z);
        Vector2 knee = new Vector2(kneeBone.position.y, kneeBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y, ankleBone.position.z);

        maxLegDistance = Vector2.Distance(hip, knee) +
    Vector2.Distance(knee, ankle);
    }

    void LateUpdate()
    {
        if (!isHeld) { CheckForGround(); }
        RotateAnkleBone();
    }

    public void CheckForGround()
    {
        if (targetPoint.position == prevTargetPos) { return; }

        initialDir = footBone.position - ankleBone.position;
        initialDir.Normalize();
        grounded = false;
        targetPoint.gameObject.GetComponent<Target_Follow>().follow = true;

        Debug.DrawRay(ankleBone.position, initialDir, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ankleBone.position, initialDir, out hit, groundCheckDistance, groundLayer))
        {
            SetTargetFollowState(false);
            grounded = true;
            canMove = false;
            footBone.position = hit.point;
            float ydif = targetPoint.position.y - footBone.position.y;
            targetPoint.position = new Vector3(targetPoint.position.x, hit.point.y + ydif, targetPoint.position.z);
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

    public void SetTargetFollowState(bool newState) { if (isSkidding) { return; } targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; }
}
