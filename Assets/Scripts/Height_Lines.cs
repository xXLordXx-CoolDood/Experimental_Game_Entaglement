using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Height_Lines : MonoBehaviour
{
    public float spacing, height, width, heightFalloff, widthFalloff;
    public int numOfLines;
    public Color32 color;

    private void Start()
    {
        GameObject obj = new GameObject();
        obj.AddComponent<Image>();
        obj.GetComponent<Image>().color = color;

        for (int i = 0; i < numOfLines; i++)
        {
            GameObject spawned = Instantiate(obj, transform);
            spawned.transform.position = new Vector3(spawned.transform.position.x, spawned.transform.position.y + (spacing * (i + 1)), spawned.transform.position.z);
            spawned.transform.localScale = new Vector3(width - (widthFalloff * i), height - (heightFalloff * i), 1);
        }

        for (int i = 0; i < numOfLines; i++)
        {
            GameObject spawned = Instantiate(obj, transform);
            spawned.transform.position = new Vector3(spawned.transform.position.x, spawned.transform.position.y - (spacing * (i + 1)), spawned.transform.position.z);
            spawned.transform.localScale = new Vector3(width - (widthFalloff * i), height - (heightFalloff * i), 1);
        }
    }
}
