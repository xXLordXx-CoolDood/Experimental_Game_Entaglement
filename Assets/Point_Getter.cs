using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Point_Getter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private int points = 0;

    private void Start()
    {
        scoreText.text = $"Score - {points}pts";
    }

    public void GetPoints(int _points, GameObject _object)
    {
        points += _points;
        scoreText.text = $"Score - {points}pts";
        Destroy(_object);
    }
}
