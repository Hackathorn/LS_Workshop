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

    public string _LSPointName;
    public int _LSDimSize;
    public float[] _LSPointPos;
    public float[] _LSPointStd;
    public Sprite _LSPointSprite;
    public Texture2D _LSPointTexture;
    public Gradient LSPointPoleGradient;
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
    private GameObject _goLSWorkshop;
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
    // private int _dimSize; // set from _LSPointPos.Length

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
        _goLSWorkshop = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _goLSWorkshop.GetComponent<LSpaceController>();

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

        // set _go children (Ball, Pole, Image, Line) for this point
        _goBall = this.transform.Find("BallSphere").gameObject;
        _goPole = this.transform.Find("PoleMesh").gameObject;
        _goImage = this.transform.Find("ImageSprite").gameObject;
        _goLine = this.transform.Find("LineRender").gameObject;


        // Refresh either Ball or Pole
        if (_isBall) 
            RefreshPointsAsBall(); 
        else 
            RefreshPointsAsPoleMesh();
            // RefreshPointsAsPoleLine();
    }

    private void RefreshPointsAsBall() 
    {
        _goBall.SetActive(true);
        _goPole.SetActive(false);
        _goLSWorkshop.transform.Find("PoleHolder").gameObject.SetActive(false);

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

    private void RefreshPointsAsPoleMesh()
    {
        _goBall.SetActive(false);
        _goPole.SetActive(true);
        _goLSWorkshop.transform.Find("PoleHolder").gameObject.SetActive(true);

        // create _LSDimSize vertices around a X-Z circle clockwise
        // remove center vertex so that # of vectices = dimSize
        Vector3[] vertices = new Vector3[_LSDimSize];

        for (int i = 0; i < _LSDimSize; i++)
        {
            int ii = _LSDimSize - i - 1;
            float posX = _plotScale * (float) Math.Cos(ii * (2*Math.PI/_LSDimSize));
            float posZ = _plotScale * (float) Math.Sin(ii * (2*Math.PI/_LSDimSize));
            float posY = _plotScale * _LSPointPos[i];
            vertices[i] = new Vector3(posX, posY, posZ);
        }
        
        // create dimSize triangles[t..t+2], each with 3*dimSize values
        int t = 0; 
        int[] triangles = new int[3 * (_LSDimSize - 1)];

        for (int i = 1; i < (_LSDimSize - 1); i++)
        {
            triangles[t  ] = 0  ;
            triangles[t+1] = i  ;
            triangles[t+2] = i+1  ;
            t += 3;
        }

        // create Colors array (instead of UV) for each vertex
        Color[] colors = new Color[vertices.Length];
        float minY = _LSPointPos.Min();
        float maxY = _LSPointPos.Max();
        // aveY = vertices.Average();
        for (int i = 0; i < _LSDimSize; i++)
        {
            float colorY = Mathf.InverseLerp(minY, maxY, vertices[i].y);
            colors[i] = LSPointPoleGradient.Evaluate(colorY);
        }
        
        // update mesh filter
        Mesh myMesh = _goPole.GetComponent<MeshFilter>().mesh;
        myMesh.Clear();
        myMesh.vertices = vertices;
        myMesh.triangles = triangles;
        myMesh.colors = colors;  // may be in conflict with Shader Graph code >>>>>>>>>>
        // myMesh.RecalculateNormals();  // may not be needed

        // update mesh collider
        // Mesh myCol = _goPole.GetComponent<MeshCollider>().mesh;  // not needed???
    }

    private void RefreshPointsAsPoleLine()
    {
        _goBall.SetActive(false);
        _goPole.SetActive(true);
        _goLSWorkshop.transform.Find("PoleHolder").gameObject.SetActive(true);

        // create _LSDimSize vertices around a X-Z circle
        Vector3[] vertices = new Vector3[_LSDimSize];

        for (int i = 0; i < _LSDimSize; i++)
        {
            float posX = (float) Math.Cos(i * (2*Math.PI/_LSDimSize));
            float posZ = (float) Math.Sin(i * (2*Math.PI/_LSDimSize));
            vertices[i] = new Vector3(posX, _LSPointPos[i], posZ);

            // create sphere for each vertex
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.05f, 0.01f, 0.05f);
            sphere.transform.position = vertices[i];

            // color sphere based on posY value 
            var ren = sphere.GetComponent<MeshRenderer>();
            ren.material.SetColor("_Color", Color.red);

            // merge close Pole spheres together, increasing size
            // >>>>>>>>>>>>>> TBC
        }
        
        // draws lines among vertices
        LineRenderer lr = _goLine.GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        lr.material.EnableKeyword("_EMISSION");
        lr.startWidth = lr.endWidth = 0.01f;
        lr.endColor = lr.startColor = Color.yellow;
        lr.material.SetColor("_EmissionColor", lr.startColor);                    

        lr.positionCount = vertices.Length;
        lr.SetPositions(vertices);

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lr.colorGradient = gradient;
        

        // set color of sphere based on  Colors array (instead of UV) for each vertex
        // Color[] colors = new Color[vertices.Length];
        // float minY = _LSPointPos.Min();
        // float maxY = _LSPointPos.Max();
        // // aveY = vertices.Average();
        // for (int i = 0; i < _LSDimSize; i++)
        // {
        //     float colorY = Mathf.InverseLerp(minY, maxY, vertices[i].y);
        //     colors[i] = LSPointPoleGradient.Evaluate(colorY);
        // }
        

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
