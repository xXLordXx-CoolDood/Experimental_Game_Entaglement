using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Follow : MonoBehaviour
{
    public Animator anim;
    public Transform target;
    public Transform mech;

    [HideInInspector] public bool follow = true;

    private Vector2 prevTargetZY = Vector2.zero;

    private void Start()
    {
        prevTargetZY = new Vector2(target.position.z, target.position.y);
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid"))
        {
            prevTargetZY = new Vector2(target.position.z, target.position.y);
        }

        if (!follow) { return; }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower")) {
            Vector2 targetZY = new Vector2(target.position.z, target.position.y);

            Vector2 zyDelta = targetZY - prevTargetZY;

            transform.position = new Vector3(transform.position.x, transform.position.y + zyDelta.y, 
                transform.position.z + zyDelta.x);

            prevTargetZY = targetZY;
        }
    }

    private void CheckForGround()
    {

    }
}
