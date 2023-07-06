using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MechGun : MonoBehaviour
{
    public bool isReadyToShoot;

    Tuple<bool?, bool?, bool?> shootSequence;

    public float gun1, gun2, gun3;
    public Image on1, on2, on3, off1, off2, off3;

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

        if(shootSequence.Item1 != null)
        {
            on1.color = new Color32(255, 255, 0, Convert.ToByte(255 - (Convert.ToInt32(shootSequence.Item1) * 180)));
            off1.color = new Color32(33, 255, 0, Convert.ToByte((Mathf.Clamp(Convert.ToInt32(shootSequence.Item1), 0, 1) * 180) + 75));
        }
        if(shootSequence.Item2 != null)
        {
            on2.color = new Color32(255, 255, 0, Convert.ToByte(255 - (Convert.ToInt32(shootSequence.Item2) * 180)));
            off2.color = new Color32(33, 255, 0, Convert.ToByte((Mathf.Clamp(Convert.ToInt32(shootSequence.Item2), 0, 1) * 180) + 75));
        }
        if(shootSequence.Item3 != null)
        {
            on3.color = new Color32(255, 255, 0, Convert.ToByte(255 - (Convert.ToInt32(shootSequence.Item3) * 180)));
            off3.color = new Color32(33, 255, 0, Convert.ToByte((Mathf.Clamp(Convert.ToInt32(shootSequence.Item3), 0, 1) * 180) + 75));
        }
        Debug.Log(shootSequence);
    }
}
