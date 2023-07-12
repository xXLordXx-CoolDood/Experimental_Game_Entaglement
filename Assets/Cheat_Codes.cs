using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Cheat_Codes : MonoBehaviour
{
    public List<string> bigHead = new List<string>();
    public List<string> superIceSlide = new List<string>();

    public Transform turret;

    [SerializeField] private List<string> cheatCode = new List<string>(0);
    private bool gun1Neutral, gun2Neutral, gun3Neutral;

    void Update()
    {
        #region check for input
        if (ArduinoDevice.current.gun1up.isPressed && AllCodesNeutral())
        {
            cheatCode.Add("1U");
            CheckValidCode();
        }
        if (ArduinoDevice.current.gun1down.isPressed && AllCodesNeutral())
        {
            cheatCode.Add("1D");
            CheckValidCode();
        }
        if (ArduinoDevice.current.gun2up.isPressed && AllCodesNeutral())
        {
            cheatCode.Add("2U");
            CheckValidCode();
        }
        if (ArduinoDevice.current.gun2down.isPressed && AllCodesNeutral())
        {
            cheatCode.Add("2D");
            CheckValidCode();
        }
        if (ArduinoDevice.current.gun3up.isPressed && AllCodesNeutral())
        {
            cheatCode.Add("3U");
            CheckValidCode();
        }
        if (ArduinoDevice.current.gun3down  .isPressed && AllCodesNeutral())
        {
            cheatCode.Add("3D");
            CheckValidCode();
        }
        #endregion

        #region check for neutral switches
        if (!ArduinoDevice.current.gun1up.isPressed && !ArduinoDevice.current.gun1down.isPressed) { gun1Neutral = true; }
        else { gun1Neutral = false; }
        if (!ArduinoDevice.current.gun2up.isPressed && !ArduinoDevice.current.gun2down.isPressed) { gun2Neutral = true; }
        else { gun2Neutral = false; }
        if (!ArduinoDevice.current.gun3up.isPressed && !ArduinoDevice.current.gun3down.isPressed) { gun3Neutral = true; }
        else { gun3Neutral = false; }
        #endregion
    }

    private void CheckValidCode()
    {
        if(cheatCode[cheatCode.Count - 1] != bigHead[cheatCode.Count - 1] && cheatCode[cheatCode.Count - 1] != superIceSlide[cheatCode.Count - 1])
        {
            cheatCode.Clear();
        }
        else if (CheckForBigHead()) { turret.localScale = new Vector3(2, 2, 2); cheatCode.Clear(); }
        else if (CheckForIceSlide()) 
        { 
            transform.GetChild(0).GetComponent<Mech_Controller>().iceMultiplier = 7;
            transform.GetChild(0).GetComponent<Point_Getter>().saveScore = false;
            cheatCode.Clear(); 
        }
    }

    private bool CheckForBigHead()
    {
        if (cheatCode.Count == bigHead.Count) { return !bigHead.Except(cheatCode).Any(); }
        return false;
    }

    private bool CheckForIceSlide()
    {
        if(cheatCode.Count == superIceSlide.Count) { return !superIceSlide.Except(cheatCode).Any(); }
        return false;
    }

    private bool AllCodesNeutral()
    {
        if(gun1Neutral && gun2Neutral && gun3Neutral) { return true; }

        return false;
    }
}
