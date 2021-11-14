using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    Toggle invertedY;

    [SerializeField]
    Slider readingSpeed;

    private void Start()
    {
        invertedY.isOn = BugWatchSettings.MouseYDirectionInverted;
        readingSpeed.value = BugWatchSettings.WordsPerMinute;
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnInvertedYMouse()
    {
        BugWatchSettings.MouseYDirectionInverted = invertedY.isOn;
    }

    public void OnReadingSpeed()
    {
        BugWatchSettings.WordsPerMinute = readingSpeed.value;
    }
}
