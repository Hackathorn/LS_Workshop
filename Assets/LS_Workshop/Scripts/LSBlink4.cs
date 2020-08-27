using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSBlink4 : MonoBehaviour
{
    // public variables to be set in inspector
    public float onDuration = 2f; // amount of seconds renderer on
    public float offDuration = 2f; // amount of seconds render off
    public float totalDuration = 0f; // total seconds of blinking; zero => infinite

    // variable to hold a reference to Renderer component on this gameObject
    private MeshRenderer myRenderer;
 
    // variable to hold the amount of time that has passed
    private float timeElapsed;
    // switch whether the blinker is ON
    private bool blinkerOn = true;
    private float timeStart;

    // function to turn on the blinker externally
    public void turnOn()
    {
        blinkerOn = true;
        timeElapsed = 0f;
    }
 
    // this function is called once by Unity the moment the game starts
    private void Awake()
    {
        // get a reference to the MeshRenderer component
        myRenderer = this.GetComponent<MeshRenderer>();
        if (myRenderer == null) {
            Debug.Log("ERROR!!! No MeshRenderer for Blinker script");
            this.enabled = false; // disable script
        }
        turnOn(); // turn on blinker
     }
 
    // this function is called every frame by Unity
    private void Update()
    {
        // if we have a reference to the Light component
        if (blinkerOn)
        {
            // add the amount of time that has passed since last frame
            timeElapsed += Time.deltaTime;
            
            // exceeded totalDuration time? => turnoff blinking & enable renderer
            if (totalDuration != 0 || timeElapsed > totalDuration) 
            {
                blinkerOn = false;
                myRenderer.enabled = true;

                // if the amount of time passed is greater than or equal to the delay
                if(myRenderer.enabled && timeElapsed >= onDuration)
                {
                    // reset the time elapsed and turn off the renderer
                    timeElapsed = 0;
                    myRenderer.enabled = false;
                }
                else if (!myRenderer.enabled && timeElapsed >= offDuration) 
                {
                    timeElapsed = 0;
                    myRenderer.enabled = true;

                    // >>>>>>>>>>>>>>>> ALSO increase transform scale by +1 in LS
                    // GameObject _go = GameObject.Find("LSWorkshop");
                    // LSpaceController _scr = _go.GetComponent<LSpaceController>();
                    // transform.localScale = Vector3.one * _scr.PlotScale;
                }
            }
        }
    }
 }
