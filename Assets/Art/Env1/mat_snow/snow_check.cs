using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class snow_check : MonoBehaviour
{
    public VisualEffect SnowCheck;

    void OnCollisionEnter(Collision collision)
    {
        SnowCheck.SendEvent("SnowCheckOn");
        Debug.Log("Collision");
    }

    void OnCollisionExit(Collision collision)
    {
        SnowCheck.SendEvent("SnowCheckOff");
        Debug.Log("No Collision");
    }
}
