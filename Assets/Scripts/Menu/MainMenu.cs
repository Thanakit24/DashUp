using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Update is called once per frame
    public void LevelSelectionScreen()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Credit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
