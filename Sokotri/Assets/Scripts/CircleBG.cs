using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleBG : MonoBehaviour
{
    [SerializeField]
     bool isForeground = false;
    [SerializeField]
    public float animationTime = 2f;

    [SerializeField]
    CircleBG otherCircle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Colorize(Color32 c)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null || !isForeground) return;

        setForeground(true);
        sr.color = c;

        transform.DOScale(new Vector3(10, 10, 1), animationTime).OnStart(() => { otherCircle.setForeground(false); });

    }

    public void setForeground(bool b)
    {
        if (b == isForeground) return;

        isForeground = b;



        if(isForeground == false)
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
