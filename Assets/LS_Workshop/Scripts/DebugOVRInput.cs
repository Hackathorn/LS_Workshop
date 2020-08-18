using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOVRInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One)) {
            Debug.Log("INPUT: Button One down"); 
        }

        if(OVRInput.GetDown(OVRInput.Button.Two)) {
            Debug.Log("INPUT: Button Two down"); 
        }

        Debug.Log("INPUT: Left Thumbstick value is " + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));

    }
}
