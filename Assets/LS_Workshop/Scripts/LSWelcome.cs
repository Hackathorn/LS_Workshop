using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSWelcome : MonoBehaviour
{
    // Derviced from the Oculus Interaction SampleFrame for StartScene and its StartMenu.cs script

    // DELETE these 3 public variables???
    // public OVROverlay overlay;
    // public OVROverlay text;
    // public OVRCameraRig vrRig;

    // Scenes In Build list should be as follows...
    // 0 Welcome
    // 1 Overview
    // 2 How To Use
    // 3 What is Latent Space?
    // 4 LS Workshop - CONTINUE

    public AudioSource welcomeVoice;
    public static readonly string[] SceneLabels = {"Welcome", "Overview", "How To Use", "What is Latent Space", "CONTINUE..."};

    void Start()
    {
        int n = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; // check for right number of scenes
        // Debug.Log("WELCOME n = " + n);
        if (n == 5) 
        {
            DebugUIBuilder.instance.AddLabel("WELCOME! Select...");
            DebugUIBuilder.instance.AddButton(SceneLabels[1], () => LoadScene(0)); // Overview
            DebugUIBuilder.instance.AddButton(SceneLabels[2], () => LoadScene(0)); // How To Use
            DebugUIBuilder.instance.AddButton(SceneLabels[3], () => LoadScene(0)); // What is LS
            DebugUIBuilder.instance.AddDivider();
            DebugUIBuilder.instance.AddButton(SceneLabels[4], () => LoadScene(4)); // Continue to Workshop

            DebugUIBuilder.instance.AddLabel("Button A to select");
            DebugUIBuilder.instance.Show();

            // welcomeVoice.Play();
        }
    }

    void LoadScene(int idx)
    {
        DebugUIBuilder.instance.Hide();
        Debug.Log("Load scene: " + idx);
        UnityEngine.SceneManagement.SceneManager.LoadScene(idx);
    }
}
