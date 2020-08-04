using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSPlatformTracker : MonoBehaviour
{
    public GameObject PlayerController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 myPosition = PlayerController.transform.position;
        transform.position = myPosition - Vector3.up; 
    }
}
