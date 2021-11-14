using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMain : MonoBehaviour
{    
    [SerializeField]
    Button Resume;

    [SerializeField]
    Button NewGame;

    [SerializeField]
    Button Settings;

    [SerializeField]
    Button Quit;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            Quit.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Resume.gameObject.SetActive(BugWatchSettings.HasProgress);
    }
    
    public void OnResume()
    {
        SceneManager.LoadScene(BugWatchSettings.Level);
    }

    public void OnNewGame()
    {
        BugWatchSettings.NewGame();
        OnResume();
    }

    public void OnSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
