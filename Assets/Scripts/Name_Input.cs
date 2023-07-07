using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class Name_Input : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] letter;
    [SerializeField] private Transform upLetters, downLetters;
    [SerializeField] private char[] letters;

    public float speed;

    private int index = 1, letterIndex;
    private float counter, yOffset;

    private void Update()
    {
        if(counter > 54.42f) { counter -= 54.42f; index--; }
        if(counter < 0) { counter += 54.42f; index++; }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            counter += Time.deltaTime * speed;
            upLetters.position = new Vector3(upLetters.position.x, upLetters.position.y + Time.deltaTime * speed, 0);
            downLetters.position = new Vector3(downLetters.position.x, downLetters.position.y + Time.deltaTime * speed, 0);
        }

        if (Keyboard.current.downArrowKey.isPressed)
        {
            counter -= Time.deltaTime * speed;
            upLetters.position = new Vector3(upLetters.position.x, upLetters.position.y - Time.deltaTime * speed, 0);
            downLetters.position = new Vector3(downLetters.position.x, downLetters.position.y - Time.deltaTime * speed, 0);
        }

        if (!Keyboard.current.upArrowKey.isPressed && !Keyboard.current.downArrowKey.isPressed)
        {
            SnapLetter();
        }

        if (upLetters.localPosition.y < -75) { upLetters.localPosition = new Vector3(upLetters.localPosition.x, 1415, 0); }
        if (upLetters.localPosition.y > 1415) { upLetters.localPosition = new Vector3(upLetters.localPosition.x, -75, 0); }
        if (downLetters.localPosition.y < -75) { downLetters.localPosition = new Vector3(downLetters.localPosition.x, 1415, 0); }
        if (downLetters.localPosition.y > 1415) { downLetters.localPosition = new Vector3(downLetters.localPosition.x, -75, 0); }
    }

    private void SnapLetter()
    {
        //upLetters.localPosition = new Vector3(upLetters.localPosition.x, , 0);
        Debug.Log("Snap");
    }
}

//28.5
