using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSpacesProject; 
public class LSPointController : MonoBehaviour
{
    // LSpaceController script in prefab LSpaceWorkshop

    // public variables below are set in LSpaceController script when this point is instantiated
    // these values do not change during Play
    public Gradient LSPointPoleGradient;

    public string _LSPointName;
    public float[] _LSPointPos;
    public float[] _LSPointStd;
    public Sprite _LSPointSprite;
    public Texture2D _LSPointTexture;
    public string _pointClusterName;
    public int _pointClusterCatergory;
    public string _pointClusterLabel;

    private bool _LSPointSelected = false;
    public bool LSPointSelectionToggle () {
        _LSPointSelected = !_LSPointSelected;
        GetComponent<LSBlink>().enabled = _LSPointSelected;
        // Debug.Log("HIT - point " + this.name + " un/selected as " + _LSPointSelected); 
        return _LSPointSelected;
    }

    // private variables below are set in RefreshPoints during onPlotChange
    private GameObject _goBall; 
    private GameObject _goPole; 
    private GameObject _goImage; 
    private GameObject _goLine;

    private float _plotScale;
    private bool _isBall;
    private bool _isNewYCompared;
    private bool _isImageShown;
    private int _baseX;
    private int _baseZ;
    private int _vertY;
    private int _newY;
    private bool _Variance; // uncertain as to function???
    private int _dimSize; // set from _LSPointPos.Length

    // Register listener for onPlotChange and do initial RefreshPoints
    void Start() 
    {
        // subscribe to events
        LSpaceController.onPlotChange += RefreshPoints;
        LSpaceController.onClusterChange += RefreshCluster;

        RefreshPoints();
    }

    // private void Update() 
    // {
    //     AnimateNewY();
    // }

    // Remove listener from onPlotChange when point is destroyed
    private void OnDisable() 
    {
        // UNsubscribe to events
        LSpaceController.onPlotChange -= RefreshPoints;
        LSpaceController.onClusterChange -= RefreshCluster;
    }

    // onPlotChange... Refresh point by getting current plot parms and setting pos & scale
    private void RefreshPoints()
    {
        // find public parameters in LSpaceController
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();

        // set current params for this point
        _plotScale = _scr.PlotScale;
        _isBall = _scr.isBall;
        _isNewYCompared = _scr.isNewYCompared;
        _isImageShown = _scr.isImageShown;
        _baseX = _scr.baseX;
        _baseZ = _scr.baseZ;
        _vertY = _scr.vertY;
        _newY = _scr.newY;
        _Variance = _scr.Variance;
        _dimSize = _LSPointPos.Length;

        // set _go children (Ball, Pole, Image, Line) for this point
        _goBall = this.transform.Find("BallSphere").gameObject;
        _goPole = this.transform.Find("PoleMesh").gameObject;
        _goImage = this.transform.Find("ImageSprite").gameObject;
        _goLine = this.transform.Find("LineRender").gameObject;


        // then refresh either Ball or Pole
        if (_isBall) 
            RefreshPointsAsBall(); 
        else 
            RefreshPointsAsPole();
    }

    private void RefreshPointsAsBall() 
    {
        _goBall.SetActive(true);
        _goPole.SetActive(false);

        // calculate new position on this point
        float xPos = Convert.ToSingle(_LSPointPos[_baseX]);
        float yPos = Convert.ToSingle(_LSPointPos[_vertY]);
        float zPos = Convert.ToSingle(_LSPointPos[_baseZ]);

        _goBall.transform.localPosition = new Vector3 (xPos, yPos, zPos) * _plotScale; //rescale to WorldSpace

        // calculate new scale for this point
        float xStd = Convert.ToSingle(_LSPointStd[_baseX]) * _plotScale; //rescale to WorldSpace
        float yStd = Convert.ToSingle(_LSPointStd[_vertY]) * _plotScale;
        float zStd = Convert.ToSingle(_LSPointStd[_baseZ]) * _plotScale;
        _goBall.transform.localScale = new Vector3 (xStd, yStd, zStd) * 0.25f; // TBC >>>>>>>>>>>>>>>>> FUDGE FACTOR for looks!!!

        // Sets color according to x/y/z values
        Renderer rend = _goBall.GetComponent<Renderer>();
        if (rend != null) 
        {
            rend.material.color = new Color(xPos, yPos, zPos, 1.0f);
            // Activate emission color keyword so we can modify emission color
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", new Color(xPos, yPos, zPos, 1.0f));
        }

        // activate and set image into point sprite
        if (_isImageShown) {
            _goImage.SetActive(true);
            SpriteRenderer _scr2 = _goImage.GetComponent<SpriteRenderer>();
            _scr2.sprite = _LSPointSprite;
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> TBC adjust image scale based on parent
            // like.......... ImagePrefab.localScale /= this.scale;
        }
        else {
            _goImage.SetActive(false);
        }

        AnimateNewY();
    }

