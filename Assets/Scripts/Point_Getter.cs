using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Point_Getter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText, timerText;

    public float time = 300, gColor;
    private int points = 0, multiplier = 1;

    private void Start()
    {
        scoreText.text = $"Score - {points}pts";
    }

    private void Update()
    {
        time -= Time.deltaTime;
        gColor += Time.deltaTime * multiplier * 510;

        if(gColor > 255) { gColor = 255; multiplier = -1; }
        if(gColor < 0) { gColor = 0; multiplier = 1; }

        timerText.text = $"T I M E - {Math.Round(time, 2)}";
        timerText.color = new Color32(255, Convert.ToByte(gColor), 0, 255);

        if(time <= 0)
        {
            PlayerPrefs.SetInt("points", points);
            SceneManager.LoadScene("NameScreenUI");
        }
    }

    public void GetPoints(int _points, GameObject _object)
    {
        Debug.Log(_points);
        points += _points;
        scoreText.text = $"Score - {points}pts";
        Destroy(_object);
    }
}
