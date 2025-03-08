using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceDetection : MonoBehaviour
{
    public List<InputDevice> CurrentDevices = new List<InputDevice>();

    private void Update()
    {
        foreach(var g in Gamepad.all)
        {
            if (g.wasUpdatedThisFrame && !CurrentDevices.Contains(g.device))
            {
                CurrentDevices.Add(g.device);
                Debug.Log("new device - controller");
            }
        }

        if (Keyboard.current.anyKey.isPressed && !CurrentDevices.Contains(Keyboard.current.device))
        {
            CurrentDevices.Add(Keyboard.current.device);
            Debug.Log("new device - keyboard");
        }
    }
}
