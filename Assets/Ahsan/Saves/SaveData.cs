using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string p1Name;
    public string p2Name;
    public int score;

    public string GetFormattedData()
    {
        return p1Name + "  &  " + p2Name + "   " + score.ToString();
    }
}


