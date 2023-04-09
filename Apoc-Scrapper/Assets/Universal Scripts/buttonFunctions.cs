using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        // Changing our state to be unpaused
        gameManager.instance.UnpauseState();

        // Toggling our is paused bool
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }
    
    public void Restart()
    {
        // Ensuring we are unpaused when we restart the level
        gameManager.instance.UnpauseState();

        // Reloading our current scene using Scene Manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void Quit()
    {
        Application.Quit(); 
    }
}
