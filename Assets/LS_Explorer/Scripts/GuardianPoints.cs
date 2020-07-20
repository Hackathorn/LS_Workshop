using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRBoundary;

/*  
    Derived from How To: Generate Wall around Guardian on Oculus Quest in Unity 
    by Mr Pineapple Studio at https://www.youtube.com/watch?v=cCJRNLxfHwg 
    Also see doc at https://developer.oculus.com/documentation/unity/unity-ovrboundary/
*/

public class GuardianPoints : MonoBehaviour
{
    private Vector3[] boundaryPoints;
    private bool isWallCreated = false;

    // GO to mark guardian boundary
    [Header("Guardian Boundard")]
    [Tooltip("GameObject to mark boundary")]
    public GameObject wallPoint;

    // Update is called once per frame
    void Update()
    {
        if (isWallCreated == false)
        {
            CreateWall();
        }
    }

    private void CreateWall()
    {
        Debug.Log("CreateWall invoked");
        // Check if boundary is configured
        bool configured = OVRManager.boundary.GetConfigured();
        if(configured)
        {
            Debug.Log("creating guardian wall");
            // Grab all boundary points; set BoundaryType to OuterBoundary
            boundaryPoints = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);
            Debug.Log(boundaryPoints);

            for (int i = 0; i < boundaryPoints.Length; i++)
            {
                var newPoint = Instantiate(wallPoint, boundaryPoints[i], Quaternion.identity);
                Debug.Log("instantiate wallPoint at " + boundaryPoints[i]);

                /*  if needed to position wallPoint... 
                Vector3 forward = Vector3.zero;
                if (i < boundaryPoints.Length - 1)
                    forward = boundaryPoints[i] - boundaryPoints[i + 1];
                newPoint.transform.forward = forward;
                */
            }
        }
    }
}
