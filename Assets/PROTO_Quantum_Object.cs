using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROTO_Quantum_Object : MonoBehaviour
{
    public Rigidbody rbPartner;
    public float switchThreshold;

    private Rigidbody rb;
    public Vector3 selfPreviousVelocity, partnerPreviousVelocity;
    [SerializeField] private bool beta;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(rb.velocity.magnitude) < Mathf.Abs(partnerPreviousVelocity.magnitude) * switchThreshold && !beta) 
        { beta = true; }

        if(Mathf.Abs(rb.velocity.magnitude) * switchThreshold > Mathf.Abs(partnerPreviousVelocity.magnitude) && beta) 
        { beta = false; }

        if(!beta) 
        {
            rbPartner.velocity = rb.velocity;
        }
        else 
        {
            rb.velocity = rbPartner.velocity;
        }

        selfPreviousVelocity = rb.velocity;
        partnerPreviousVelocity = rbPartner.velocity;
    }
}
