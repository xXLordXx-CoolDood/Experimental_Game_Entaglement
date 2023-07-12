using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int value = 100;

    [SerializeField] private Material[] matList = new Material[5];

    private void Start()
    {
        for(int i = 0; i < matList.Length; i++)
        {
            if (GetComponent<MeshRenderer>().material.name.Substring(0, 12) == matList[i].name && tag != "Points")
            {
                value = 100 + i * 100;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Point_Getter>().GetPoints(value, gameObject);
        }
    }
}
