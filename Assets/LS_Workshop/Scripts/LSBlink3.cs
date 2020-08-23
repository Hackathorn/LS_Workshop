using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSBlink3 : MonoBehaviour
{
    // making these public will show them in the inspector (set the values there)
    public float onDuration = 2f; // amount of seconds renderer on
    public float offDuration = 2f; // amount of seconds render off

    // variable to hold a reference to the Light component on this gameObject
    private MeshRenderer myRenderer;
 
    // variable to hold the amount of time that has passed
    private float timeElapsed;
 
    // this function is called once by Unity the moment the game starts
    private void Awake()
    {
        // get a reference to the MeshRenderer component
        myRenderer = GetComponent<MeshRenderer>();
     }
 
    // this function is called every frame by Unity
    private void Update()
    {
        // if we have a reference to the Light component
        if(myRenderer != null)
        {
            // add the amount of time that has passed since last frame
            timeElapsed += Time.deltaTime;
 
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
                GameObject _go = GameObject.Find("LSWorkshop");
                LSpaceController _scr = _go.GetComponent<LSpaceController>();
                transform.localScale = Vector3.one * _scr.PlotScale;

                myRenderer.enabled = true;
            }
        }
    }
 }
