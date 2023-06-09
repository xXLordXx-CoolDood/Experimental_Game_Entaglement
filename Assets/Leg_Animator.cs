using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, ankleBone, hipBone;
    public float rotationMultiplier;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed;
    [HideInInspector] public int forwardMultiplier = 1;

    void Start()
    {

    }

    void Update()
    {
        //HandleTarget();
        RotateAnkleBone();
    }

    private void HandleTarget()
    {

    }

    private void RotateAnkleBone()
    {
        Vector2 hip = new Vector2(hipBone.position.x, hipBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.x, ankleBone.position.z);

        float rotAmnt = Vector2.Angle(ankle, hip);

        if(ankleBone.position.z - hipBone.position.z > 0) { rotAmnt *= -1; }

        rotAmnt *= rotationMultiplier;

        Vector3 ankleRot = new Vector3(rotAmnt, 0, 0);

        targetPoint.rotation = Quaternion.Euler(ankleRot);
    }
}