    private void RefreshPointsAsPole()
    {
        // private Vector3[] vertices;
        // private int[] triangles;
        // private Color[] colors; 
        // private Gradient pointGradient;

        _goBall.SetActive(false);
        _goPole.SetActive(true);

        // create _dimSize vertices around a X-Z circle clockwise
        Vector3[] vertices = new Vector3[_dimSize+1];
        vertices[0] = Vector3.zero; // set initial vertex in center

        for (int i = 0; i < _dimSize; i++)
        {
            int ii = _dimSize - i - 1;
            float posX = (float) Math.Cos(ii * (2*Math.PI/_dimSize));
            float posZ = (float) Math.Sin(ii * (2*Math.PI/_dimSize));
            vertices[i+1] = new Vector3(posX, _LSPointPos[i], posZ);
        }
        
        // create dimSize triangles[t..t+2], each with 3*dimSize values
        int t = 0; 
        int[] triangles = new int[3 * _dimSize];

        for (int i = 1; i < _dimSize; i++)
        {
            triangles[t  ] = 0  ;
            triangles[t+1] = i  ;
            triangles[t+2] = i+1  ;
            t += 3;
        }
        // complete last triangle
        triangles[t  ] = 0;
        triangles[t+1] = _dimSize;
        triangles[t+2] = 1;

        // create Colors array (instead of UV) for each vertex
        Color[] colors = new Color[vertices.Length];
        float minY = _LSPointPos.Min();
        float maxY = _LSPointPos.Max();
        // aveY = vertices.Average();
        for (int i = 0; i < _dimSize; i++)
        {
            float colorY = Mathf.InverseLerp(minY, maxY, vertices[i].y);
            colors[i] = LSPointPoleGradient.Evaluate(colorY);
        }
        
        // update myMesh
        Mesh myMesh = _goPole.GetComponent<MeshFilter>().mesh;
        myMesh.Clear();
        myMesh.vertices = vertices;
        myMesh.triangles = triangles;
        myMesh.colors = colors;
        // myMesh.RecalculateNormals();

    }

    private bool isComparingNewY = false; 
    private Vector3 startPos, endPos;
    private float newDelta;
    private void AnimateNewY()
    {
        LineRenderer lr = _goLine.GetComponent<LineRenderer>();
        if (!isComparingNewY && _newY != -1)    // if not comparing and newY not NONE 
        {                                       // then let's do it
            isComparingNewY = true;
            startPos = endPos = transform.position;
            endPos.y = Convert.ToSingle(_LSPointPos[_newY]) * _plotScale;
            newDelta = endPos.y - startPos.y; 

            // deactivate MeshRenderer & activate LineRenderer
            // this.GetComponent<MeshRenderer>().enabled = false;
            lr.enabled = true;
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
            lr.startWidth = lr.endWidth = 0.05f;
            if (newDelta > 0) lr.startColor = Color.yellow;
                else lr.startColor = Color.blue;
            lr.endColor = lr.startColor;
            lr.material.EnableKeyword("_EMISSION");
            lr.material.SetColor("_EmissionColor", lr.startColor);

        }
        else    // return space to normal balls
        {
            isComparingNewY = false;
            // this.GetComponent<MeshRenderer>().enabled = true;
            lr.enabled = false;
        }
    }

    private void RefreshCluster() {
        Debug.Log("Refreshing Cluster...");
    }
}
