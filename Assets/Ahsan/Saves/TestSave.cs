using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSave : MonoBehaviour
{
    private void Awake()
    {
        SaveLoadScript.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        SaveData[] saveData = new SaveData[]
        {
                new SaveData{ p1Name = "AAA" },
                new SaveData{ p1Name = "NNN" },
                new SaveData{ p1Name = "BBB" }
        };
        string jsonSave = JsonHelper.ToJson(saveData);
        Debug.Log(jsonSave);
        SaveLoadScript.Save(jsonSave);
        
        string jsonLoad = SaveLoadScript.Load();
        SaveData[] loadData = JsonHelper.FromJson<SaveData>(jsonLoad);
        Debug.Log(loadData);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
