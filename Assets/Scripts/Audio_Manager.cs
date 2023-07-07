using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Multiple Manager Instances!");
        }

        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
