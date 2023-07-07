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

    [InputControl(name = "Button1", layout = "Button", bit = 0, displayName = "Shoot Left")]
    [InputControl(name = "Button2", layout = "Button", bit = 1, displayName = "Switch Cam")]
    [InputControl(name = "Button3", layout = "Button", bit = 2, displayName = "Gun1Down")]
    [InputControl(name = "Button4", layout = "Button", bit = 3, displayName = "Gun1Up")]
    [InputControl(name = "Button5", layout = "Button", bit = 4, displayName = "Gun2Down")]
    [InputControl(name = "Button6", layout = "Button", bit = 5, displayName = "Gun2Up")]
    [InputControl(name = "Button7", layout = "Button", bit = 6, displayName = "Gun3Down")]
    [InputControl(name = "Button8", layout = "Button", bit = 7, displayName = "Gun3Up")]
    [InputControl(name = "Button9", layout = "Button", bit = 8, displayName = "Forward")]
    [InputControl(name = "Button10", layout = "Button", bit = 9, displayName = "Reverse")]
    [InputControl(name = "Button11", layout = "Button", bit = 10, displayName = "Shoot Right")]
    [InputControl(name = "Button12", layout = "Button", bit = 11, displayName = "Front Left Leg")]
    [InputControl(name = "Button13", layout = "Button", bit = 12, displayName = "Back Right Leg")]
    [InputControl(name = "Button14", layout = "Button", bit = 13, displayName = "Back Left Leg")]
    [InputControl(name = "Button15", layout = "Button", bit = 14, displayName = "Front Right Leg")]
    [InputControl(name = "Button16", layout = "Button", bit = 15, displayName = "Turn Left")]
    [InputControl(name = "Button17", layout = "Button", bit = 16, displayName = "Turn Right")]


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

    public ButtonControl shootLeft { get; private set; }
    public ButtonControl cameraSwitch { get;private set; }
    public ButtonControl gun1up { get; private set; }
    public ButtonControl gun1down { get; private set; }
    public ButtonControl gun2up { get; private set; }
    public ButtonControl gun2down { get; private set; }
    public ButtonControl gun3up { get; private set; }
    public ButtonControl gun3down { get; private set; }
    public ButtonControl forward { get; private set; }
    public ButtonControl reverse { get; private set; }
    public ButtonControl shootRight { get; private set; }
    public ButtonControl frontLeft { get; private set; }
    public ButtonControl backRight { get; private set; }
    public ButtonControl backLeft { get; private set; }
    public ButtonControl frontRight { get; private set; }
    public ButtonControl turnLeft { get; private set; }
    public ButtonControl turnRight { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        shootLeft = GetChildControl<ButtonControl>("Button1");
        cameraSwitch = GetChildControl<ButtonControl>("Button2");
        gun1down    = GetChildControl<ButtonControl>("Button3");
        gun1up   = GetChildControl<ButtonControl>("Button4");
        gun2down    = GetChildControl<ButtonControl>("Button5");
        gun2up    = GetChildControl<ButtonControl>("Button6");
        gun3down  = GetChildControl<ButtonControl>("Button7");
        gun3up   = GetChildControl<ButtonControl>("Button8");
        forward    = GetChildControl<ButtonControl>("Button9");
        reverse    = GetChildControl<ButtonControl>("Button10");
        shootRight = GetChildControl<ButtonControl>("Button11");
        frontLeft = GetChildControl<ButtonControl>("Button12");
        backRight = GetChildControl<ButtonControl>("Button13");
        backLeft = GetChildControl<ButtonControl>("Button14");
        frontRight = GetChildControl<ButtonControl>("Button15");
        turnLeft = GetChildControl<ButtonControl>("Button16");
        turnRight = GetChildControl<ButtonControl>("Button17");
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
