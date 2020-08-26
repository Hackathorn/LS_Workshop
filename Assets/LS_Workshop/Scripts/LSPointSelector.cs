using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSpacesProject;

public class LSPointSelector : MonoBehaviour
{
    public Vector3 rayOrigin;
    public Vector3 rayDirection; 
    // [SerializeField] private Material highlightMaterial;
    
    void FixedUpdate()
    {
        // var ray = Vector3.zero; // = OVRPlayerController.main.ScreenPointToRay...
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            Debug.Log("Found point " + hit.transform.name + "at distance " + hit.distance);
            // var selection = hit.transform;
            // var selectionRenderer = selection.GetComponent<Renderer>();
            // if (selectionRenderer != null)
            // {
            //     selectionRenderer.material = highlightMaterial;
            // }
        }
    }
}
