using UnityEngine;
using System.Linq;
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
	public GameObject PoleHolder;  // parent of all LS Poles instantiated, one per dimension
	public int SampleSizeLimit = 1000;

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
	// isNewYCompared is switch for animating Y position of points
	private bool _isNewYCompared = false;  // initial rendering as Ball
	public bool isNewYCompared 
	{
		get {return _isNewYCompared; }
		set {_isNewYCompared = value;  
			Debug.Log("isImageShown set to " + _isNewYCompared);
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
	private int _newY = -1;   // -1 = NONE >>>>>>>>>>> FUDGE TBC
	public int newY {
		get {return _newY; }
		set {_newY = value;
			// onPlotChange();  // Hold alerting points >>>>>>>>>>> TBC
			}
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
	public int _LSDimSize;  // problems with pos/std arrays in LSPoint
	private int _LSImageWidth;
	private int _LSImageHeight;
	// cluster data
	private string _clusterName; 
	private byte[] _clusterMap; 
	private string[] _clusterLabels; 

	void Awake()
	{
		_CurrentLSpaceID = LoadSpaces();
		LoadClusters(_CurrentLSpaceID);
		CreatePoints(_CurrentLSpaceID);
		CreatePoles(_CurrentLSpaceID);
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

		// Debug.Log("SELECT with LSClusterList len = " + LSClusterList.Count);

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
			// Debug.Log("Count of Outliers in ClusterMap: " + outliers); // TBC - deal with DB outliers >>>>>>>
			// Debug.Log("Min/Max values of ClusterMap: " + min + ", " + max);

			// get cluster labels from comma-delimited string; also remove blanks, tabs, etc
			char[] delim = {',', ' ', '\t'};
			_clusterLabels = LSClusterList[0].LSClusterLabels.Split(delim, StringSplitOptions.RemoveEmptyEntries); 
			// Debug.Log("First/second category label: " + _clusterLabels[0] + ", " + _clusterLabels[1]); 
		}
		else {
			Debug.Log("No clusters found in LS database");
		}
	}

	private float[,] DimPos = new float[1000, 8];
	private float[,] DimStd = new float[1000, 8]; 
	private float DimPosMax = 0f;
	private float DimPosMin = 0f;
	private float DimStdMax = 0f;
	private float DimStdMin = 0f;
	private int[,] DimBins;

	/// <summary>
    /// Create each point and set initial static parms.
    /// </summary>
    /// <param name="LSpaceID">ID to the current rendered Latent Space</param>
	private void CreatePoints(int LSpaceID)
	{
		string sql = "SELECT LSPointID, LSPointName, LSPointPos, LSPointStd, LSPointImage FROM LSPoints WHERE LSpaceID = " + LSpaceID;
		List<LSPoints> LSPointList = dbManager.Query<LSPoints>(sql);

		// output the list of LS Points
		int ptCount = 0; // count points for SampleSizeLimit

		foreach (LSPoints lspoint in LSPointList)
		{
			if (ptCount > SampleSizeLimit) break; // stop if limit is exceeded

			// create the point prefab object
			GameObject _pt = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

			// Make child of PointHolder object as the Point container 
			_pt.transform.parent = PointHolder.transform;

			// Assigns name to the prefab
			_pt.transform.name = "pt" + lspoint.LSPointID.ToString();

			LSPointController _scr = _pt.GetComponent<LSPointController>();
			_scr._LSPointName = lspoint.LSPointName;

			// unpack blobs for LSPointPos and LSPointStd floats
			int byteLen = lspoint.LSPointPos.Length; 
			int listLen = byteLen / sizeof(float);

			// what if this listLen and _LSDimSize differ???
			if (_LSDimSize != listLen) {
				Debug.Log("LS ERROR: Dim Size not equal to point Pos/Std array size!!!");
			}
			_scr._LSDimSize = _LSDimSize; 

			var LSPointPosList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointPos, 0, LSPointPosList, 0, byteLen);
			_scr._LSPointPos = LSPointPosList;

			var LSPointStdList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointStd, 0, LSPointStdList, 0, byteLen);
			_scr._LSPointStd = LSPointStdList;

			// compile dimensional data
			for (int i = 0; i < listLen; i++) {
				DimPos[ptCount,i] = LSPointPosList[i];
				DimStd[ptCount,i] = LSPointStdList[i];
			}

			// create sprite from LSPointImage bytes
			int ExpectedBytes = _LSImageWidth * _LSImageHeight;
			int ActualBytes = lspoint.LSPointImage.Length;
			if (ExpectedBytes != ActualBytes) {
				Debug.Log("LS ERROR: Expected bytes for point image not equal to actual bytes");
			}
			else  // continue with creating point sprite
			{
				Texture2D tex = new Texture2D(_LSImageWidth, _LSImageHeight, TextureFormat.Alpha8, false);
				tex.LoadRawTextureData(lspoint.LSPointImage);
				tex.Apply();
				_scr._LSPointTexture = tex;
				// Debug.Log("loaded texture: " + tex.width);
				_scr._LSPointSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1);
				// Debug.Log("created sprite");
			}

			// set cluster into point
			_scr._pointClusterName = _clusterName;
			int cat = _clusterMap[lspoint.LSPointID]; 
			_scr._pointClusterCatergory = cat;
			_scr._pointClusterLabel = _clusterLabels[cat]; 

			ptCount++;
		}
	}
	/// <summary>
    /// sdfasdfasdf>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    /// </summary>
    /// <param name="LSpaceID"> ID to the current rendered Latent Space</param>
	private void CreatePoles(int LSpaceID)
	{
		// loop thru all samples and dimensions to find max/min of Pos/Std
		for (int i = 0; i < _LSSampleSize; i++) {
			for (int j = 0; j < _LSDimSize; j++) {
				if (DimPos[i,j] > DimPosMax) 
					DimPosMax = DimPos[i,j];
				if (DimPos[i,j] < DimPosMin) 
					DimPosMin = DimPos[i,j];
				if (DimPos[i,j] > DimStdMax) 
					DimStdMax = DimPos[i,j];
				if (DimPos[i,j] < DimStdMin) 
					DimStdMin = DimPos[i,j];
			}
		}

		// Debug.Log("DimPosMax=" + DimPosMax + " DimPosMin=" + DimPosMin);
		// Debug.Log("DimStdMax=" + DimStdMax + " DimStdMin=" + DimStdMin);

		// calculate Ceil and Floor for Pos to 0.1f boundary 
		float DimPosCeil = Mathf.Ceil(10*DimPosMax) / 10;
		float DimPosFloor = Mathf.Floor(10*DimPosMin) / 10; 
		// Debug.Log("DimPosCeil= " + DimPosCeil + " DimPosFloor=" + DimPosFloor);

		// bin DimPos values into [DimPosMin, DimPosMax] interval where width is 0.1f
		int NoBins = (int) (10f * (DimPosCeil - DimPosFloor)) + 1;  // add extra bin
		DimBins = new int[_LSDimSize, NoBins];
		// Debug.Log("Spread=" + (DimPosCeil-DimPosFloor) + " NoBins=" + NoBins);

		int maxBinCount = 0;
		for (int i = 0; i < _LSDimSize; i++)   // for each dimension
		{
			// if (i > 0) break;  // >>>>>>>>>>>>>>>>>>>>>> DEBUG one dim

			for (int j = 0; j < _LSSampleSize; j++) 
			{
				// calculate bin counts across all samples
				int jj = (int) ((10f * (DimPos[j,i] - DimPosFloor)) + 0.5f); 
				DimBins[i,jj]++;
				// if ((DimPos[j,i] > -0.05f) && (DimPos[j,i] < 0.05f)) {
				// 	Debug.Log("CLOSE TO ZERO: DimPos=" + DimPos[j,i] + " Bin=" + jj); }

				// find max count among all bins across all dimensions
				if (DimBins[i,jj] > maxBinCount) maxBinCount = DimBins[i,jj];
			}

            // calculate a circle of poles in XZ plane
			float polePosScale = 10f;
            float posX = polePosScale * (float) Math.Cos(i * (2*Math.PI/_LSDimSize));
            float posZ = polePosScale * (float) Math.Sin(i * (2*Math.PI/_LSDimSize));

			// create column of spheres for each pole
			for (int j = 0; j < NoBins; j++)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.parent = PoleHolder.transform;  // store all Pole spheres here
				sphere.name = String.Format("Dim{0}-Bin{1}-Cnt{2}", i, j, DimBins[i,j]);

				// calculate posY for middle of next bin, like [-3.6,-3.5...-0.1,0.0,+0.1...3.6,3.7]
				float posY = polePosScale * (((float) j / 10f) + DimPosFloor);
				sphere.transform.position = new Vector3(posX, posY, posZ);

				// Debug.Log(String.Format("Dim {0} Bin {1} as posY = {2}", i, j, posY)); 

				// set XZ scale to DimBins count; 
				float binSpacing = 1f; // distance between adjacent bin centers on Y axis
				float binWidth = 2f; 			// boost radius over the Y axis
				float countPercent = (float) DimBins[i,j] / (float) maxBinCount;
				float radiusXZ = binWidth * countPercent; // scale radius on bin count
				sphere.transform.localScale = new Vector3(radiusXZ, binSpacing/2f, radiusXZ);

				// color sphere based on posY value 
				MeshRenderer ren = sphere.GetComponent<MeshRenderer>();
				ren.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
				ren.material.EnableKeyword("_EMISSION");
				ren.material.SetColor("_EmissionColor", Color.red);
				// if (posY == 0f) ren.material.SetColor("_EmissionColor", Color.black);
				if ((int) Mathf.Round(posY) == 0) ren.material.SetColor("_EmissionColor", Color.black);

				// enable trigger in shpere collider
				SphereCollider col = sphere.GetComponent<SphereCollider>();
				col.isTrigger = true;
			}
		}
	}
}
