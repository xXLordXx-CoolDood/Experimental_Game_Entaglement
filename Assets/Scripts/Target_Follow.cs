using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Target_Follow : MonoBehaviour
{
    public Animator anim;
    public Transform target;
    public Transform mech, pivot;
    public LayerMask quadLayer;

    [SerializeField] public bool follow = true, isSkidding;

    private Vector3 prevTargetPos = Vector3.zero;

    private void Start()
    {
        prevTargetPos = target.position;
    }

    private void Update()
    {
        Vector3 start = transform.position + transform.right;

        Debug.DrawRay(start, -transform.right * 2);

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid"))
        {
            prevTargetPos = target.position;
        }

        if (!follow && !isSkidding) { return; }

        if (isSkidding)
        {
            transform.position = target.position;
            prevTargetPos = target.position;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") ||
                anim.GetCurrentAnimatorStateInfo(0).IsTag("Kneel")) 
        {
            //Calculate position difference to pivot
            Vector3 delta = target.position - prevTargetPos;

            transform.position = new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, transform.position.z + delta.z);

            RaycastHit hit;
            if (Physics.Raycast(start, -transform.right, out hit, 2, quadLayer))
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            prevTargetPos = target.position;
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}
