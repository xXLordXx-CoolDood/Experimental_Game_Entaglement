using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_DogLikeMech : MonoBehaviour
{
    public Transform FL, BL, FR, BR, myBody;
    public float maxLegHeight = 3f;
    public float legSpeed;
    public float rotationMultiplier = 1;
    public LayerMask groundLayer;
    public float maxLegLength;

    private PlayerInput playerInput;
    private bool FLB, BLB, FRB, BRB;
    private bool FLgrounded, BLgrounded, FRgrounded, BRgrounded;
    private float offsetY, offsetZ;
    private float legHeightOffset;
    private int groundedLegs;

    // Start is called before the first frame update
    void Awake()
    {
        //float legLength = Vector3.Distance(b1.position, b2.position);
        //legLength += Vector3.Distance(b2.position, b3.position);
        //legLength += Vector3.Distance(b3.position, b4.position);
        //Debug.Log(legLength);

        playerInput = GetComponent<PlayerInput>();
        offsetY = transform.position.y;
        offsetZ = (FR.position.z + FL.position.z + BR.position.z + BL.position.z) / 4;
        legHeightOffset = transform.position.y - maxLegHeight;
    }

    #region
    public void LeftLegF(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { FLB = true; }
        if(ctx.canceled) { FLB = false; }
    }
    public void RightLegF(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { FRB = true; }
        if (ctx.canceled) { FRB = false; }
    }
    public void LeftLegB(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { BLB = true; }
        if (ctx.canceled) { BLB = false; }
    }

    public void RightLegB(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { BRB = true; }
        if (ctx.canceled) { BRB = false; }
    }

    #endregion

    private void LateUpdate()
    {
        FixTargets();
        UpdateLegs();
        UpdateBody();
    }

    void FixTargets()
    {
        RaycastHit hit;
        FLgrounded = false; BLgrounded = false; FRgrounded = false; BRgrounded = false;
        groundedLegs = 0;

        if (Physics.Raycast(FL.position, -FL.up + FL.forward, out hit, 0.1f, groundLayer) && !FLB) 
        {
            FLgrounded = true; groundedLegs++;
        }
        if (Physics.Raycast(BL.position, -BL.up + BL.forward, out hit, 0.1f, groundLayer) && !BLB)
        {
            BLgrounded = true; groundedLegs++;
        }
        if (Physics.Raycast(FR.position, -FR.up + FR.forward, out hit, 0.1f, groundLayer) && !FRB)
        {
            FRgrounded = true; groundedLegs++;
        }
        if (Physics.Raycast(BR.position, -BR.up + BR.forward, out hit, 0.1f, groundLayer) && !BRB)
        {
            BRgrounded = true; groundedLegs++;
        }
    }

    void UpdateLegs()
    {
        if (FLB && groundedLegs > 1) { FL.position += new Vector3(0, 1 * Time.deltaTime * legSpeed, 0); } //If pressed
        else if(!FLB && !FLgrounded) { FL.localPosition -= new Vector3(0, 1 * Time.deltaTime * legSpeed, -1 * Time.deltaTime * legSpeed) ; }

        FL.position = new Vector3(FL.position.x, Mathf.Clamp(FL.position.y, -0.051f, maxLegHeight), FL.position.z);

        if (BLB && groundedLegs > 1) { BL.position += new Vector3(0, 1 * Time.deltaTime * legSpeed, 0); }
        else if (!BLB && !BLgrounded) { BL.localPosition -= new Vector3(0, 1 * Time.deltaTime * legSpeed, -1 * Time.deltaTime * legSpeed); }

        BL.position = new Vector3(BL.position.x, Mathf.Clamp(BL.position.y, -0.051f, maxLegHeight), BL.position.z);

        if (FRB && groundedLegs > 1) { FR.position += new Vector3(0, 1 * Time.deltaTime * legSpeed, 0); }
        else if (!FRB && !FRgrounded) { FR.localPosition -= new Vector3(0, 1 * Time.deltaTime * legSpeed, -1 * Time.deltaTime * legSpeed); }

        FR.position = new Vector3(FR.position.x, Mathf.Clamp(FR.position.y, -0.051f, maxLegHeight), FR.position.z);

        if (BRB && groundedLegs > 1) { BR.position += new Vector3(0, 1 * Time.deltaTime * legSpeed, 0); }
        else if (!BRB && !BRgrounded) { BR.localPosition -= new Vector3(0, 1 * Time.deltaTime * legSpeed, -1 * Time.deltaTime * legSpeed); }

        BR.position = new Vector3(BR.position.x, Mathf.Clamp(BR.position.y, -0.051f, maxLegHeight), BR.position.z);
    }

    void UpdateBody()
    {
        //Move forward
        transform.position = new Vector3(transform.position.x, ((FR.position.y + FL.position.y + BR.position.y + BL.position.y) / 4) + offsetY, ((FR.position.z + FL.position.z + BR.position.z + BL.position.z) / 4) - offsetZ);
        maxLegHeight = transform.position.y - legHeightOffset;

        //Rotate front/back
        float averageFrontHeight = (FR.position.y + FL.position.y) / 2;
        float averageBackHeight = (BR.position.y + BL.position.y) / 2;

        float angle = Mathf.Atan2(averageFrontHeight - averageBackHeight, Vector3.Distance(transform.position, transform.position + Vector3.forward)) * Mathf.Rad2Deg;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = angle * -rotationMultiplier;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
