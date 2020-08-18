using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSTestForQuest : MonoBehaviour
{
    private bool QuestPresent;
    // Start is called before the first frame update
    void Start()
    {
        //DEBUG ref: https://docs.unity3d.com/Manual/xr_input.html
        //ref: https://docs.unity3d.com/Packages/com.unity.xr.oculus@1.4/manual/index.html for Android build

        QuestPresent = false;
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        foreach (var device in inputDevices) {
            if (device.name == "Quest") {
                QuestPresent = true;
            }
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.characteristics.ToString()));
        }
         Debug.Log("Quest present? " + QuestPresent);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
