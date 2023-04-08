using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.PlayState();
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }
    public void Save()
    {

    }
    public void Restart()
    {
        gameManager.instance.PlayState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Options()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }

}
