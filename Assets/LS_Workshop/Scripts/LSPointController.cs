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
    public string myLSPointName;
    public float[] myLSPointPos;
    public float[] myLSPointStd;
    public Sprite myLSPointSprite;
    public int myDimSize;

    // private variables below are set in RefreshPoints below during Update
    private float myPlotScale;
    private float myPointScale;
    private int myBaseX;
    private int myBaseZ;
    private int myVertY;


    // Start is called before the first frame update
    void Start() 
    {
        InitializeParameters();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshPoints();
    }

    private void InitializeParameters()
    {
        // Debug.Log("Setting initial Point Parameters..........");
        // find public parameters in LSpaceController
        GameObject myGO = GameObject.Find("LSpaceWorkshop");
        LSpaceController myScript = myGO.GetComponent<LSpaceController>();

        // set my parameters for this point
        myPlotScale = myScript.PlotScale;
        myPointScale = myScript.PointScale;
        myBaseX = myScript.BaseX;
        myVertY = myScript.VertY;
        myBaseZ = myScript.BaseZ;

        // Debug.Log("transform.name is " + transform.name + " of type " + transform.name.GetType());

        if (transform.name == "0")
        {
            Debug.Log("name = " + transform.name);
            Debug.Log("myPointScale = " + myPlotScale);
            Debug.Log("myPointScale = " + myPointScale);
            Debug.Log("myBases = " + myBaseX + " " + myBaseZ + " " + myVertY);
            Debug.Log("myLSPointPos = " + myLSPointPos[0] + " " + myLSPointPos[1] + " " + myLSPointPos[2]);
        }

    }
    private void RefreshPoints()
    {
        // Debug.Log("Refreshing this point with revised parameters..........");

        // calculate new position on this point
        float xPos = Convert.ToSingle(myLSPointPos[myBaseX]);
        float yPos = Convert.ToSingle(myLSPointPos[myVertY]);
        float zPos = Convert.ToSingle(myLSPointPos[myBaseZ]);
        Vector3 position = new Vector3 (xPos, yPos, zPos) * myPlotScale;
        // Debug.Log(position);

        // calculate new scale for this point
        float xStd = Convert.ToSingle(myLSPointStd[myBaseX]);
        float yStd = Convert.ToSingle(myLSPointStd[myVertY]);
        float zStd = Convert.ToSingle(myLSPointStd[myBaseZ]);
        Vector3 scale = new Vector3 (xStd, yStd, zStd) * myPlotScale * myPointScale;
        
        // Position point at relative to parent PointHolder
        transform.localPosition = position;
        // Scale point relative to Std of LSparent PointHolder
        transform.localScale = scale;

        // Sets color according to x/y/z value
        GetComponent<Renderer>().material.color = new Color(xPos, yPos, zPos, 1.0f);

        // Activate emission color keyword so we can modify emission color
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

        GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(xPos, yPos, zPos, 1.0f));
    }

}
