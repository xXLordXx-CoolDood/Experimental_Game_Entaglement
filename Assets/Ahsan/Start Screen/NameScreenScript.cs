using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NameScreenScript: MonoBehaviour
{
    UIDocument uiDoc;
    // Start is called before the first frame update
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        List<char> chars = new List<char>();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            chars.Add(c);
        }
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>  (e as Label).text = chars[i].ToString();

        var root = uiDoc.rootVisualElement;

        root.Query().Where(elem => (elem as ListView) != null).ForEach(elem =>
        {
            (elem as ListView).makeItem = makeItem;
            (elem as ListView).bindItem = bindItem;
            (elem as ListView).itemsSource = chars;
            (elem as ListView).selectionType = SelectionType.Single;

        });
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
