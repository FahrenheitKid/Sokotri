using DG.Tweening;
using UnityEngine;

public class UIJuiciness : MonoBehaviour
{
    [SerializeField]
    private Vector3 initialScale;

    [SerializeField]
    private Vector3 initialPosition;

    [SerializeField]
    private Vector3 initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.eulerAngles;
    }

    public void DoScale(Vector3 scale, float animTime, bool relative = false)
    {
        transform.DOScale(scale, animTime).SetRelative(relative);
    }

    public void mouseEnterScale()
    {
        DoScale(new Vector3(0.4f, 0.4f, 1), TheGrid.moveTime, true);
    }

    public void mouseExitScale()
    {
        DoScale(initialScale, TheGrid.moveTime);
    }
}