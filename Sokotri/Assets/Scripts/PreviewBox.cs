using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PreviewBox : MonoBehaviour
{
    [SerializeField]
    private float boxAnimationTime = 1.0f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Sequence vanish()
    {
        Sequence vanish = DOTween.Sequence();

        vanish.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1), boxAnimationTime / 2));
        vanish.Append(transform.DOScale(new Vector3(0, 0, 1), boxAnimationTime / 2));

        return vanish;
    }

    public void Vanish(Sprite newSprite, Box.Element el)
    {
        Sequence vanish = DOTween.Sequence();

        vanish.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1), boxAnimationTime / 2));
        vanish.Append(transform.DOScale(new Vector3(0, 0, 1), boxAnimationTime / 2));

        // vanish.OnStart(() => { print("startii"); });

        //vanish.OnKill(() => {
        //    GetComponent<Image>().sprite = newSprite;
        //    GetComponent<Image>().color = (el == Box.Element.quintessential) ? Box.quintessentialColor : (Color32)Color.white;
        //    appear();
        //    print("killed"); });

        vanish.OnComplete(() =>
        {
            GetComponent<Image>().sprite = newSprite;
            GetComponent<Image>().color = (el == Box.Element.quintessential) ? Box.quintessentialColor : (Color32)Color.white;

            appear();
            // print("completed"); });
        });
    }

    public Sequence appear()
    {
        Sequence appear = DOTween.Sequence();

        appear.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1), boxAnimationTime));
        appear.Join(transform.DORotate(new Vector3(0, 0, 360), boxAnimationTime, RotateMode.FastBeyond360));

        return appear;
    }
}