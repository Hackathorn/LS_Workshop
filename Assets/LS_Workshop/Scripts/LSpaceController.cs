using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; 
using LSpacesProject;

/// <summary>
/// >>>>>>>>>>>>>>> TBC
/// </summary>
public class LSpaceController : MonoBehaviour
{

	// reference to SQLite3 database manager object in the scene
	public SimpleSQL.SimpleSQLManager dbManager;

	[Tooltip("Prefab for marking a point in N-dimensional space")]
	public GameObject PointPrefab;  // prefab for LS Points
	public GameObject PointHolder;  // parent of all LS Points instantiated

	// public int CurrentLSpaceID = 1;  // assume using first (maybe only) LSpace in database

	//****** define delegate/event to signal PlotChange event
	public delegate void PlotChange();
	public static event PlotChange onPlotChange;
	public delegate void ClusterChange();
	public static event ClusterChange onClusterChange;

	//****** define get/set properties for PlotParms to emit PlotChange events

	// PlotScale scales LS mean/std value into World space for cube dimensions (1m, 10m...). 
	// That cube contains the entire Latent Space.
	private float _PlotScale = 10f;  // initial PlotScale for 100m world cube
	public float PlotScale 
	{
		get {return _PlotScale; }
		// multiply LS values by PlotScale for World Space position & scale
		set {_PlotScale = (float) Math.Pow(10f, value - 1f);  
			Debug.Log("PLOT CHANGE = " + _PlotScale);
			onPlotChange(); }
	} 

	// isBall is switch between rendering points as ball in 3-dim versus meshes in n-dim
	private bool _isBall = true;  // initial rendering as Ball
	public bool isBall 
	{
		get {return _isBall; }
		set {_isBall = value;  
			onPlotChange(); }
	} 
	// isImageShown is switch for showing point images
	private bool _isImageShown = false;  // initial rendering as Ball
	public bool isImageShown 
	{
		get {return _isImageShown; }
		set {_isImageShown = value;  
			Debug.Log("isImageShown set to " + _isImageShown);
			onPlotChange(); }
	} 
	private int _baseX = 0;
	public int baseX {
		get {return _baseX; }
		set {_baseX = value;
			onPlotChange(); }
	} 
	private int _baseZ = 1;
	public int baseZ {
		get {return _baseZ; }
		set {_baseZ = value;
			onPlotChange(); }
	} 
	private int _vertY = 2;
	public int vertY {
		get {return _vertY; }
		set {_vertY = value;
			onPlotChange(); }
	} 
	private int _newY = -1;
	public int newY {
		get {return _newY; }
		set {_newY = value;
			onPlotChange(); }
	} 

	private bool _Variance = false;
	public bool Variance {
		get {return _Variance; }
		set {_Variance = !_Variance;
			Debug.Log("Variance = " + _Variance);
			onPlotChange(); }
	} 
	public bool Refresh {
		get {return true; }
		set {onPlotChange();
			Debug.Log("Refresh!!!"); }
	} 

	private int _CurrentLSpaceID; 
	private string _LSName;
	private int _LSSampleSize;
	private int _LSDimSize;
	private int _LSImageWidth;
	private int _LSImageHeight;

	private string _clusterName; 
	private byte[] _clusterMap; 
	private string[] _clusterLabels; 


	void Awake()
	{
		_CurrentLSpaceID = LoadSpaces();
		LoadClusters(_CurrentLSpaceID);
		CreatePoints(_CurrentLSpaceID);
	}

	///  <summary>
	///****** Loads LSpaces from database
	/// FUTURE -- If more than one, select one to process. For now, assume first.
	///  </summary>
	private int LoadSpaces()
	{
		// string sql = "SELECT * FROM LSpaces";
		string sql = "SELECT LSpaceID, LSName, LSSampleSize, LSDimSize, LSImageWidth, LSImageHeight "
			 + "FROM LSpaces";
		List<LSpaces> LSpaceList = dbManager.Query<LSpaces>(sql);
		
		// TBC -- insert SelectLSpace via UI if LSpaceList.Count > 1 - FUTURE
		// Otherwise, assume current value of LSpaceIDCurrent, which could be set in Inspector
		int _LSpaceID = 1;

		if (LSpaceList.Count == 1)
		{
			_LSName = LSpaceList[0].LSName;
			_LSSampleSize = LSpaceList[0].LSSampleSize;
			_LSDimSize = LSpaceList[0].LSDimSize;
			_LSImageWidth = LSpaceList[0].LSImageWidth;
			_LSImageHeight = LSpaceList[0].LSImageHeight;
			return _LSpaceID;
		}
		else 
		{
			Debug.Log("ERROR - More than one LSpace in database");
			return _LSpaceID - 1;
		}
	}

