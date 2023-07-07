using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int value = 100;

    [SerializeField] private Material[] matList = new Material[5];

    private void Awake()
    {
        switch (value)
        {
            case 100: GetComponent<Renderer>().material = matList[0]; break;
            case 200: GetComponent<Renderer>().material = matList[1]; break;
            case 300: GetComponent<Renderer>().material = matList[2]; break;
            case 400: GetComponent<Renderer>().material = matList[3]; break;
            case 500: GetComponent<Renderer>().material = matList[4]; break;
            default: break;
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
