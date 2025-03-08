using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceDetection : MonoBehaviour
{
    public List<InputDevice> CurrentDevices = new List<InputDevice>();
    public List<Movement> unboundMovements = new List<Movement>();
    

    private void Update()
    {
        if (unboundMovements.Count == 0) return;
        
        foreach(var gamepad in Gamepad.all)
        {
            if (gamepad.wasUpdatedThisFrame && !CurrentDevices.Contains(gamepad.device))
            {
                CurrentDevices.Add(gamepad.device);
                //unboundMovements[0].BindDevice(gamepad.device);
                unboundMovements.RemoveAt(0);
                Debug.Log("new device - controller");
            }
        }

        foreach (var joystick in Joystick.all)
        {
            if (joystick.wasUpdatedThisFrame && !CurrentDevices.Contains(joystick.device))
            {
                CurrentDevices.Add(joystick.device);
                //unboundMovements[0].BindDevice(joystick.device);
                unboundMovements.RemoveAt(0);
                Debug.Log("new device - joystick");
            }
        }

        if (Keyboard.current.anyKey.isPressed && !CurrentDevices.Contains(Keyboard.current.device))
        {
            CurrentDevices.Add(Keyboard.current.device);
            //unboundMovements[0].BindDevice(Keyboard.current.device);
            unboundMovements.RemoveAt(0);
            Debug.Log("new device - keyboard");
        }
    }
}