	// load all cluster data for CurrentLSpaceID
	private void LoadClusters(int LSpaceID) 
	{
		string sql = "SELECT LSClusterName, LSClusterMap, LSClusterLabels FROM LSClusters WHERE LSpaceID = " + LSpaceID;
		List<LSClusters> LSClusterList = dbManager.Query<LSClusters>(sql);

		Debug.Log("SELECT with LSClusterList len = " + LSClusterList.Count);

		// Only deal with the first cluster -- TBC >>>>>>>>>>
		if (LSClusterList.Count > 0) 
		{
			// get cluster name from database
			_clusterName = LSClusterList[0].LSClusterName;

			// get cluster map 
			_clusterMap = LSClusterList[0].LSClusterMap;

			// find min/max values and outlier count of mapping
			byte min = 0; byte max = 0; int outliers = 0;
			for (int i = 0; i < _clusterMap.Length; i++) {
				if (_clusterMap[i] == 255) {
					outliers++;
				} else {
					if (_clusterMap[i] < min) min = _clusterMap[i];
					if (_clusterMap[i] > max) max = _clusterMap[i];
				}
			}
			Debug.Log("Count of Outliers in ClusterMap: " + outliers); // TBC - deal with DB outliers >>>>>>>
			Debug.Log("Min/Max values of ClusterMap: " + min + ", " + max);

			// get cluster labels from comma-delimited string; also remove blanks, tabs, etc
			char[] delim = {',', ' ', '\t'};
			_clusterLabels = LSClusterList[0].LSClusterLabels.Split(delim, StringSplitOptions.RemoveEmptyEntries); 
			Debug.Log("First/second category label: " + _clusterLabels[0] + ", " + _clusterLabels[1]); 
		}
		else {
			Debug.Log("No clusters found in LS database");
		}
	}

	/// <summary>
    /// Create each point and set initial static parms.
    /// </summary>
    /// <param name="LSpaceID">ID to the current rendered Latent Space</param>
	private void CreatePoints(int LSpaceID)
	{
		string sql = "SELECT LSPointID, LSPointName, LSPointPos, LSPointStd, LSPointImage FROM LSPoints WHERE LSpaceID = " + LSpaceID;
		List<LSPoints> LSPointList = dbManager.Query<LSPoints>(sql);

		Debug.Log("Query complete with LSPointList len = " + LSPointList.Count);

		// output the list of LS Points
		foreach (LSPoints lspoint in LSPointList)
		{
			GameObject _pt = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

			// Make child of PointHolder object as the Point container 
			_pt.transform.parent = PointHolder.transform;

			// Assigns name to the prefab
			_pt.transform.name = lspoint.LSPointID.ToString();

			LSPointController _scr = _pt.GetComponent<LSPointController>();
			_scr._LSPointName = lspoint.LSPointName;

			// unpack blobs for LSPointPos and LSPointStd floats
			int byteLen = lspoint.LSPointPos.Length; 
			int listLen = byteLen / sizeof(float);
			// ??? what if this listLen and _LSDimSize differ???
			_scr._DimSize = _LSDimSize = listLen;   // make it the same

			var LSPointPosList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointPos, 0, LSPointPosList, 0, byteLen);
			_scr._LSPointPos = LSPointPosList;

			var LSPointStdList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointStd, 0, LSPointStdList, 0, byteLen);
			_scr._LSPointStd = LSPointStdList;

			// create sprite from LSPointImage bytes
			Texture2D tex = new Texture2D(_LSImageWidth, _LSImageHeight, TextureFormat.Alpha8, false);
			Debug.Log("Tex format: " + tex.format);

			int ExpectedBytes = _LSImageWidth * _LSImageHeight;
			Debug.Log("expected bytes: " + ExpectedBytes);
			int ActualBytes = lspoint.LSPointImage.Length;
			Debug.Log("Actual bytes: " + ActualBytes);

			tex.LoadRawTextureData(lspoint.LSPointImage);
			tex.Apply();
			_scr._LSPointTexture = tex;
			Debug.Log("loaded texture: " + tex.width);
			_scr._LSPointSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1);
			Debug.Log("created sprite");

			_scr._pointClusterName = _clusterName;
			int cat = _clusterMap[lspoint.LSPointID]; 
			_scr._pointClusterCatergory = cat;
			_scr._pointClusterLabel = _clusterLabels[cat]; 
		}
	}
}
