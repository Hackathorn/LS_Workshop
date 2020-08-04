using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSpacesProject;

public class LSPointSelector : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    
    void Update()
    {
        // var ray = Vector3.zero; // = OVRPlayerController.main.ScreenPointToRay...
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit))
        // {
        //     var selection = hit.transform;
        //     var selectionRenderer = selection.GetComponent<Renderer>();
        //     if (selectionRenderer != null)
        //     {
        //         selectionRenderer.material = highlightMaterial;
        //     }
        // }
    }
}
