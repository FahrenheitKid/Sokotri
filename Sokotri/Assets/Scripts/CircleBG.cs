using DG.Tweening;
using UnityEngine;

public class CircleBG : MonoBehaviour
{
    [SerializeField]
    private bool isForeground = false;

    [SerializeField]
    public float animationTime = 2f;

    [SerializeField]
    private CircleBG otherCircle;

    private Tween rainbowTween;

    [SerializeField]
    private float rainbowHue;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (rainbowTween != null)
        {
            if (rainbowTween.IsPlaying())
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                float h, s, v;
                Color.RGBToHSV(Box.getElementColor(Box.Element.quintessential), out h, out s, out v);
                s *= 0.5f;
                sr.color = Color.HSVToRGB(rainbowHue, s, v * 1.5f);
            }
        }
    }

    public void Colorize(Color32 c, bool rainbow = false)
    {
        if (!rainbow)
        {
            if (rainbowTween != null)
            {
                rainbowTween.Kill();
            }
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null || !isForeground) return;

        setForeground(true);
        sr.color = c;

        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);

        transform.DOScale(new Vector3(10, 10, 1), animationTime).OnStart(() => { otherCircle.setForeground(false); });
        if (rainbow)
        {
            rainbowHue = 0;

            rainbowTween = DOTween.To(() => rainbowHue, x => rainbowHue = x, 1f, 10f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            rainbowTween.Play();
        }
    }

    public void setForeground(bool b)
    {
        if (b == isForeground) return;

        isForeground = b;

        if (isForeground == false)
        {
            //transform.localScale = new Vector3(0, 0, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y, 2);
        }
        else
        {
            transform.localScale = Vector3.zero;
            transform.position = new Vector3(transform.position.x, transform.position.y, 1);

            //  otherCircle.setForeground(false);
        }
    }

    public bool IsForeground()
    {
        return isForeground;
    }
}