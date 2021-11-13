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

    public static float Show(string text, AudioClip narration)
    {
        var duration = TextDuration(text, narration == null ? 0 : narration.length);
        if (narration)
        {
            PlayerInternalSpeaker.Speaker.PlayOneShot(narration);
        }
        else
        {
            PlayerInternalSpeaker.Mumble(duration);
        }
        return _instance.ShowCaption(text, duration);
    }

    public static float Show(string text)
    {
        return _instance.ShowCaption(text, 0);
    }

    public static void Clear()
    {
        _instance.ClearCaption();
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

    private static float WordCountish(string text)
    {
        return text
            .Split(' ')
            .Select(w => w.Trim().Length)
            .Where(l => l > 0)
            .Select(l => l > 6 ? 1.5f : 1.0f)
            .Sum();
    }
    
    public static float TextDuration(string text, float minDuration)
    {
        var words = WordCountish(text);
        return Mathf.Max(words * 60f / _instance.wordsPerMinute + _instance.extraTime, minDuration);
    }

    private float ShowCaption(string text, float minDuration)
    {
        var duration = TextDuration(text, minDuration);
        StartCoroutine(_showCaption(text, duration));
        return duration;
    }

    private void ClearCaption()
    {
        caption.text = "";
        caption.enabled = false;
    }

    private IEnumerator<WaitForSeconds> _showCaption(string text, float duration)
    {
        caption.text = text;
        caption.enabled = true;
        
        yield return new WaitForSeconds(duration);

        if (caption.text == text)
        {
            ClearCaption();
        }
    }
}
