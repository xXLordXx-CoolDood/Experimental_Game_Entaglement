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
        

        List<string> testData = new List<string>() { 
            "Kat & Noa",
            "Xav & Ahs",
            "Oli & Ahs",
            "Noa & Kat",
            "Xav & Noa"
        };
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>  (e as Label).text = (i+1).ToString() + ". " + testData[i];

        var root = uiDoc.rootVisualElement;

        root.Q<ListView>().makeItem = makeItem;
        root.Q<ListView>().bindItem = bindItem;

        root.Q<ListView>().itemsSource = testData;
        root.Q<ListView>().selectionType = SelectionType.None;
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
