using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LSpacesProject; 
public class LSPointController : MonoBehaviour
{
    // LSpaceController script in prefab LSpaceWorkshop

    // public variables below are set in LSpaceController script when this point is instantiated
    public GameObject ImagePrefab;
    public string _LSPointName;
    public float[] _LSPointPos;
    public float[] _LSPointStd;
    public Sprite _LSPointSprite;
    public Texture2D _LSPointTexture;
    public int _DimSize;
    public string _pointClusterName;
    public int _pointClusterCatergory;
    public string _pointClusterLabel;

//
    private bool _LSPointSelected = false;
    public bool LSPointSelectionToggle () {
        _LSPointSelected = !_LSPointSelected;
        GetComponent<LSBlink>().enabled = _LSPointSelected;
        // Debug.Log("HIT - point " + this.name + " un/selected as " + _LSPointSelected); 
        return _LSPointSelected;
    }

    // private variables below are set in RefreshPoints below during Update
    private float _PlotScale;
    private bool _isBall;
    private bool _isNewYCompared;
    private bool _isImageShown;
    private int _BaseX;
    private int _BaseZ;
    private int _VertY;
    private int _newY;
    private bool _Variance; 

    // Register listener for onPlotChange and do initial RefreshPoints
    void Start() 
    {
        // subscribe to events
        LSpaceController.onPlotChange += RefreshPoints;
        LSpaceController.onClusterChange += RefreshCluster;

        RefreshPoints();
    }

    private void Update() 
    {
        AnimateNewY();
    }

    // Remove listener from onPlotChange when point is destroyed
    private void OnDisable() 
    {
        // UNsubscribe to events
        LSpaceController.onPlotChange -= RefreshPoints;
        LSpaceController.onClusterChange -= RefreshCluster;
    }

    // Refresh this point by getting current plot parms and setting pos & scale
    private void RefreshPoints()
    {
        // find public parameters in LSpaceController
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();

        // set _ parameters for this point
        _PlotScale = _scr.PlotScale;
        _isBall = _scr.isBall;
        _isNewYCompared = _scr.isNewYCompared;
        _isImageShown = _scr.isImageShown;
        _BaseX = _scr.baseX;
        _VertY = _scr.vertY;
        _BaseZ = _scr.baseZ;
        _newY = _scr.newY;
        _Variance = _scr.Variance;

        // calculate new position on this point
        float xPos = Convert.ToSingle(_LSPointPos[_BaseX]);
        float yPos = Convert.ToSingle(_LSPointPos[_VertY]);
        float zPos = Convert.ToSingle(_LSPointPos[_BaseZ]);

        transform.localPosition = new Vector3 (xPos, yPos, zPos) * _PlotScale; //rescale to WorldSpace

        // calculate new scale for this point
        float xStd = Convert.ToSingle(_LSPointStd[_BaseX]) * _PlotScale; //rescale to WorldSpace
        float yStd = Convert.ToSingle(_LSPointStd[_VertY]) * _PlotScale;
        float zStd = Convert.ToSingle(_LSPointStd[_BaseZ]) * _PlotScale;
        transform.localScale = new Vector3 (xStd, yStd, zStd) * 0.25f; // TBC >>>>>>>>>>>>>>>>> FUDGE FACTOR for looks!!!

        // Sets color according to x/y/z value
        GetComponent<Renderer>().material.color = new Color(xPos, yPos, zPos, 1.0f);
        // Activate emission color keyword so we can modify emission color
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(xPos, yPos, zPos, 1.0f));

        // activate and set image into point sprite
        if (_isImageShown) {
            ImagePrefab.SetActive(true);
            SpriteRenderer _scr2 = ImagePrefab.GetComponent<SpriteRenderer>();
            _scr2.sprite = _LSPointSprite;
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> TBC adjust image scale based on parent
            // like.......... ImagePrefab.localScale /= this.scale;
        }
        else {
            ImagePrefab.SetActive(false);
        }
    }

    //ref: https://developer.oculus.com/documentation/unity/unity-ovrinput/
    //ref: https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
    [Tooltip("Number of frames to complete newY movement")]
    public int totalFrames = 200;
    private bool isLerping = false; 
    private int currentFrame;
    private Vector3 startPos, endPos, newDelta;
    private void AnimateNewY()
    {
        if (_isNewYCompared) 
        {
            if (!isLerping) {           //start yet another LERPing cycle
                isLerping = true;
                currentFrame = 0;
                startPos = transform.position;
                newDelta.y = Convert.ToSingle(_LSPointPos[_newY]) * _PlotScale;
                endPos = startPos + newDelta;
            }
            // else continuing with Lerping
            //    if at Lerping end, then return to original position and isLerping = false
            else ContinueLerping();
        }
        else if (isLerping)   // if isLerping, then stop and return to vertY
        {
            ContinueLerping();
        }

        void ContinueLerping()
        {
            currentFrame += 1;
            float pctComplete = (float)currentFrame / (float)totalFrames;
            this.transform.position = Vector3.Lerp(startPos, endPos, pctComplete);
            // Debug.Log("LERP cycle " + currentFrame + " with " + transform.position);
            if (currentFrame >= totalFrames) {
                isLerping = false;
                this.transform.position = startPos;
                }
        }
    }

    private void RefreshCluster()
    {

    }

}
