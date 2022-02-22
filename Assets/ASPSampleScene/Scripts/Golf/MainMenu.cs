using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    bool howToPlaying = false;
    public GameObject HowToPlayPanel;
    
    public void StartGerrymanderingButton()
    {
        ASPLevelHandler.ClearMemory();
        loadScene("ASPSampleSceneGame");
    }

    private void loadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void HowToPlayButton()
    {
        HowToPlayPanel.SetActive(true);
        howToPlaying = true;
    }

    private void OnMouseDown()
    {
        if (howToPlaying)
        {
            HowToPlayPanel.SetActive(false);
            howToPlaying = false;
        }
    }
}

