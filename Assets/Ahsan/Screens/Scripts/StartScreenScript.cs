using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreenScript : MonoBehaviour
{
    UIDocument uiDoc;
    float loadProg;
    // Start is called before the first frame update
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        

        var root = uiDoc.rootVisualElement;

        root.Q("Slider").visible = false;

        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMechScene()
    {

        StartCoroutine("ProgressLoad");

    }

    public IEnumerator ProgressLoad()
    {
        var root = uiDoc.rootVisualElement;
        
        root.Q("Slider").visible = true;
        List<VisualElement> bars = root.Q("Slider").Children().ToList();
        bars.ForEach(e => e.visible = false);
        
        for (int i = 1; i < bars.Count + 1; i++)
        {
            bars[i - 1].visible = true;
            yield return new WaitForSeconds(0.075f);
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Mech_Test_Scene");
    }
}
