using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public struct ArduinoDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('A', 'R', 'D', 'N');

    [InputControl(name = "Button1", layout = "Button", bit = 0, displayName = "Button 1")]
    [InputControl(name = "Button2", layout = "Button", bit = 1, displayName = "Button 2")]
    [InputControl(name = "Button3", layout = "Button", bit = 2, displayName = "Button 3")]
    [InputControl(name = "Button4", layout = "Button", bit = 4, displayName = "Button 4")]
    [InputControl(name = "Button5", layout = "Button", bit = 5, displayName = "Button 5")]
    [InputControl(name = "Button6", layout = "Button", bit = 6, displayName = "Button 6")]
    [InputControl(name = "Button7", layout = "Button", bit = 7, displayName = "Button 7")]
    [InputControl(name = "Button8", layout = "Button", bit = 8, displayName = "Button 8")]
    [InputControl(name = "Button9", layout = "Button", bit = 9, displayName = "Button 9")]
    [InputControl(name = "Button10", layout = "Button", bit = 10, displayName = "Button 10")]
    [InputControl(name = "Button11", layout = "Button", bit = 11, displayName = "Button 11")]
    [InputControl(name = "Button12", layout = "Button", bit = 12, displayName = "Button 12")]
    [InputControl(name = "Button13", layout = "Button", bit = 13, displayName = "Button 13")]
    [InputControl(name = "Button14", layout = "Button", bit = 14, displayName = "Button 14")]
    [InputControl(name = "Button15", layout = "Button", bit = 15, displayName = "Button 15")]
    [InputControl(name = "Button16", layout = "Button", bit = 16, displayName = "Button 16")]
    [InputControl(name = "Button17", layout = "Button", bit = 17, displayName = "Button 17")]


    public int buttons;

}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(ArduinoDeviceState))]
public class ArduinoDevice : InputDevice
{
#if UNITY_EDITOR
    static ArduinoDevice()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.RegisterLayout<ArduinoDevice>(matches: new InputDeviceMatcher().WithInterface("Arduino"));
    }

    public ButtonControl firstButton { get; private set; }
    public ButtonControl secondButton { get;private set; }
    public ButtonControl thirdButton { get; private set; }
    public ButtonControl fourthButton { get; private set; }
    public ButtonControl fifthButton { get; private set; }
    public ButtonControl sixthButton { get; private set; }
    public ButtonControl seventhButton { get; private set; }
    public ButtonControl eighthButton { get; private set; }
    public ButtonControl ninthButton { get; private set; }
    public ButtonControl tenthButton { get; private set; }
    public ButtonControl eleventhButton { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        firstButton = GetChildControl<ButtonControl>("Button1");
        secondButton = GetChildControl<ButtonControl>("Button2");
        thirdButton    = GetChildControl<ButtonControl>("Button3");
        fourthButton   = GetChildControl<ButtonControl>("Button4");
        fifthButton    = GetChildControl<ButtonControl>("Button5");
        sixthButton    = GetChildControl<ButtonControl>("Button6");
        seventhButton  = GetChildControl<ButtonControl>("Button7");
        eighthButton   = GetChildControl<ButtonControl>("Button8");
        ninthButton    = GetChildControl<ButtonControl>("Button9");
        tenthButton    = GetChildControl<ButtonControl>("Button10");
        eleventhButton = GetChildControl<ButtonControl>("Button11");
    }

    public static ArduinoDevice current { get; private set; }
    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
        {
            current = null;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Arduino Device/Create Device")]

    private static void CreateDevice()
    {
        InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "Arduino",
            product = "Mech"
        });
    }

    [MenuItem("Tools/Arduino Device/Remove Device")]
    private static void RemoveDevice() 
    { 
        var arduinoDevice = InputSystem.devices.FirstOrDefault(x => x is ArduinoDevice);
        if (arduinoDevice != null)
        {
            InputSystem.RemoveDevice(arduinoDevice);
        }
    }
#endif

}
