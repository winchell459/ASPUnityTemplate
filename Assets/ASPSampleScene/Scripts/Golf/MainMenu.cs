using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    
    public void StartGerrymanderingButton()
    {
        ASPLevelHandler.ClearMemory();
        loadScene("ASPSampleSceneGame");
    }

    private void loadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

