using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechGun : MonoBehaviour
{
    public bool isReadyToShoot;

    Tuple<bool?, bool?, bool?> shootSequence;

    public float gun1, gun2, gun3;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSequence();
    }

    // Update is called once per frame
    void Update()
    {
        Tuple<bool?, bool?, bool?> switchSequence = new Tuple<bool?, bool?, bool?> (
            gun1 > 0 ? true : gun1 < 0 ? false : null, 
            gun2 > 0 ? true : gun2 < 0 ? false : null,
            gun3 > 0 ? true : gun3 < 0 ? false : null
            );

        if (switchSequence == shootSequence)
        {
            isReadyToShoot = true;
        }
    }

    
    public void GenerateSequence()
    {
        Tuple<bool?, bool?, bool?> newSequence = new Tuple<bool?, bool?, bool?>(UnityEngine.Random.Range(0f, 1f) > 0.5f, UnityEngine.Random.Range(0f, 1f) > 0.5f, UnityEngine.Random.Range(0f, 1f) > 0.5f);
        while (shootSequence == newSequence)
        {
            newSequence = new Tuple<bool?, bool?, bool?>(UnityEngine.Random.Range(0f, 1f) > 0.5f, UnityEngine.Random.Range(0f, 1f) > 0.5f, UnityEngine.Random.Range(0f, 1f) > 0.5f);
        }
        shootSequence = newSequence;
        Debug.Log(shootSequence);
    }
}
