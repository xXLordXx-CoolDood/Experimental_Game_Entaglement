using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Follow : MonoBehaviour
{
    public Animator anim;
    public Transform target, mech;

    [SerializeField] public bool follow = true, isSkidding;

    private Vector3 prevTargetPos = Vector3.zero;

    private void Start()
    {
        prevTargetPos = target.position;
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid"))
        {
            prevTargetPos = target.position;
            return;
        }

        if (!follow && !isSkidding) { return; }

        if (isSkidding)
        {
            transform.position = target.position;
            prevTargetPos = target.position;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") ||
                anim.GetCurrentAnimatorStateInfo(0).IsTag("Kneel")) {
            Vector3 delta = target.position - prevTargetPos;

            transform.position = new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, 
                transform.position.z + delta.z);

            prevTargetPos = target.position;
        }
    }
}
