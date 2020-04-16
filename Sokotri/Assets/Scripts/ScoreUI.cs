using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreTitle;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private Color32 scoreTextColor;

    [SerializeField]
    private Color32 scoreTitleColor;

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    CircleBG []circles = new CircleBG[2];

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
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            UpdateScore(1, Box.getElementColor(Box.Element.fire));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateScore(1, new Color32(231, 76, 60, 255));
        }
    }

    public void UpdateScore(int val, Color32 c)
    {
        if (c != Color.white)
            scoreText.color = c;

        scoreText.text = val.ToString();

        Colorize(c);
    }

    
    public void Colorize(Color32 c)
    {
        float h, s, v;
         Color.RGBToHSV(c,out h, out s,out v);

        s *= Random.Range(0.4f,0.8f);

        Color32 desaturated = Color.HSVToRGB(h, s, v);

        if (!circles.ToList().Any(x => x.IsForeground() != true)) return;

        CircleBG foreground = circles.First(x => x.IsForeground() == true);
        CircleBG background = circles.First(x => x.IsForeground() == false);

        background.setForeground(true);
        // background.setForeground(false);

        background.Colorize(desaturated);
        

    }
}