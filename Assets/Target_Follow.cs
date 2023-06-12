using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Follow : MonoBehaviour
{
    public Transform target;
    //public Leg_Animator legReference;
    public bool follow = true;

    private void Update()
    {
        if (follow) { transform.position = target.position; }
    }
}
