using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadScript
{
    private static readonly string SaveLocation = Application.dataPath + "/Saves/";
   
    public static string Load()
    {
        if (File.Exists(SaveLocation + "save.txt"))
        {
            string loadValue = File.ReadAllText(SaveLocation + "save.txt");
            return loadValue;
        }
        return null;
    }

    public static void Save(string saveValue)
    {
        File.WriteAllText(SaveLocation + "save.txt", saveValue);
    }


    // Start is called before the first frame update
    public static void Init()
    {
        if (!Directory.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
    }

}
