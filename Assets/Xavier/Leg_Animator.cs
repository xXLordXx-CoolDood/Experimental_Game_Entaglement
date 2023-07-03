using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, groundSnap, footBone, ankleBone, kneeBone, hipBone, mechHolder;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1, groundDistanceOffset = 0, leftMultiplier = 1, xRot;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;

    public bool isSkidding, showDebug;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed, heightOffset = 0;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 prevTargetPos = Vector3.zero;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundSnap.position, 0.33f);
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

    public void CheckForGround()
    {
        Vector3 dir = groundSnap.position - ankleBone.position;
        float dist = Vector3.Distance(groundSnap.position, ankleBone.position);

        grounded = false;

        Debug.DrawRay(groundSnap.position, Vector3.down, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ankleBone.position, dir.normalized, out hit, dist, groundLayer))
        {
            if (showDebug) { Debug.Log("grounded"); }
            SetTargetFollowState(false);
            grounded = true;
            canMove = false;
            footBone.position = new Vector3(footBone.position.x, hit.point.y - 0.075f, footBone.position.z);
            ankleBone.position = new Vector3(ankleBone.position.x, footBone.position.y + dist - 0.075f, ankleBone.position.z);
            targetPoint.GetComponent<Target_Follow>().follow = false;
            heightOffset = ankleBone.position.y - defaultHeightDif;
            targetPoint.position = ankleBone.position;
        }
        //else if(Physics.Raycast(groundSnap.position, dir, out hit, 1f, groundLayer) == false &&
        //    legAnimations.GetCurrentAnimatorStateInfo(0).IsTag("Cycle"))
        //{
        //    if (showDebug) { Debug.Log("airborne"); }
        //}

        prevTargetPos = targetPoint.position;
    }

    public void SetTargetFollowState(bool newState) 
    { 
        if (isSkidding) { return; } 

        targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; 
    }
}
