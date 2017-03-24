/**
*Developer: Asema Hassan SSoe16
*Anatomy Education VR Puzzle game

 * Anatomy Education VR Puzzle game
 * Select the models with laser pointer.
 * Display name on tool tip
 * On Trigger click loads the model into 3D Puzzle VR game scene.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WandControllerForMenu : MonoBehaviour
{
    private SteamVR_ControllerPointerEvents_ObjectNamePopUp laserPointedObj = null;
    private SteamVR_TrackedController device;
    private string selectedModelName = "Human_Bot";

    void Start()
    {
        device = GetComponent<SteamVR_TrackedController>();
        laserPointedObj = GetComponent<SteamVR_ControllerPointerEvents_ObjectNamePopUp>();
        device.TriggerClicked += Trigger;
    }
    void Trigger(object sender, ClickedEventArgs e)
    {
        if (laserPointedObj.selectedTarget != null)
        {
            selectedModelName = laserPointedObj.selectedTarget.name;
            Debug.Log("Trigger has been pressed: " + selectedModelName);
            PlayerPrefs.SetString("CurrentModelName", selectedModelName);
            laserPointedObj.selectedTarget.transform.parent.GetComponent<Animation>().Play("Model_Idle");
            Invoke("LoadPuzzleScene", 2.0f);
        }
    }

    void LoadPuzzleScene()
    {
        SceneManager.LoadScene("PuzzleVRGame");
    }
}
