using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script gets values from CSVReader script
// It instantiates points and particles according to values read

public class LSPlotController2 : MonoBehaviour {

    //********Public Variables********

    // Bools for editor options
    public bool renderPointPrefabs = true;
    public bool renderPrefabsWithColor = true;

    // Name of the input file, no extension
    public string pointfile;
    // public string labelfile;

    // Indices for columns to be assigned
    public int column1 = 0;
    public int column2 = 1;
    public int column3 = 2;

    // Full column names from CSV (as Dictionary Keys)
    public string xColumnName;
    public string yColumnName;
    public string zColumnName;
     
    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
     private float plotScale = 10;

    // Scale of the prefab particlePoints
    [Range(0.0f, 0.5f)]
    public float pointScale = 0.25f;

    // Changes size of particles generated
    [Range(0.0f, 2.0f)]
    public float particleScale = 5.0f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;
    
    //********Private Variables********
        // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    // Number of rows
    private int rowCount;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;
    // private List<Dictionary<string, object>> labelList;

    // Particle system for holding point particles
    private ParticleSystem.Particle[] particlePoints; 


    //********Methods********

    void Awake()
    {
        //Run CSV Reader
        pointList = LSDataController.Read(pointfile);
        // labelList = LSDataController.Read(labelfile);
    }

    // Use this for initialization
    void Start () 
	{
        // Store dictionary keys (column names in CSV) in a list
        List<string> columnList = new List<string>(pointList[1].Keys);
        // List<string> labelList = new List<string>(labelList[1].Values);

        Debug.Log("There are " + columnList.Count + " columns");
        Debug.Log("columnList = " + columnList);

        foreach (string key in columnList)
            Debug.Log("Column name is " + key);

        // Assign column names according to index indicated in columnList
        xColumnName = columnList[column1];
        yColumnName = columnList[column2];
        zColumnName = columnList[column3];
        
        // Get maxes of each axis, using FindMaxValue method defined below
        xMax = FindMaxValue(xColumnName);
        yMax = FindMaxValue(yColumnName);
        zMax = FindMaxValue(zColumnName);

        // Get minimums of each axis, using FindMinValue method defined below
        xMin = FindMinValue(xColumnName);
        yMin = FindMinValue(yColumnName);
        zMin = FindMinValue(zColumnName);
            
        Debug.Log("Max Values: " + xMax + " " + yMax + " " + zMax); // Write to console
        Debug.Log("Min Values: " + xMin + " " + yMin + " " + zMin); // Write to console

        if (renderPointPrefabs == true)
        {
            // Call PlacePoint methods defined below
            PlacePrefabPoints();
                    }

    }
    
		
	// Update is called once per frame
	void Update ()
    {
        //Activate Particle System
    //    GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);

    }

    // Places the prefabs according to values read in
	private void PlacePrefabPoints()
	{
                  
        // Get count (number of rows in table)
        rowCount = pointList.Count;

                for (var i = 0; i < pointList.Count; i++)
        {

            // Set x/y/z, standardized to between 0-1
            // float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            // float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            // float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);
            float x = Convert.ToSingle(pointList[i][xColumnName]);
            float y = Convert.ToSingle(pointList[i][yColumnName]);
            float z = Convert.ToSingle(pointList[i][zColumnName]);

            // Create vector 3 for positioning particlePoints
			Vector3 position = new Vector3 (x, y, z) * plotScale;

			//instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

            
            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;

            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Converts index to string to name the point the index number
            string dataPointName = i.ToString();

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            if (renderPrefabsWithColor == true)
            {
                // Sets color according to x/y/z value
                dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

                // Activate emission color keyword so we can modify emission color
                dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));

            }
                                  						
		}

	}

    //Method for finding max value, assumes PointList is generated
    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    //Method for finding minimum value, assumes PointList is generated
    private float FindMinValue(string columnName)
    {
        //set initial value to first value
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

}


