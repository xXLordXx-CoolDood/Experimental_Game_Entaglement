using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Follow : MonoBehaviour
{
    public Animator anim;
    public Transform target;

    private Vector2 prevTargetZY = Vector2.zero;
    public float zOffset = 0;

    private void Start()
    {
        prevTargetZY = new Vector2(target.position.z, target.position.y);
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Change")) {
            Vector2 targetZY = new Vector2(target.position.z, target.position.y);

            Vector2 zyDelta = targetZY - prevTargetZY;

            transform.position = new Vector3(transform.position.x, transform.position.y + zyDelta.y, 
                transform.position.z + zyDelta.x);

            prevTargetZY = targetZY;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle")) {
            prevTargetZY = new Vector2(target.position.z, target.position.y);
            zOffset = target.position.z - transform.position.z;
        }
    }
}
