using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSPlatformTracker : MonoBehaviour
{
    public GameObject PlayerController;

    void Update()
    {
        
        Vector3 myPosition = PlayerController.transform.position;
        transform.position = myPosition - Vector3.up; 
    }
}
