using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestLoadScene : MonoBehaviour {

    public string selectedModelName;
    // Use this for initialization
    void Start () {
	
	}
	// Update is called once per frame
	void Update () {
	
	}
    public void Test_LoadPuzzleScene()
    {
        PlayerPrefs.SetString("CurrentModelName", selectedModelName);
        SceneManager.LoadScene("PuzzleVRGame");
    }
}
