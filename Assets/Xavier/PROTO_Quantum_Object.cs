using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROTO_Quantum_Object : MonoBehaviour
{
    public bool syncPositions;
    public Rigidbody rbPartner;
    public float switchThreshold;

    private Rigidbody rb;
    private Vector3 selfPreviousVelocity, partnerPreviousVelocity, selfPreviousPosition, partnerPreviousPosition;
    [SerializeField] private bool beta;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        selfPreviousPosition = transform.position;
        partnerPreviousPosition = rbPartner.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 myPosDelta = transform.position - selfPreviousPosition;
        Vector3 partnerPosDelta = rbPartner.transform.position - partnerPreviousPosition;

        if(Mathf.Abs(rb.velocity.magnitude) < Mathf.Abs(partnerPreviousVelocity.magnitude) * switchThreshold) 
        { beta = !beta; }

        if(!beta) 
        {
            rbPartner.velocity = rb.velocity;
            if (syncPositions) { rbPartner.transform.position += (myPosDelta - partnerPosDelta); }
        }
        else 
        {
            rb.velocity = rbPartner.velocity;
            if (syncPositions) { transform.position += (partnerPosDelta - myPosDelta); }
        }

        selfPreviousVelocity = rb.velocity;
        selfPreviousPosition = transform.position;
        partnerPreviousVelocity = rbPartner.velocity;
        partnerPreviousPosition = rbPartner.transform.position;
    }
}
