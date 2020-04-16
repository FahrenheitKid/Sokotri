using TMPro;
using UnityEngine;

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
    }
}