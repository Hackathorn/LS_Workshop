using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class LSConfigureUI : MonoBehaviour
{
    public Material[] skyBoxes; 
    bool inMenu;
    private Text sliderText;
    private int panel = DebugUIBuilder.DEBUG_PANE_CENTER; // display UI in the Center, Left or Right panels

	void Start ()
    {
        panel = DebugUIBuilder.DEBUG_PANE_LEFT;
        DebugUIBuilder.instance.AddLabel("Choose Menu...", panel);
        DebugUIBuilder.instance.AddButton("Space", SpaceButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Dimension", DimensionButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Move", MoveButtonPressed, panel);
        DebugUIBuilder.instance.AddButton("Cluster", ClusterButtonPressed, panel);
        DebugUIBuilder.instance.AddDivider(panel);
        DebugUIBuilder.instance.AddButton("Status", StatusButtonPressed, panel);
        DebugUIBuilder.instance.AddLabel("Button A to select", panel);
        DebugUIBuilder.instance.AddLabel("Button B to hide", panel);

        // DebugUIBuilder.instance.AddLabel("--Space--  ", panel);
        // DebugUIBuilder.instance.AddToggle("Ball or Pole;", BallPolePressed, panel);
        // DebugUIBuilder.instance.AddDivider(panel);

        panel = DebugUIBuilder.DEBUG_PANE_CENTER; 
        DebugUIBuilder.instance.AddLabel("---Rendering---", panel);
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

        panel = DebugUIBuilder.DEBUG_PANE_RIGHT; 
        DebugUIBuilder.instance.AddLabel("---Status---", panel);

        DebugUIBuilder.instance.Show();
        inMenu = true;

        InvokeRepeating("RefreshStatus", 0f, 1f); // update the Right panel (every second if shown) with current LS status
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
        Debug.Log("Button pressed for SPACE");
    }
 public void DimensionButtonPressed()
    {
        Debug.Log("Button pressed for DIMENSION");
    }
 public void MoveButtonPressed()
    {
        Debug.Log("Button pressed for MOVE");
    }
 public void ClusterButtonPressed()
    {
        Debug.Log("Button pressed for CLUSTER");
    }
    public void RadioPressed(string radioLabel, string group, Toggle t)
    {
        Debug.Log("Radio value changed: "+radioLabel+", from group "+group+". New value: "+t.isOn);
        if (group == "group2" && t.isOn) // Scale = 1m, 10m, 100m, 1km as string "0" ... "3"
        {
            // find public parameters in LSpaceController
            GameObject _go = GameObject.Find("LSWorkshop");
            LSpaceController _scr = _go.GetComponent<LSpaceController>();

            // change PoltScale in LSWorkshop Controller
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
