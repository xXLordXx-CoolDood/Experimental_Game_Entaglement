using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreenScript : MonoBehaviour
{
    UIDocument uiDoc;
    // Start is called before the first frame update
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();

        

        
        

        var root = uiDoc.rootVisualElement;

        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMechScene()
    {
        SceneManager.LoadScene("Mech_Test_Scene");
    }
}
