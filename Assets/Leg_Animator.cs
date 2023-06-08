using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, ankleBone, hipBone;
    public float rotationMultiplier, backRotationMultiplier;

    [HideInInspector] public PROTO_Dog_Controller controllerRef;
    [HideInInspector] public bool isHeld, canMove, grounded;
    [HideInInspector] public float legSpeed;
    [HideInInspector] public int forwardMultiplier = 1;

    void Start()
    {

    }

    void Update()
    {
        RotateAnkleBone();
        Debug.Log($"Offset = {ankleBone.position.z - hipBone.position.z}");
    }

    private void RotateAnkleBone()
    {
        float rotAmnt = 0;

        //If ankle is forward, use forward rotation equation. If not, use backwards rotation equation
        if (ankleBone.position.z - hipBone.position.z > 0) { rotAmnt = (ankleBone.position.z - hipBone.position.z) 
                * -rotationMultiplier; }
        else { rotAmnt = (ankleBone.position.z - hipBone.position.z) * backRotationMultiplier; }

        Vector3 ankleRotation = new Vector3(rotAmnt, 0, 0);

        targetPoint.rotation = Quaternion.Euler(ankleRotation);
    }
}
