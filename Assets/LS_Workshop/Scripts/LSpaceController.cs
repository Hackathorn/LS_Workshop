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

	public int LSpaceIDCurrent = 1; 
	public float PlotScale = 10.0f; 
	public float PointScale = 0.25f; 
	public int BaseX = 0; 
	public int BaseZ = 1; 
	public int VertY = 2; 

	private string LSNameCurrent;
	private int LSSampleSizeCurrent;
	private int LSDimSizeCurrent;
	private int LSImageWidthCurrent;
	private int LSImageHeightCurrent;

	void Awake()
	{
		LSpaceIDCurrent = LoadLSpaces();
		CreatePoints(LSpaceIDCurrent);
	}

	///  <summary>
	/// Loads LSpaces from database
	/// FUTURE -- If more than one, select one to process. For now, assume first.
	/// Declare public variable LSpaceIDCurrent. If non-None, use as current.
	///  </summary>
	private int LoadLSpaces()
	{
		// Debug.Log("Starting load of LSpaces.......... ");

		// string sql = "SELECT * FROM LSpaces";
		string sql = "SELECT LSpaceID, LSName, LSSampleSize, LSDimSize, LSImageWidth, LSImageHeight FROM LSpaces";
		List<LSpaces> LSpaceList = dbManager.Query<LSpaces>(sql);
		
		Debug.Log("Query complete with LSpaceList len = " + LSpaceList.Count);

		// output the list of LSpaces
		foreach (LSpaces lspace in LSpaceList)
		{
			string outputText =   "LSpaceID: " + lspace.LSpaceID.ToString() + " "+
								"LSName:" + lspace.LSName + " " +
								"LSSampleSize:" + lspace.LSSampleSize.ToString() + " " +
								"LSSampleSize:" + lspace.LSDimSize.ToString() + " " +
								"LSImageWidth:" + lspace.LSImageWidth.ToString() + " " +
								"LSImageHeight:" + lspace.LSImageHeight.ToString() + "\n";
			// Debug.Log(outputText);
		}

		// >>>>>>>>>>>> insert SelectLSpace via UI if LSpaceList.Count > 1 - FUTURE
		// Otherwise, assume current value of LSpaceIDCurrent, which could be set in Inspector
		LSpaceIDCurrent = 1;
		LSNameCurrent = LSpaceList[0].LSName;
		LSSampleSizeCurrent = LSpaceList[0].LSSampleSize;
		LSDimSizeCurrent = LSpaceList[0].LSDimSize;
		LSImageWidthCurrent = LSpaceList[0].LSImageWidth;
		LSImageHeightCurrent = LSpaceList[0].LSImageHeight;

		return LSpaceIDCurrent;
	}

	private void CreatePoints(int LSpaceIDCurrent)
	{
		// asdf
		string sql = "SELECT LSPointID, LSPointName, LSPointPos, LSPointStd, LSPointImage FROM LSPoints WHERE LSpaceID = " + LSpaceIDCurrent;
		List<LSPoints> LSPointList = dbManager.Query<LSPoints>(sql);

		Debug.Log("Query complete with LSPointList len = " + LSPointList.Count);

		Int16 i = 0;
		// output the list of LSpaces
		foreach (LSPoints lspoint in LSPointList)
		{
			GameObject myPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

			// Make child of PointHolder object as a container in hiearchy
			myPoint.transform.parent = PointHolder.transform;

			// Assigns name to the prefab
			myPoint.transform.name = lspoint.LSPointID.ToString();

			// Debug.Log("myPoint.transform.name = " + i + "--" + myPoint.transform.name + "**************************************");

			// unpack blobs for LSPointPos and LSPointStd floats
			int byteLen = lspoint.LSPointPos.Length; 
			int listLen = byteLen / sizeof(float);

			LSPointController myScript = myPoint.GetComponent<LSPointController>();

			myScript.myLSPointName = lspoint.LSPointName;
			myScript.myDimSize = LSDimSizeCurrent = listLen;

			var LSPointPosList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointPos, 0, LSPointPosList, 0, byteLen);
			myScript.myLSPointPos = LSPointPosList;

			var LSPointStdList = new float[listLen];
			Buffer.BlockCopy(lspoint.LSPointStd, 0, LSPointStdList, 0, byteLen);
			myScript.myLSPointStd = LSPointStdList;


			// find child GO ImageSprite, component SpriteRenderer, 
			// set Sprite variable to myLSPointImage
			// GameObject g = transform.Find("ImageSprite").gameObject; 
			// SpriteRenderer mySprite = g.GetComponent<SpriteRenderer>();
			// myLSPointSprite = mySprite.sprite;
			// Debug.Log("Image Length = " + lspoint.LSPointImage.Length);

			// var texture = new Texture2D(2, 2);
			// texture.LoadImage(lspoint.LSPointImage);
			// Rect rec = new Rect(0, 0, LSImageWidthCurrent, LSImageHeightCurrent);
			// myScript.myLSPointSprite = Sprite.Create(texture,rec,new Vector2(0.5f,0.5f),100);; 

			// Debug.Log("LSPointID = " + myPoint.transform.name);
			// Debug.Log("myDimSize = " + myScript.myDimSize);
			// // Debug.Log("LSPointPosList = " + LSPointPosList[0] + " " +LSPointPosList[1] + " " +LSPointPosList[2]);
			// Debug.Log("myLSPointPos = " + myScript.myLSPointPos[0] + " " +myScript.myLSPointPos[1] + " " +myScript.myLSPointPos[2]);
			// // Debug.Log("LSPointStdList = " + LSPointStdList[0] + LSPointStdList[1] + LSPointStdList[2]);
			// Debug.Log("image bytes = " + lspoint.LSPointImage.Length);
			// // Debug.Log("texture W/H = " + texture.width + " " + texture.height);
			// Debug.Log("rec = " + rec);
			// Debug.Log("sprite = " + myScript.myLSPointSprite);
			// Debug.Log("lspoint.LSPointID = " + lspoint.LSPointID);
			
			// i += 1; if (i > 3) { break; }
		}
	}
}
