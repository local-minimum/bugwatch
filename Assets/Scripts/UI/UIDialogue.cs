using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ClickSide { Left, Right };
public class RealizedDialogueOption
{
    private Caption caption;

    public string optionText {
        get
        {
            return caption.shortText;
        }
    }
    public string fullText
    {
        get
        {
            return caption.text;
        }
    }
    public AudioClip narration
    {
        get
        {
            return caption.narration;
        }
    }

    public void UpdatePlayerProfile()
    {
        if (caption.moodEffect != null)
        {
            BugWatchSettings.PlayerProfile = BugWatchSettings.PlayerProfile.Evolve(caption.moodEffect);
        }
    }

    public RealizedDialogueOption(Caption caption)
    {
        this.caption = caption;        
    }
}

public class RealizedDialogue
{
    public RealizedDialogueOption leftOption;
    public RealizedDialogueOption rightOption;
    public RealizedDialogueOption indecisiveOption;
    public string intro;
    public AudioClip introNarration;

    public RealizedDialogue(
        string intro,
        AudioClip introNarration,
        RealizedDialogueOption leftOption,
        RealizedDialogueOption rightOption,
        RealizedDialogueOption indecisiveOption
    )
    {
        this.intro = intro;
        this.introNarration = introNarration;
        this.leftOption = leftOption;
        this.rightOption = rightOption;
        this.indecisiveOption = indecisiveOption;
    }
}


public class UIDialogue : MonoBehaviour
{
    [SerializeField]
    Image leftCountdown;

    [SerializeField]
    Image rightCountdown;

    [SerializeField]
    TMPro.TextMeshProUGUI intro;

    [SerializeField]
    TMPro.TextMeshProUGUI left;

    [SerializeField]
    TMPro.TextMeshProUGUI right;

    [SerializeField, Range(1, 10)]
    float decisionTime = 3f;

    [SerializeField, Range(0, 1)]
    float noTextBonusPreTime = 0.25f;


    private static UIDialogue instance { get; set; }

    public static void Show(RealizedDialogue dialogue)
    {
        PlayerController.Pause = true;
        UICaption.Clear();
        instance.ShowDialogue(dialogue);
    }

    private void Restore()
    {
        Visible = true;        
        leftCountdown.fillAmount = 1;
        rightCountdown.fillAmount = 1;
        left.text = "";
        right.text = "";
        intro.text = "";
    }

    private void ExitDialogue()
    {
        Cursor.visible = false;
        activeDialogue = null;
        Visible = false;
        PlayerController.Pause = false;
    }

    private bool Visible
    {
        set
        {
            for (int i = 0, l = transform.childCount; i<l; i++)
            {
                var child = transform.GetChild(i).gameObject;                
                child.SetActive(value && child != left.transform.parent.gameObject && child != right.transform.parent.gameObject);
            }            
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        leftCountdown.fillMethod = Image.FillMethod.Horizontal;
        leftCountdown.fillMethod = Image.FillMethod.Horizontal;
        Visible = false;
    }

    float aspect = 0;

    private void Update()
    {
        var rect = leftCountdown.rectTransform.rect;
        var aspect = rect.width / rect.height;
        if (this.aspect != aspect)
        {
            var width = Mathf.RoundToInt(rect.width);
            var height = Mathf.RoundToInt(rect.height);
            var tex = new Texture2D(width, height);            
            var sprite = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.one * 0.5f);
            leftCountdown.sprite = sprite;
            rightCountdown.sprite = sprite;
            this.aspect = aspect;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    private RealizedDialogue activeDialogue;

    private void ShowDialogue(RealizedDialogue dialogue)
    {
        Restore();
        StartCoroutine(RunDialogue(dialogue));
    }

    private IEnumerator<WaitForSeconds> RunDialogue(RealizedDialogue dialogue)
    {
        var hasIntro = !string.IsNullOrEmpty(dialogue.intro) || dialogue.introNarration != null;
        activeDialogue = dialogue;
        if (hasIntro)
        {
            var narrationDuration = dialogue.introNarration == null ? 0 : dialogue.introNarration.length;
            var duration = UICaption.TextDuration(dialogue.intro, narrationDuration);
            intro.text = dialogue.intro;
            if (dialogue.introNarration == null)
            {
                PlayerInternalSpeaker.Mumble(duration);
            } else
            {
                PlayerInternalSpeaker.Speaker.PlayOneShot(dialogue.introNarration);
            }
            yield return new WaitForSeconds(duration);
        }

        Cursor.visible = true;        
        left.text = dialogue.leftOption.optionText;
        right.text = dialogue.rightOption.optionText;
        left.transform.parent.gameObject.SetActive(true);
        right.transform.parent.gameObject.SetActive(true);
        if (!hasIntro) {
            yield return new WaitForSeconds(noTextBonusPreTime);
        }
        var countDownTime = UICaption.TextDuration(left.text, 0) + UICaption.TextDuration(right.text, 0) + decisionTime;
        var startTime = Time.timeSinceLevelLoad;
        var progress = 0f;
        while (progress < 1f && activeDialogue != null)
        {
            yield return new WaitForSeconds(0.02f);
            progress = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / countDownTime);
            leftCountdown.fillAmount = 1 - progress;
            rightCountdown.fillAmount = 1 - progress;
        }
        
        if (activeDialogue != null)
        {
            Enact(activeDialogue.indecisiveOption);
            ExitDialogue();
        }
    }

    public void OnClick(bool leftSide)
    {
        if (activeDialogue != null)
        {
            switch (leftSide)
            {
                case true:
                    Enact(activeDialogue.leftOption);
                    break;
                case false:
                    Enact(activeDialogue.rightOption);
                    break;
            }
        }
        ExitDialogue();
    }

    public void Enact(RealizedDialogueOption option)
    {
        UICaption.Show(option.fullText, option.narration);
        option.UpdatePlayerProfile();
    }
}
