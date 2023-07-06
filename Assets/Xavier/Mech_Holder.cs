using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mech_Holder : MonoBehaviour
{
    [SerializeField] private GameObject mech;

    public void MechDie(GameObject debris)
    {
        StartCoroutine(RespawnTimer(debris));
    }

    IEnumerator RespawnTimer(GameObject debris)
    {
        mech.SetActive(false);
        yield return new WaitForSeconds(5);
        mech.SetActive(true);
        mech.GetComponent<Mech_Controller>().Respawn();
        Destroy(debris);
    }
}
