using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech_Controller : MonoBehaviour
{
    public Transform FRLeg, BRLeg, FLLeg, BLLeg;

    private float heightOffset = 1;

    void Start()
    {
        heightOffset = transform.position.y - FRLeg.position.y;
    }

    void Update()
    {
        UpdateBodyPosition();
    }

    private void UpdateBodyPosition() {
        float averageX = (FRLeg.position.x + BRLeg.position.x + FLLeg.position.x + BLLeg.position.x) / 4;
        float averageY = (FRLeg.position.y + BRLeg.position.y + FLLeg.position.y + BLLeg.position.y) / 4;
        float averageZ = (FRLeg.position.z + BRLeg.position.z + FLLeg.position.z + BLLeg.position.z) / 4;

        transform.position = new Vector3(averageX, averageY + heightOffset, averageZ);
    }

    private void UpdateBodyRotation() {

    }
}
