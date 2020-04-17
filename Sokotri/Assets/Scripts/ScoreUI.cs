using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreTitle;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI highScoreTitle;

    [SerializeField]
    private TextMeshProUGUI highScoreText;

    [SerializeField]
    private TextMeshProUGUI Match3ModeTitle;

    [SerializeField]
    private TextMeshProUGUI Match3ModeText;

    [SerializeField]
    private TextMeshProUGUI helpText;

    [SerializeField]
    private Image helpBG;

    [SerializeField]
    private Color32 scoreTextColor;

    [SerializeField]
    private Color32 scoreTitleColor;

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private CircleBG[] circles = new CircleBG[2];

    private int currentHighScore = 0;

    public bool isRainbow = false;

    public bool isHelpOn = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();

        if (scoreTitle == null)
            scoreTitle = GetComponent<TextMeshProUGUI>();

        if (scoreText != null)
            scoreText.color = scoreTextColor;

        scoreTitle.color = scoreTitleColor;
        currentHighScore = PlayerPrefs.GetInt(TheGrid.highScoreKey, 0);

        if (highScoreText)
            highScoreText.text = currentHighScore.ToString();

        if (Match3ModeText)
            Match3ModeText.text = grid_ref.getMatch3Uses().ToString();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void UpdateScore(int val, Color32 c)
    {
        if (c != Color.white)
            scoreText.color = c;

        UtilityTools.updateTextWithPunch(scoreText, val.ToString(), scoreText.transform.localScale / 1.8f, TheGrid.moveTime, true);
        //scoreText.text = val.ToString();
        if (val > currentHighScore)
        {
            currentHighScore = val;
            UtilityTools.updateTextWithPunch(highScoreText, currentHighScore.ToString(), scoreText.transform.localScale / 1.8f, TheGrid.moveTime, true);
        }

        Color32 quinColor = Box.getElementColor(Box.Element.quintessential);

        if (c.r == quinColor.r && c.g == quinColor.g && c.b == quinColor.b)
        {
            isRainbow = true;
        }
        else
        {
            if (isRainbow) isRainbow = false;
        }

        Colorize(c, isRainbow);
    }

    public void UpdateMatch3Uses(int val)
    {
        if (Match3ModeText != null)
        {
            UtilityTools.updateTextWithPunch(Match3ModeText, val.ToString(), Match3ModeText.transform.localScale / 1.8f, TheGrid.moveTime, true);
        }
    }

    public void Colorize(Color32 c, bool rainbow)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);

        s *= Random.Range(0.4f, 0.8f);

        Color32 desaturated = Color.HSVToRGB(h, s, v);

        if (!circles.ToList().Any(x => x.IsForeground() != true)) return;

        CircleBG foreground = circles.First(x => x.IsForeground() == true);
        CircleBG background = circles.First(x => x.IsForeground() == false);

        background.setForeground(true);
        // background.setForeground(false);

        background.Colorize(desaturated, rainbow);
    }

    public void ToggleHelp()
    {
        isHelpOn = !isHelpOn;

        if (helpText == null || helpBG == null) return;

        if (isHelpOn)
        {
            helpText.DOColor(Color.white, TheGrid.moveTime * 3);
            Color black = Color.black;
            black.a = 0.8f;
            helpBG.DOColor(black, TheGrid.moveTime * 3);
        }
        else
        {
            helpText.DOColor(Color.clear, TheGrid.moveTime * 3);
            helpBG.DOColor(Color.clear, TheGrid.moveTime * 3);
        }
    }
}