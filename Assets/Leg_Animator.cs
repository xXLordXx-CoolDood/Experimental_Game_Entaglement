using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, footBone, ankleBone, kneeBone, hipBone;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1;
    public AnimationCurve ankleCurve;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 initialDir = Vector3.zero, prevTargetPos = Vector3.zero;
    private float maxLegDistance = 0;

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
    }

    void Update()
    {
        HandleTarget();
        RotateAnkleBone();
    }

    private void HandleTarget()
    {
        initialDir = footBone.position - ankleBone.position;
        initialDir.Normalize();

        grounded = false;

        RaycastHit hit;
        if (Physics.Raycast(ankleBone.position, initialDir, out hit, groundCheckDistance))
        {
            grounded = true;
            footBone.position = hit.point;
        }

        //if (legLength > maxLegDistance) { targetPoint.position = prevTargetPos; return; }

        //prevTargetPos = targetPoint.position;
    }

    private void RotateAnkleBone()
    {
        if (grounded) { Vector3 rot = new Vector3(0, 0, 0); targetPoint.rotation = Quaternion.Euler(rot); return; }

        Vector2 hip = new Vector2(hipBone.position.y / 2, hipBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y / 2, ankleBone.position.z);

        float currentLegLength = Vector2.Distance(hip, ankle);
        float displacement = currentLegLength / maxLegDistance;

        float rotAmnt = Vector2.Angle(ankle, hip);

        if(ankleBone.position.z - hipBone.position.z > 0) { rotAmnt *= -1; }

        rotAmnt *= rotationMultiplier * ankleCurve.Evaluate(displacement);

        Vector3 ankleRot = new Vector3(rotAmnt, 0, 0);

        targetPoint.rotation = Quaternion.Euler(ankleRot);
    }
}
