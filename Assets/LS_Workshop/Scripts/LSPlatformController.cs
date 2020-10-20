using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSPlatformController : MonoBehaviour
{
    public GameObject LSZeroAxes;
    public GameObject XPosText;
    public GameObject YPosText;
    public GameObject ZPosText;
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
    private Text XPosTextUI, YPosTextUI, ZPosTextUI;    
    private Transform PosTextCanvas;

    /**************************************************************************/
    // START -- subscribe to onPlotChange, plus setup transform links to Camera & Axes
    void Start() {
        LSpaceController.onPlotChange += RefreshAxes;

        goCamera = transform.Find("OVRCameraRig");
        if (goCamera == null) Debug.Log("ERROR: Can not find OVRCameraRig gameobject");
        goAxes = transform.Find("LSCurrentAxes");
        if (goAxes == null) Debug.Log("ERROR: Can not find LSCurrentAxes gameobject");

        XPosTextUI = XPosText.GetComponent<Text>();
        YPosTextUI = YPosText.GetComponent<Text>();
        ZPosTextUI = ZPosText.GetComponent<Text>();
        PosTextCanvas = XPosText.GetComponentInParent<Transform>(); // get canvas to flip Y rot

        if (maxTime == 0f) maxTime = 0.5f;  // just-in-case it is not set in inspector
    }
    // Remove listener from onPlotChange when point is destroyed
    private void OnDisable() {
        LSpaceController.onPlotChange -= RefreshAxes;
    }

    /**************************************************************************/
    // UPDATE -- check for controller move and lerp, plus check for player rotation 
    void Update()
    {
        if (!isLerping) {    // NO lerping so check for controller input
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

                if (isSnapRot) {        // do camera rotation instantly
                    goCamera.transform.rotation = Quaternion.Euler(0, endAngle, 0);
                    isLerping = false;  // no lerping needed
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
            else {                   // Rot Lerp (if not SNAP rotation)
                float angle = Mathf.LerpAngle(startAngle, endAngle, pctComplete);
                goCamera.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            if (pctComplete > 1f) {  // done lerping?
                isLerping = false;
                }
        }
        // Update the position data in the X-Y-Z text fields
        string format = "+0.0000;-0.0000";
        Vector3 pos = this.transform.position;
        XPosTextUI.text = "X  " + (pos.x  / LSpaceController.PlotScale).ToString(format); 
        YPosTextUI.text = "Y  " + (pos.y  / LSpaceController.PlotScale).ToString(format);
        ZPosTextUI.text = "Z  " + (pos.z  / LSpaceController.PlotScale).ToString(format);
        // also rotate ZeroCanvas toward player
        XPosText.transform.parent.transform.rotation = Quaternion.Euler(0, endAngle, 0);

    }
    /*************************************************************************/
    // when onPlotChange, then reset scale for platform axes 
    private void RefreshAxes()
    {
        // find public parameters in LSpaceController
        // GameObject _go = GameObject.Find("LSWorkshop");
        // LSpaceController _scr = _go.GetComponent<LSpaceController>();
        // Debug.Log("REFRESH AXES PlotScale = " + _scr.PlotScale);

        LSZeroAxes.transform.localScale = Vector3.one * LSpaceController.PlotScale / 10f;
    }
}