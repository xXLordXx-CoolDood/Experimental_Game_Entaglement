using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int value = 100;

    [SerializeField] private Material[] matList = new Material[5];

    private void Awake()
    {
        for(int i = 0; i < matList.Length; i++)
        {
            if(GetComponent<Renderer>().material == matList[i])
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
