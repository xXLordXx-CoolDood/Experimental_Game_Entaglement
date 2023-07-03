using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, groundSnap, footBone, ankleBone, kneeBone, hipBone;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1, groundDistanceOffset = 0;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;

    public bool isSkidding;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed, heightOffset = 0;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 prevTargetPos = Vector3.zero;
    private float maxLegDistance = 0, defaultHeightDif;

    void Start()
    {
        defaultHeightDif = ankleBone.position.y;
        heightOffset = -defaultHeightDif;

        prevTargetPos = targetPoint.position;

        Vector2 hip = new Vector2(hipBone.position.y, hipBone.position.z);
        Vector2 knee = new Vector2(kneeBone.position.y, kneeBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y, ankleBone.position.z);

        maxLegDistance = Vector2.Distance(hip, knee) + Vector2.Distance(knee, ankle);
    }

    void LateUpdate()
    {
        if (!grounded) { TargetGravity(); }
        if (!isHeld) { CheckForGround(); }

        //RotateAnkleBone();
    }

    private void TargetGravity()
    {
        //targetPoint.position = new Vector3(targetPoint.position.x, targetPoint.position.y - Time.deltaTime, targetPoint.position.z);
    }

    public void CheckForGround()
    {
        Vector3 dir = groundSnap.position - ankleBone.position;
        float dist = Vector3.Distance(groundSnap.position, ankleBone.position);

        grounded = false;

        RaycastHit hit;
        if (Physics.Raycast(ankleBone.position, dir.normalized, out hit, dist, groundLayer))
        {
            SetTargetFollowState(false);
            grounded = true;
            canMove = false;
            footBone.position = new Vector3(footBone.position.x, hit.point.y + 0.075f, footBone.position.z);
            ankleBone.position = new Vector3(ankleBone.position.x, footBone.position.y + dist, ankleBone.position.z);
            targetPoint.GetComponent<Target_Follow>().follow = false;
            targetPoint.position = ankleBone.position;
            heightOffset = ankleBone.position.y - defaultHeightDif;
        }
        //else
        //{
        //    targetPoint.gameObject.GetComponent<Target_Follow>().follow = true;
        //}

        prevTargetPos = targetPoint.position;
    }

    public void SetTargetFollowState(bool newState) 
    { 
        if (isSkidding) { return; } 

        targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; 
    }
}
