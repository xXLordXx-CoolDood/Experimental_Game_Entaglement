using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Mesh;
using static UnityEngine.Rendering.DebugUI;

public class NameScreenScript : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement root;

    List<VisualElement> leftName;
    List<VisualElement> rightName;
    ListView highScoreList;

    int leftSlot, rightSlot;
    int leftChar, rightChar;
    float leftScroll, rightScroll;


    bool leftFinalized = false, rightFinalized = false;
    bool listUpdated = false;
    string p1Name = "", p2Name = "";

    List<char> chars = new List<char>();

    int points;
    List<SaveData> saveData;
    SaveData thisLevelData = new SaveData { p1Name = "___", p2Name = "___" };


    // Start is called before the first frame update
    void Start()
    {
        points = PlayerPrefs.GetInt("points");
        thisLevelData.score = points;

        SaveLoadScript.Init();

        InitializeCharacterList();

        // Set the UI Components to be accessed
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        NameInputInitialize();
        HighScoreInputInitialize();
    }

    private void HighScoreInputInitialize()
    {
        if (SaveLoadScript.Load() != null)
        {
            SaveData[] testData = JsonHelper.FromJson<SaveData>(SaveLoadScript.Load());
            if (testData == null)
            {
                saveData = new List<SaveData>();
            }
            else
            {
                saveData = testData.ToList();
            }
        }
        else
        {
            saveData = new List<SaveData>();
        }
        Func<VisualElement> makeItem = () =>
        {
            var v = new VisualElement();
            v.style.flexDirection = FlexDirection.Row;
            v.style.flexGrow = 1;
            v.style.justifyContent = Justify.SpaceBetween;
            
            Label name = new Label();
            Label score = new Label();
            v.Add(name);
            v.Add(score);

            return v;
        };

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            var labels = e.Query<Label>().ToList();
            labels[0].text = saveData[i].p1Name + " & " + saveData[i].p2Name;
            labels[1].text = saveData[i].score.ToString(); 
        };

        saveData.Add(thisLevelData);
        saveData.Sort((x, y) => { return (y.score.CompareTo(x.score)); });

        highScoreList = root.Query<ListView>();
        highScoreList.makeItem = makeItem;
        highScoreList.bindItem = bindItem;
        highScoreList.itemsSource = saveData;
        highScoreList.selectionType = SelectionType.None;


    }

    private void NameInputInitialize()
    {
        leftName = root.Query("LeftName").Children<VisualElement>().Where(x => x.name.Contains("slot")).ToList();
        rightName = root.Query("RightName").Children<VisualElement>().Where(x => x.name.Contains("slot")).ToList();


        // Blank out the characters
        leftName.ForEach(x => x.Query("selectedCharacter").ForEach(x => (x as Label).text = ""));
        rightName.ForEach(x => x.Query("selectedCharacter").ForEach(x => (x as Label).text = ""));

        // Enable the first slots of each name
        SelectSlot(leftName, leftSlot);
        SelectSlot(rightName, rightSlot);

        SetChar(leftName[leftSlot], leftChar);
        SetChar(rightName[rightSlot], rightChar);

        StartCoroutine("ScrollLetters");
    }

    private void Update()
    {

    }

    public IEnumerator ScrollLetters()
    {
        while (!leftFinalized || !rightFinalized)
        {
            Scroll(leftScroll, leftName, ref leftSlot, ref leftChar);
            Scroll(rightScroll, rightName, ref rightSlot, ref rightChar);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SetChar(VisualElement slot, int charValue)
    {
        slot.Query<Label>("scrollUp").First().text = chars[(charValue + chars.Count - 1) % chars.Count].ToString();
        slot.Query<Label>("selectedCharacter").First().text = chars[charValue].ToString();
        slot.Query<Label>("scrollDown").First().text = chars[(charValue + 1) % chars.Count].ToString();
    }

    void SelectSlot(List<VisualElement> slot, int select)
    {
        slot.ForEach(x =>
        {
            x.visible = false;
            x.Query("selectedCharacter").First().visible = true;
        });
        slot[select].visible = true;
    }

    private void InitializeCharacterList()
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            chars.Add(c);
        }
    }

    private void FinalizeName(List<VisualElement> Name, ref int currentSlot, ref bool NameFinalized)
    {
        Name[currentSlot].visible = false;
        Name[currentSlot].Query("selectedCharacter").First().visible = true;
        NameFinalized = true;
    }

    private void MoveRight(List<VisualElement> Name, ref int currentSlot, ref int currentChar)
    {
        currentSlot++;
        SelectSlot(Name, currentSlot);
        currentChar = 0;
        SetChar(Name[currentSlot], currentChar);
    }

    private void MoveLeft(List<VisualElement> Name, ref int currentSlot, ref int currentChar)
    {
        Name[currentSlot].Query("selectedCharacter").ForEach(x => (x as Label).text = "");
        currentSlot--;
        SelectSlot(Name, currentSlot);
        currentChar = chars.IndexOf(Name[currentSlot].Query<Label>("selectedCharacter").First().text[0]);
        SetChar(Name[currentSlot], currentChar);
        return;
    }

    private void Scroll(float dir, List<VisualElement> Name, ref int currentSlot, ref int currentChar)
    {
        if (dir > 0f)
        {
            currentChar = (currentChar + 1) % chars.Count;
        }

        if (dir < 0f)
        {
            currentChar = (currentChar + chars.Count - 1) % chars.Count;

        }
        SetChar(Name[currentSlot], currentChar);
    }

    public void SaveNames()
    {
        leftName.ForEach(x => p1Name += x.Query<Label>("selectedCharacter").First().text);
        rightName.ForEach(x => p2Name += x.Query<Label>("selectedCharacter").First().text);

        thisLevelData.p1Name = p1Name;
        thisLevelData.p2Name = p2Name;

        highScoreList.Rebuild();

    }

    public void MoveLeftInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && leftSlot > 0)
        {
            MoveLeft(leftName, ref leftSlot, ref leftChar);
            return;

        }
    }

    public void MoveRightInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && rightSlot > 0)
        {
            MoveLeft(rightName, ref rightSlot, ref rightChar);
            return;
        }
    }

    public void EnterLeftCharacter(InputAction.CallbackContext ctx)
    {
        if (leftFinalized) { return; }
        if (ctx.performed)
        {

            if (leftSlot < leftName.Count - 1 && leftName[leftSlot].Query<Label>("selectedCharacter").First().text != "<")
            {
                MoveRight(leftName, ref leftSlot, ref leftChar);
                return;
            }

            if (leftSlot == leftName.Count - 1)
            {
                FinalizeName(leftName, ref leftSlot, ref leftFinalized);
            }

        }
    }

    public void EnterRightCharacter(InputAction.CallbackContext ctx)
    {
        if (rightFinalized) { return; }
        if (ctx.performed)
        {

            if (rightSlot < rightName.Count - 1 && rightName[rightSlot].Query<Label>("selectedCharacter").First().text != "<")
            {
                MoveRight(rightName, ref rightSlot, ref rightChar);
                return;
            }

            if (rightSlot == rightName.Count - 1)
            {
                FinalizeName(rightName, ref rightSlot, ref rightFinalized);
            }

        }
    }


    public void ScrollLeft(InputAction.CallbackContext ctx)
    {
        if (leftFinalized) { return; }
        leftScroll = ctx.ReadValue<float>() * -1;
    }

    public void ScrollRight(InputAction.CallbackContext ctx)
    {
        if (rightFinalized) { return; }
        rightScroll = ctx.ReadValue<float>() * -1;
    }

    public void LogMission(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (listUpdated)
        {
            SaveLoadScript.Save(JsonHelper.ToJson<SaveData>(saveData.ToArray()));
            SceneManager.LoadScene("StartScreenUI");
            return;
        }

        if (leftFinalized && rightFinalized)
        {
            SaveNames();
            listUpdated = true;
            Debug.Log(saveData.Count);
            root.Q<Label>("startText").style.fontSize = 48;
            root.Q<Label>("startText").text = "Transfer To"+"\n"+"Next Pilots";

        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
