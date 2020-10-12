using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleThruPoints : MonoBehaviour
{
    public float Duration;
    public int PointCount;
    public int curPoint;
    private MeshRenderer[] renList;
    float elapsed = 0f;
    bool lateStart = true;

    void Update()
    {
        if (lateStart) // do once at first update
        {
            lateStart = false;
            // create list of points
            Transform pointHolder = this.transform.Find("PointHolder");
            if (pointHolder != null) 
            {
                PointCount = pointHolder.childCount;
                renList = new MeshRenderer[PointCount];

                // disable all the pole meshes
                for (int i = 0; i < PointCount; i++)
                {
                    GameObject pt = pointHolder.GetChild(i).gameObject;
                    renList[i] = pt.transform.Find("PoleMesh").GetComponent<MeshRenderer>();
                    renList[i].enabled = false;
                }
            }
        }
        elapsed += Time.deltaTime;
        if (elapsed >= Duration) 
        {
            elapsed = 0;
            // disable mesh renderer of previous PoleMesh
            renList[curPoint].enabled = false;
            // next point, recycle if needed
            curPoint++;
            if (curPoint >= PointCount) curPoint = 0;
            // enable mesh renderer of next PoleMesh
            renList[curPoint].enabled = true;
        }
    }
}
