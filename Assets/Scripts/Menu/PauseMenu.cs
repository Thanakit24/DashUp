using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    private PlayerController playerController;
    float originalVol;
    //private Teleport teleport;

    // Update is called once per frame

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        originalVol = AudioManager.instance.musicSource.volume;
        Resume();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        playerController.enabled = true;
        Time.timeScale = 1f;
        GameIsPaused = false;

        AudioManager.instance.musicSource.pitch = 1;
        AudioManager.instance.musicSource.volume = originalVol;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        playerController.enabled = false;
        Time.timeScale = 0f;
        GameIsPaused = true;

        AudioManager.instance.musicSource.pitch = 0.75f;
        originalVol = AudioManager.instance.musicSource.volume;
        AudioManager.instance.musicSource.volume *= 0.5f;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void Skip()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Quit()
    {
        Debug.Log("QuittingGame");
        Application.Quit();
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioManager.instance.musicSource.pitch = 1;
        AudioManager.instance.musicSource.volume = 0.3f; //hardcoded value because setting it back to originalVol makes the volume set to 0 
    }
}
