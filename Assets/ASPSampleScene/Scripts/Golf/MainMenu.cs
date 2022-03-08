using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    bool howToPlaying = false;
    public GameObject HowToPlayPanel, howToPlayButton;
    
    
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
        howToPlayButton.SetActive(false);
        HowToPlayPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height * 4 / 3, Screen.height);
        howToPlaying = true;
    }

    private void OnMouseDown()
    {
        if (howToPlaying)
        {
            HowToPlayPanel.SetActive(false);
            howToPlaying = false;
            howToPlayButton.SetActive(true);
        }
    }
}

