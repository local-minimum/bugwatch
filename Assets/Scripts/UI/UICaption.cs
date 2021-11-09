using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UICaption : MonoBehaviour
{   
    private static UICaption _instance { get; set; }

    public static void Show(string text)
    {
        _instance.ShowCaption(text);
    }

    [SerializeField]
    TMPro.TextMeshProUGUI caption;

    float wordsPerMinute = 200;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else if (_instance != this)
        {
            Destroy(gameObject);
        }
        BugWatchSettings.OnChangeFloatSetting += BugWatchSettings_OnChangeFloatSetting;
    }

    private void Start()
    {
        caption.text = "";
        caption.enabled = false;
        wordsPerMinute = BugWatchSettings.WordsPerMinute;
    }

    private void BugWatchSettings_OnChangeFloatSetting(GameSetting setting, float value)
    {
        switch (setting)
        {
            case GameSetting.WordsPerMinute:
                wordsPerMinute = value;
                break;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        BugWatchSettings.OnChangeFloatSetting -= BugWatchSettings_OnChangeFloatSetting;
    }

    private float wordCountish(string text)
    {
        return text
            .Split(' ')
            .Select(w => w.Trim().Length)
            .Where(l => l > 0)
            .Select(l => l > 6 ? 1.5f : 1.0f)
            .Sum();
    }
    
    private void ShowCaption(string text)
    {
        StartCoroutine(_showCaption(text));
    }

    private IEnumerator<WaitForSeconds> _showCaption(string text)
    {
        caption.text = text;
        caption.enabled = true;
        var words = wordCountish(text);
        yield return new WaitForSeconds(words * 60f / wordsPerMinute);
        if (caption.text == text)
        {
            caption.text = "";
            caption.enabled = false;
        }
    }
}
