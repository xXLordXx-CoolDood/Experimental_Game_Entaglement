using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROTO_Leg_Animator : MonoBehaviour
{
    public Transform root, mid, legTarget;
    public LayerMask groundLayer;
    public AnimationCurve legHeightCurve;

    [HideInInspector] public bool isHeld, canMove;
    [HideInInspector] public float legSpeed, maxLegHeight;

    float maxLegDistance;

    void Awake()
    {
        legTarget.parent = null;

        maxLegDistance = Vector3.Distance(root.position, mid.position) + Vector3.Distance(mid.position, legTarget.position);
    }

    void LateUpdate()
    {
        if (!isHeld) { CheckForGround(); }

        UpdateLeg();
    }

    void CheckForGround()
    {
        RaycastHit hit;

        Vector3 checkHeight = new Vector3(legTarget.position.x, legTarget.position.y + maxLegDistance, legTarget.position.z);
        if (Physics.Raycast(checkHeight, -Vector3.up, out hit, maxLegDistance, groundLayer))
        {
            legTarget.position = hit.point;
            canMove = false;
        }
    }

    void UpdateLeg()
    {
        float time = legTarget.position.y / maxLegHeight;

        if (isHeld) { 
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y + (Time.deltaTime * legSpeed)
                , -maxLegHeight, maxLegHeight), legTarget.position.z);
        }
        else if(canMove){
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y - (Time.deltaTime * legSpeed
                 ), -maxLegHeight, maxLegHeight), legTarget.position.z + (Time.deltaTime * legSpeed));
        }
        else {
            legTarget.transform.position = new Vector3(legTarget.position.x, Mathf.Clamp(legTarget.position.y - (Time.deltaTime * legSpeed)
                , -maxLegHeight, maxLegHeight), legTarget.position.z);
        }
    }
}
