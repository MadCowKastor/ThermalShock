using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void SceneSwap(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void PressQuit()
    {
        Application.Quit();
    }

}
