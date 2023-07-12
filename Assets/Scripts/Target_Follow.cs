using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Target_Follow : MonoBehaviour
{
    public Animator anim;
    public Transform target, mech, pivot, hipBone, groundSnap;
    public float maxLegDistance = 3.5f;
    public LayerMask groundLayer;

    [SerializeField] public bool follow = true, isSkidding;

    private Vector3 prevTargetPos = Vector3.zero, prevPos = Vector3.zero;
    private bool isTooFar;

    private void Start()
    {
        prevTargetPos = target.position;
        prevPos = transform.position;
    }

    private void Update()
    {
        #region legsnap
        pivot.eulerAngles = new Vector3(0, pivot.eulerAngles.y, pivot.eulerAngles.z);
        Debug.DrawRay(pivot.position, pivot.forward * 3, Color.green);
        Vector3 posOnLine = Vector3.Project(transform.position - pivot.position, pivot.forward) + new Vector3(pivot.position.x, transform.position.y, pivot.position.z);

        if (Vector3.Distance(transform.position, posOnLine) > 0.25f && (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower")))
        { transform.position = posOnLine; }

        //Get x/z vector towards target position. Get the direction we are currently moving as well
        Vector3 calcDir = target.position - transform.position;
        Vector3 movingDir = prevPos - transform.position;
        calcDir.y = 0;
        movingDir.y = 0;
        calcDir.Normalize();
        movingDir.Normalize();

        //Check if leg is too far from target point
        if (!SimilarDirections(calcDir, movingDir) && Vector3.Distance(transform.position, target.position) > maxLegDistance) //If applicable, move back towards the mech by a small amount
        {
            if(mech.GetComponent<Mech_Controller>().isSkidding && mech.GetComponent<Mech_Controller>().iceMultiplier > 5)
            {
                transform.position = target.position;
                return;
            }

            transform.position += calcDir * Time.deltaTime * 5;
            isTooFar = true;

            prevTargetPos = target.position;
            return;
        }
        else { isTooFar = false; }
        #endregion

        float dirMultiplier = Mathf.Clamp(mech.GetComponent<Mech_Controller>().waist.localEulerAngles.y - 270, -1, 1);

        Vector3 start = transform.position + (transform.right * dirMultiplier);
        Debug.DrawRay(start, transform.right * 1.5f * -dirMultiplier);

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Cycle") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Mid"))
        {
            prevTargetPos = target.position;
        }

        if (!follow && !isSkidding) { prevPos = transform.position; prevTargetPos = target.position; return; }

        if (isSkidding)
        {
            RaycastHit hit;
            if(Physics.Raycast(hipBone.position, Vector3.down, out hit, Vector3.Distance(hipBone.position, groundSnap.position), groundLayer))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + (transform.position.y - groundSnap.position.y), hit.point.z); 
            }
            else { transform.position = target.position; }
            prevTargetPos = target.position;
            prevPos = transform.position;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Rise") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Lower") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Kneel")) 
        {
            //Calculate position difference to pivot
            Vector3 delta = target.position - prevTargetPos;
            mech.GetComponent<Mech_Controller>().idleTimer = 0;
            transform.position = new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, transform.position.z + delta.z);
            prevTargetPos = target.position;
        }
    }

    private bool SimilarDirections(Vector3 calculatedDirection, Vector3 movingDirection)
    {
        float dotProduct = Vector3.Dot(calculatedDirection, movingDirection);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angle <= 45;
    }

    public void ResetLeg()
    {
        if (Vector3.Distance(transform.position, target.position) > 1)
        {
            transform.position = target.position;
            target.parent.GetComponent<Leg_Animator>().CheckForGround();
        }
    }
}
