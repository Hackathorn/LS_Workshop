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
    public float maxTime;
    public bool isSnapRot = true;

    private float totalTime;
    private bool isLerping = false; 
    // private int currentFrame;
    private Vector3 deltaPos, startPos, endPos;
    private float deltaAngle, startAngle, endAngle;
    private Transform goCamera, goAxes; 
    private Vector3[] axesPos = new [] 
        { new Vector3(0f,0f,+1f), new Vector3(+1f,0f,0f), new Vector3(0f,0f,-1f), new Vector3(-1f,0f,0f)  };

    void Start() {
        LSpaceController.onPlotChange += RefreshAxes;

        goCamera = transform.Find("OVRCameraRig");
        if (goCamera == null) Debug.Log("ERROR: Can not find OVRCameraRig gameobject");
        goAxes = transform.Find("LSCurrentAxes");
        if (goAxes == null) Debug.Log("ERROR: Can not find LSCurrentAxes gameobject");

        if (maxTime == 0f) maxTime = 0.5f;  // just-in-case it is not set in inspector
    }
    // Remove listener from onPlotChange when point is destroyed
    private void OnDisable() {
        LSpaceController.onPlotChange -= RefreshAxes;
    }


    void Update()
    {
        if (!isLerping) {
            deltaPos = Vector3.zero; // initialize to zero delta position
            deltaAngle = 0f;         // initialize to zero delta rotation

            // return platform to zero axes
            if(OVRInput.GetDown(OVRInput.Button.Three)) {   
                deltaPos = -1f * transform.position + Vector3.left;  } //????????? why Vector3.left?

            // move platform forward-backward-up-down-left-right by one unit worldspace
            // ...assuming that OVRCamera is facing +Z direction 
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)) { 
                deltaPos = goCamera.forward; }  // forward 
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)) { 
                deltaPos = -goCamera.forward; }  // backward 
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) { 
                deltaPos = -goCamera.right; }  // left 
            if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) { 
                deltaPos = goCamera.right ; }  // right 
            if(OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp)) { 
                deltaPos = goCamera.up ; }  // up 
            if(OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown)) { 
                deltaPos = -goCamera.up ; }  // down 

            if (deltaPos != Vector3.zero) {  //got a delta position, so start LERPing cycles
                isLerping = true;
                totalTime = 0f; 
                startPos = transform.position;
                endPos = startPos + deltaPos; 
                // Debug.Log("LERPing Pos started.... " + deltaPos + " startPos= " + startPos + " endPos= " + endPos);
            }

            // snap rotate player (OVRCameraRig) by 90 deg to right
            else if(OVRInput.GetDown(OVRInput.Button.One)) 
            {   
                deltaAngle = 90f;   // look right
                isLerping = true;
                totalTime = 0f; 
                startAngle = Mathf.Round(goCamera.transform.rotation.eulerAngles.y);
                endAngle = startAngle + deltaAngle; 
                if (endAngle >= 360f) endAngle = 0f;
                if (isSnapRot) {  // do camera rotation now
                    goCamera.transform.rotation = Quaternion.Euler(0, endAngle, 0);
                    isLerping = false;  // not lerping needed
                }

                // move platform zero axes to proper side
                int i = (int) (endAngle / 90f);
                goAxes.transform.localPosition = axesPos[i];
                // Debug.Log("LERPing Rot started.... " + "startAngle= " + startAngle + " endAngle= " + endAngle + " i= " + i);
            }
        }
        else   // continuing the LERPing cycle
        {  
            totalTime += Time.deltaTime; 
            float pctComplete = totalTime / maxTime;

            if (deltaAngle == 0f) {  // Pos Lerp
                this.transform.position = Vector3.Lerp(startPos, endPos, pctComplete);
            } 
            else {                   // Rot Lerp
                float angle = Mathf.LerpAngle(startAngle, endAngle, pctComplete);
                goCamera.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            if (pctComplete > 1f) {  // done lerping?
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

    // Refresh platform axes by getting current plot parms and setting scale
    private void RefreshAxes()
    {
        // find public parameters in LSpaceController
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();
        // Debug.Log("REFRESH AXES PlotScale = " + _scr.PlotScale);
        LSZeroAxes.transform.localScale = Vector3.one * _scr.PlotScale / 10f;
    }

    // Rotate platform by +90 or -90 on Y axis and reposition
    private int quadrant;  // 0 is 90 deg +X  1 is 0 deg for +Z  2 is 270 deg for -X  3 is 180 deg for -Z
    private float[] degrees = new float[] {90f, 0f, 270f, 180f};


    private void RotatePlayer()
    {
        // rotate 90 deg to the right, as... +Z, -X, -Z, and back to +X
        // 0 is 90 deg +X  1 is 0 deg for +Z  2 is 270 deg for -X  3 is 180 deg for -Z
        quadrant++; 
        if (quadrant > 3) quadrant = 0; // check for new cycle

        // rotate OVRCameraRig
        Transform _go = transform.Find("OVRCameraRig"); 
        _go.transform.rotation = Quaternion.Euler(0f, degrees[quadrant], 0f); 

        // rotate CanvasWithDebug >>>>>>>>>>>>>>>>>>>>>>>TBC reposition panel in front of player
        // _go = transform.Find("CanvasWithDebug"); 
        // _go.transform.rotation = Quaternion.Euler(0f, degrees[quadrant], 0f); 

        // float smooth = 5.0f;
        // Quaternion target = Quaternion.Euler(0, degrees[quadrant], 0);
        // _go.transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);
    }

}