using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROTO_Leg_Animator : MonoBehaviour
{
    public Transform root, mid, legTarget;
    public LayerMask groundLayer;
    public AnimationCurve legHeightCurve;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed;
    [HideInInspector] public int forwardMultiplier = 1;

    float maxLegDistance;

    void Awake()
    {
        //legTarget.parent = transform.parent.parent.parent;

        maxLegDistance = Vector3.Distance(root.position, mid.position) + Vector3.Distance(mid.position, legTarget.position);
    }

    void LateUpdate()
    {
        if (!isHeld) { CheckForGround(); }
        Debug.Log(forwardMultiplier);
        UpdateLeg();
    }

    void CheckForGround()
    {
        RaycastHit hit;
        grounded = false;
        Vector3 checkHeight = new Vector3(legTarget.position.x, legTarget.position.y + maxLegDistance, legTarget.position.z);
        if (Physics.Raycast(checkHeight, -Vector3.up, out hit, maxLegDistance, groundLayer))
        {
            legTarget.position = hit.point;
            canMove = false;
            grounded = true;
        }
    }

    public void Rotate(Vector3 delta)
    {
        legTarget.transform.Rotate(delta, Time.deltaTime * 20);
    }

    void UpdateLeg()
    {
        if (isHeld && (controllerRef.engagedLegs == 1 || controllerRef.opposites)) {
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y + (Time.deltaTime * legSpeed)
                , -controllerRef.maxLegHeight, controllerRef.maxLegHeight), legTarget.position.z)/* + (legTarget.transform.forward.normalized * Time.deltaTime)*/;
        }
        else if(canMove){
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y - (Time.deltaTime * legSpeed
                 ), -controllerRef.maxLegHeight, controllerRef.maxLegHeight), legTarget.position.z + (Time.deltaTime * legSpeed * forwardMultiplier)) 
                + (legTarget.transform.forward.normalized * Time.deltaTime * forwardMultiplier);
            controllerRef.AddMoveRotation();
        }
        else if(!grounded){
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y - (Time.deltaTime * legSpeed)
                , -controllerRef.maxLegHeight, controllerRef.maxLegHeight), legTarget.position.z) + (legTarget.transform.forward.normalized * Time.deltaTime * forwardMultiplier);
        }
    }
}
