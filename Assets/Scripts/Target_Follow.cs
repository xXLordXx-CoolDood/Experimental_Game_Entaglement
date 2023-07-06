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
    private float maxLegDistance;

    private Vector3 prevTargetPos = Vector3.zero;

    private void Start()
    {
        maxLegDistance = 2.3f;
        prevTargetPos = target.position;
    }

    private void Update()
    {
        float dirMultiplier = Mathf.Clamp(mech.GetComponent<Mech_Controller>().waist.localEulerAngles.y - 270, -1, 1);

        Vector3 start = transform.position + (transform.right * dirMultiplier);
        Debug.DrawRay(start, transform.right * 1.5f * -dirMultiplier);

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid"))
        {
            prevTargetPos = target.position;
        }

        if (!follow && !isSkidding) { return; }

        if (isSkidding)
        {
            TryMoveToPos(target.position);
            prevTargetPos = target.position;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Kneel")) 
        {
            //Calculate position difference to pivot
            Vector3 delta = target.position - prevTargetPos;

            TryMoveToPos(new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, transform.position.z + delta.z));

            RaycastHit hit;
            if (Physics.Raycast(start, transform.right * -dirMultiplier, out hit, 1.5f, quadLayer) && (dirMultiplier == 1 || dirMultiplier == -1))
            {
                TryMoveToPos(Vector3.Lerp(transform.position, hit.point, anim.GetCurrentAnimatorStateInfo(0).normalizedTime));
                Debug.Log("Snapped");
            }

            prevTargetPos = target.position;
        }
    }

    private void TryMoveToPos(Vector3 newPos)
    { 
        if(Vector3.Distance(pivot.position, newPos) < maxLegDistance) { transform.position = newPos; return; }

        Vector3 fixedPos = Vector3.Normalize(pivot.position - newPos);
        fixedPos += transform.position;

        transform.position = new Vector3(fixedPos.x, newPos.y, fixedPos.z);
    }
}
