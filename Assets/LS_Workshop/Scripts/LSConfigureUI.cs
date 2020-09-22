using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class LSConfigureUI : MonoBehaviour
{
    [Tooltip ("Skybox materials: None, Default, etc")]
    public Material[] skyBoxes; 
    [Tooltip ("UI content panels for Space, Dimension, Move, Cluster")]
    public GameObject[] contentPanels; // Content panels under CanvasWithDebug: 0=Space, 3=Dim, 4=Move, 5=Cluster
    bool inMenu;
    private Text sliderText;
    private int panel = DebugUIBuilder.DEBUG_PANE_CENTER; // display UI in the Center, Left or Right panels

	void Start ()
    {
        panel = DebugUIBuilder.DEBUG_PANE_LEFT;  // 2 - Content Left
        DebugUIBuilder.instance.AddLabel("Choose Menu...", panel);
        DebugUIBuilder.instance.AddButton("Space", SpaceButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Dimension", DimensionButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Move", MoveButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Cluster", ClusterButtonPressed, panel);
        DebugUIBuilder.instance.AddDivider(panel);
        DebugUIBuilder.instance.AddButton("Status", StatusButtonPressed, panel);
        // DebugUIBuilder.instance.AddLabel("Button A to select", panel);
        DebugUIBuilder.instance.AddLabel("Button B to hide", panel);

        panel = DebugUIBuilder.DEBUG_PANE_CENTER; // 0 - Center Content for Space
        DebugUIBuilder.instance.AddLabel("---Render Mode---", panel);
        DebugUIBuilder.instance.AddRadio("Ball in 3-dim", "group1", delegate(Toggle t) { RadioPressed("Ball", "group1", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("Pole Mesh in N-dim", "group1", delegate(Toggle t) { RadioPressed("Pole", "group1", t); }, panel) ;

        DebugUIBuilder.instance.AddLabel("---Scale---", panel);
        DebugUIBuilder.instance.AddRadio("    1m", "group2", delegate(Toggle t) { RadioPressed("0", "group2", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("  10m", "group2", delegate(Toggle t) { RadioPressed("1", "group2", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("100m", "group2", delegate(Toggle t) { RadioPressed("2", "group2", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("  1km", "group2", delegate(Toggle t) { RadioPressed("3", "group2", t); }, panel) ;
        DebugUIBuilder.instance.AddLabel("---Sky Dome---", panel);
        DebugUIBuilder.instance.AddRadio("None", "group3", delegate(Toggle t) { RadioPressed("0", "group3", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("Default", "group3", delegate(Toggle t) { RadioPressed("1", "group3", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("Clouds", "group3", delegate(Toggle t) { RadioPressed("2", "group3", t); }, panel) ;
        DebugUIBuilder.instance.AddRadio("Box", "group3", delegate(Toggle t) { RadioPressed("3", "group3", t); }, panel) ;

        panel = DebugUIBuilder.DEBUG_PANE_RIGHT; // 1 - Content Right 
        DebugUIBuilder.instance.AddLabel("---Status---", panel);
        InvokeRepeating("RefreshStatus", 0f, 1f); // update the Right panel (every second if shown) with current LS status

        panel = 3; // 3 - Content Dimensional 
        DebugUIBuilder.instance.AddLabel("---Dimensional---", panel);

        panel = 4; // 4 - Content Move 
        DebugUIBuilder.instance.AddLabel("---Move---", panel);

        panel = 5; // 5 - Content Cluster 
        DebugUIBuilder.instance.AddLabel("---Cluster---", panel);
        DebugUIBuilder.instance.AddToggle("Show Point Images", ShowImagePressed, false, panel);

        DebugUIBuilder.instance.Show();
        inMenu = true;

        // disable content panels, except for Space panel
        for (int i = 0; i < contentPanels.Length; i++) {
            contentPanels[i].SetActive(false);
        }
        contentPanels[0].SetActive(true);  // 0 = Space panel


	}

    public void RefreshStatus()
    {
        if (StatusDisplayed) {
            Debug.Log("Refreshing Status panel");
        }
    }

    private bool StatusDisplayed = false;
    public void StatusButtonPressed()
    {
        StatusDisplayed = !StatusDisplayed;
        Debug.Log("Button pressed for Status. Now " + StatusDisplayed);
    }

    public void SpaceButtonPressed()
    {
        for (int i = 0; i < contentPanels.Length; i++) {
            contentPanels[i].SetActive(false);
        }
        contentPanels[0].SetActive(true);  // 0=Space, 1=Dim, 2=Move, 3=Cluster
    }

    public void DimensionButtonPressed()
    {
        for (int i = 0; i < contentPanels.Length; i++) {
            contentPanels[i].SetActive(false);
        }
        contentPanels[1].SetActive(true);  // 0=Space, 1=Dim, 2=Move, 3=Cluster
    }

    public void MoveButtonPressed()
    {
        for (int i = 0; i < contentPanels.Length; i++) {
            contentPanels[i].SetActive(false);
        }
        contentPanels[2].SetActive(true);  // 0=Space, 1=Dim, 2=Move, 3=Cluster
    }

    public void ClusterButtonPressed()
    {
        for (int i = 0; i < contentPanels.Length; i++) {
            contentPanels[i].SetActive(false);
        }
        contentPanels[3].SetActive(true);  // 0=Space, 1=Dim, 2=Move, 3=Cluster
    }
    public void RadioPressed(string radioLabel, string group, Toggle t)
    {
        // Debug.Log("Radio value changed: "+radioLabel+", from group "+group+". New value: "+t.isOn);
        if (group == "group2" && t.isOn) // Scale = 1m, 10m, 100m, 1km as string "0" ... "3"
        {
            // change PoltScale in LSWorkshop Controller
            GameObject _go = GameObject.Find("LSWorkshop");
            LSpaceController _scr = _go.GetComponent<LSpaceController>();
             _scr.PlotScale = float.Parse(radioLabel);
        }

        if (group == "group3" && t.isOn) // Skybox = None, Default, Clouds, Box as "0" ... "3"
        {
            // change Material in Render > Lightning for Skybox
            RenderSettings.skybox = skyBoxes[int.Parse(radioLabel)]; 
        }
    }
    public void BallPolePressed(Toggle t)
    {
        Debug.Log("Ball-Pole Toggle pressed. Is on? "+t.isOn);
    }

    public void ShowImagePressed(Toggle t)
    {
        // Set isImageShown bool in LSWorkshop Controller to refresh all points
        GameObject _go = GameObject.Find("LSWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();
        _scr.isImageShown = t.isOn;
    }

    public void SliderPressed(float f)
    {
        Debug.Log("Slider: " + f);
        sliderText.text = f.ToString();
    }

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    void ClearPanel(int panel) 
    {
        // get GO for the proper UI panel
        // foreach (Transform child in TextHolder.transform) {
        // GameObject.Destroy(child.gameObject);
    }

}
