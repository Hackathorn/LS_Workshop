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
    public int _DimSize;

    // private variables below are set in RefreshPoints below during Update
    private float _PlotScale;
    private float _PointScale;
    private int _BaseX;
    private int _BaseZ;
    private int _VertY;
    private bool _Variance; 


    // Register listener for onPlotChange and do initial RefreshPoints
    void Start() 
    {
        LSpaceController.onPlotChange += RefreshPoints;
        RefreshPoints();
    }

    // Remove listener from onPlotChange when point is destroyed

    private void OnDisable() {
        LSpaceController.onPlotChange -= RefreshPoints;
    }

    // Refresh this point by getting current plot parms and setting pos & scale

    private void RefreshPoints()
    {
        // find public parameters in LSpaceController
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();

        // set _ parameters for this point
        _PlotScale = _scr.PlotScale;
        Debug.Log("REFRESH--PlotScale = " + _scr.PlotScale);
        _PointScale = _scr.PointScale;
        _BaseX = _scr.BaseX;
        _VertY = _scr.VertY;
        _BaseZ = _scr.BaseZ;
        _Variance = _scr.Variance;

        // calculate new position on this point
        float xPos = Convert.ToSingle(_LSPointPos[_BaseX]);
        float yPos = Convert.ToSingle(_LSPointPos[_VertY]);
        float zPos = Convert.ToSingle(_LSPointPos[_BaseZ]);

        transform.localPosition = new Vector3 (xPos, yPos, zPos) * _PlotScale; //rescale to WorldSpace

        // calculate new scale for this point
        Vector3 scale = new Vector3(1,1,1) * _PointScale;
        float xStd = Convert.ToSingle(_LSPointStd[_BaseX]) * _PlotScale; //rescale to WorldSpace
        float yStd = Convert.ToSingle(_LSPointStd[_VertY]) * _PlotScale;
        float zStd = Convert.ToSingle(_LSPointStd[_BaseZ]) * _PlotScale;
        scale = new Vector3 (xStd, yStd, zStd) * _PointScale;

        transform.localScale = scale; 

        // Sets color according to x/y/z value
        GetComponent<Renderer>().material.color = new Color(xPos, yPos, zPos, 1.0f);

        // Activate emission color keyword so we can modify emission color
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

        GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(xPos, yPos, zPos, 1.0f));
    }

}
