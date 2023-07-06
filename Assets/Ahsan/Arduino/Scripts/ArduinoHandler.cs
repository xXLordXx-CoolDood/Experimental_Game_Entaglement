using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System.Linq;
using Unity.VisualScripting;

public class ArduinoHandler : MonoBehaviour
{
    private static string outgoingMsg = "";
    private static string incomingMsg = "";
    private static SerialPort sp;
    Thread thread = new Thread(DataThread);


    private static void DataThread()
    {
        sp = new SerialPort("COM5", 9600);
        sp.Open();

        while (true)
        {
            if (outgoingMsg != "")
            {
                sp.Write(outgoingMsg);
                outgoingMsg = "";
            }

            incomingMsg = sp.ReadExisting();
            Thread.Sleep(200);
        }
    }

    private void OnDestroy()
    {
        // Closes the thread and serial port when the game ends
        thread.Abort();
        sp.Close();

        var arduinoDevice = InputSystem.devices.FirstOrDefault(x => x is ArduinoDevice);
        if (arduinoDevice != null)
        {
            InputSystem.RemoveDevice(arduinoDevice);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        thread.Start();

        InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "Arduino",
            product = "Mech"
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingMsg != "")
        {
            var arduinoDeviceState = new ArduinoDeviceState();

            int inputValue = 0;
            if (int.TryParse(incomingMsg, out inputValue))
            {
                Debug.Log(inputValue.ToBinaryString());
                arduinoDeviceState.buttons = inputValue;
            }

            InputSystem.QueueStateEvent(ArduinoDevice.current, arduinoDeviceState);
            incomingMsg = "";

        }
    }
}


