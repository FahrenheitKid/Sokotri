using DG.Tweening;
using UnityEngine;

public class PreviewBoxes : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites = new Sprite[6];

    private SpriteRenderer spriteRenderer; // Start is called before the first frame update

    private Box.Element[] boxes = new Box.Element[3];

    [SerializeField]
    private GameObject[] boxesRenderers = new GameObject[3];

    [SerializeField]
    private float boxAnimationTime = 1.0f;

    private Sequence vanish;
    private Sequence appear;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Sequence vanish = DOTween.Sequence();
        Sequence appear = DOTween.Sequence();
        generateNextPreview();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void updateSprites()
    {
        for (int i = 0; i < boxesRenderers.Length; i++)
        {
            if ((int)boxes[i] < 0 || (int)boxes[i] >= sprites.Length || i < 0 || i >= boxes.Length) continue;

            PreviewBox pb = boxesRenderers[i].GetComponent<PreviewBox>();
            if (pb)
            {
                pb.Vanish(sprites[(int)boxes[i]], boxes[i]);
            }

            //boxesRenderers[i].GetComponent<Image>().sprite = sprites[(int)boxes[i]];
            //boxesRenderers[i].GetComponent<Image>().color = (boxes[i] == Box.Element.quintessential) ? Box.quintessentialColor : (Color32)Color.white;
            //print("completou");
        }
    }

    public void generateNextPreview()
    {
        boxes = Box.getRandomBoxElements(boxes.Length);

        updateSprites();
    }

    public Box.Element[] getCurrentBoxElements()
    {
        return boxes;
    }
}