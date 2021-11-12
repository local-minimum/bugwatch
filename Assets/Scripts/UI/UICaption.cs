using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UICaption : MonoBehaviour
{   
    private static UICaption _instance { get; set; }

    public static bool Ready
    {
        get
        {
            return _instance != null;
        }
    }

    public static float Show(string text)
    {
        return _instance.ShowCaption(text);
    }

    [SerializeField]
    TMPro.TextMeshProUGUI caption;

    float wordsPerMinute = 200;

    [SerializeField]
    float extraTime = 1f;

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
    
    private float ShowCaption(string text)
    {
        var words = wordCountish(text);
        var duration = words * 60f / wordsPerMinute + extraTime;
        StartCoroutine(_showCaption(text, duration));
        return duration;
    }

    private IEnumerator<WaitForSeconds> _showCaption(string text, float duration)
    {
        caption.text = text;
        caption.enabled = true;
        
        yield return new WaitForSeconds(duration);

        if (caption.text == text)
        {
            caption.text = "";
            caption.enabled = false;
        }
    }
}
