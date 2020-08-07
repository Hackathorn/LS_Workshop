using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; 
using LSpacesProject;

/// <summary>
/// This script shows how to call a simple SQL query from a database using the class definition of the
/// database to format the results.
///
/// In this example we overwrite the working database since no data is being changed. This is set in the
/// SimpleSQLManager gameobject in the scene.
/// </summary>
public class LSpaceController : MonoBehaviour
{

	// reference to our database manager object in the scene
	public SimpleSQL.SimpleSQLManager dbManager;
	[Tooltip("Prefab for marking a point in N-dimensional space")]

	public GameObject PointPrefab;
	public GameObject PointHolder;
	private int _LSpaceID = 1; 

	//****** define delegate to signal PlotChange event
	public delegate void PlotChange();
	public static event PlotChange onPlotChange;

	//****** define get/set properties for PlotParms to emit PlotChange events
	private float _PlotScale = 10f;
	public float PlotScale 
	{
		get {return _PlotScale; }
		set {_PlotScale = (float) Math.Pow(10f, value); 
			Debug.Log("Plot Scale = " + _PlotScale);
			onPlotChange(); }
	} 
	private float _PointScale = 0.25f;
	public float PointScale 
		{
		get {return _PointScale; }
		set {_PointScale = value;
			Debug.Log("Point Scale = " + _PointScale);
			onPlotChange(); }
	} 
	private int _BaseX = 0;
	public int BaseX 
	{
		get {return _BaseX; }
		set {_BaseX = value;
			onPlotChange(); }
	} 
	private int _BaseZ = 1;
	public int BaseZ 
	{
		get {return _BaseZ; }
		set {_BaseZ = value;
			onPlotChange(); }
	} 
	private int _VertY = 2;
	public int VertY 
	{
		get {return _VertY; }
		set {_VertY = value;
			onPlotChange(); }
	} 

	private bool _Variance = false;
	public bool Variance 
	{
		get {return _Variance; }
		set {_Variance = !_Variance;
			Debug.Log("Variance = " + _Variance);
			onPlotChange(); }
	} 
	public bool Refresh 
	{
		get {return true; }
		set {onPlotChange();
			Debug.Log("Refresh!!!"); }
	} 

	private string _LSName;
	private int _LSSampleSize;
	private int _LSDimSize;
	private int _LSImageWidth;
	private int _LSImageHeight;

	void Awake()
	{
		_LSpaceID = LoadLSpaces();
		CreatePoints(_LSpaceID);
	}

	///  <summary>
	///****** Loads LSpaces from database
	/// FUTURE -- If more than one, select one to process. For now, assume first.
	/// Declare public variable LSpaceIDCurrent. If non-None, use as current.
	///  </summary>
	private int LoadLSpaces()
	{
		// string sql = "SELECT * FROM LSpaces";
		string sql = "SELECT LSpaceID, LSName, LSSampleSize, LSDimSize, LSImageWidth, LSImageHeight FROM LSpaces";
		List<LSpaces> LSpaceList = dbManager.Query<LSpaces>(sql);
		
		// TBD -- insert SelectLSpace via UI if LSpaceList.Count > 1 - FUTURE
		// Otherwise, assume current value of LSpaceIDCurrent, which could be set in Inspector
		_LSpaceID = 1;
		_LSName = LSpaceList[0].LSName;
		_LSSampleSize = LSpaceList[0].LSSampleSize;
		_LSDimSize = LSpaceList[0].LSDimSize;
		_LSImageWidth = LSpaceList[0].LSImageWidth;
		_LSImageHeight = LSpaceList[0].LSImageHeight;

		return _LSpaceID;
	}

	// create each point and set initial static parms 
	private void CreatePoints(int LSpaceIDCurrent)
	{
		string sql = "SELECT LSPointID, LSPointName, LSPointPos, LSPointStd, LSPointImage FROM LSPoints WHERE LSpaceID = " + LSpaceIDCurrent;
		List<LSPoints> LSPointList = dbManager.Query<LSPoints>(sql);

		Debug.Log("Query complete with LSPointList len = " + LSPointList.Count);

		// output the list of LSpaces
		foreach (LSPoints lspoint in LSPointList)
		{
			GameObject _pt = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

			// Make child of PointHolder object as a container in hiearchy
			_pt.transform.parent = PointHolder.transform;

			// Assigns name to the prefab
			_pt.transform.name = lspoint.LSPointID.ToString();

			// unpack blobs for LSPointPos and LSPointStd floats
			int byteLen = lspoint.LSPointPos.Length; 
			int listLen = byteLen / sizeof(float);

			LSPointController __scr = _pt.GetComponent<LSPointController>();

			__scr._LSPointName = lspoint.LSPointName;
			__scr._DimSize = _LSDimSize = listLen;

			var LSPointPosList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointPos, 0, LSPointPosList, 0, byteLen);
			__scr._LSPointPos = LSPointPosList;

			var LSPointStdList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointStd, 0, LSPointStdList, 0, byteLen);
			__scr._LSPointStd = LSPointStdList;
		}
	}
}
