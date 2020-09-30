using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSPlatformController : MonoBehaviour
{
    public GameObject LSZeroAxes;
    public Text XTextValue;
    public Text YTextValue;
    public Text ZTextValue;

    //ref: https://developer.oculus.com/documentation/unity/unity-ovrinput/
    //ref: https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
    //ref: 
    public int totalFrames = 20;
    [Tooltip("Number of frames to complete platform movement")]
    // public float duration = 1.0f;
    private bool isLerping = false; 
    private int currentFrame;
    private Vector3 startPos, endPos;

    void Start() {
        LSpaceController.onPlotChange += RefreshAxes;
    }
    // Remove listener from onPlotChange when point is destroyed
    private void OnDisable() {
        LSpaceController.onPlotChange -= RefreshAxes;
    }

    void Update()
    {
        if (!isLerping) {
            // Debug.Log("Input???? Pos = " + transform.position);
            Vector3 newDelta = Vector3.zero; 

            if(OVRInput.GetDown(OVRInput.Button.Three)) {           // return to Zero Axes
                newDelta = -1f * transform.position + Vector3.left;
            }
            // if(OVRInput.GetDown(OVRInput.Button.Three)) {    // Button X
            //     Debug.Log("BUTTON Three +1");
            //     newDelta += new Vector3(0,+1,0); }
            // if(OVRInput.GetDown(OVRInput.Button.Four)) {     // Button Y
            //     Debug.Log("BUTTON Four -1");
            //     newDelta += new Vector3(0,-1,0); }

            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)) { 
                newDelta += new Vector3(+1,0,0); }
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)) { 
                newDelta += new Vector3(-1,0,0); }
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) { 
                newDelta += new Vector3(0,0,+1); }
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) { 
                newDelta += new Vector3(0,0,-1); }
            if(OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp)) { 
                newDelta += new Vector3(0,+1,0); }
            if(OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown)) { 
                newDelta += new Vector3(0,-1,0); }

            // Vector2 input2D = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            // if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick)) {  // thumbstick pressed => Y-axis Up/Down 
            //     if (input2D.x > 0.5f) {
            //         newDelta += new Vector3(0,+1,0); }
            //     if (input2D.x < 0.5f) {
            //         newDelta += new Vector3(0,-1,0); }
            // }
            // else {                                                 // X+Z axises => Forward/Back/Left/Right
            //     if (input2D.x > 0.5f) {
            //         newDelta += new Vector3(+1,0,0); }  // forward +X
            //     if (input2D.x < 0.5f) {
            //         newDelta += new Vector3(-1,0,0); }  // back -X
            //     if (input2D.y > 0.5f) {
            //         newDelta += new Vector3(0,0,+1); }  // left +Z
            //     if (input2D.y < 0.5f) {
            //         newDelta += new Vector3(0,0,-1); }  // right -Z
            // }
            
            if (newDelta != Vector3.zero) {     //start LERPing cycles
                // Debug.Log("LERPing started " + newDelta);
                isLerping = true;
                currentFrame = 0;
                startPos = transform.position;
                endPos = startPos + newDelta;
            }
        }
        else {  // within LERPing cycle
            currentFrame += 1;
            float pctComplete = (float)currentFrame / (float)totalFrames;
            this.transform.position = Vector3.Lerp(startPos, endPos, pctComplete);
            // Debug.Log("LERP cycle " + currentFrame + " with " + transform.position);
            if (currentFrame >= totalFrames) {
                isLerping = false;
                }
            }
        
        // update X-Y-Z position texts 
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();
        float _PlotScale = _scr.PlotScale;

        Vector3 pos = transform.position;
        XTextValue.text = "X  " + ((pos.x + 1) / _PlotScale).ToString("F5");  // adjust X to front of platform
        YTextValue.text = "Y  " + (pos.y  / _PlotScale).ToString("F5");
        ZTextValue.text = "Z  " + (pos.z  / _PlotScale).ToString("F5");

    }

    // Refresh this point by getting current plot parms and setting pos & scale
    private void RefreshAxes()
    {
        // find public parameters in LSpaceController
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();
        // Debug.Log("REFRESH AXES PlotScale = " + _scr.PlotScale);
        LSZeroAxes.transform.localScale = Vector3.one * _scr.PlotScale / 10f;
    }

}
