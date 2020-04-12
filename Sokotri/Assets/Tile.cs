using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    

public class Tile : MonoBehaviour
{
public enum Status{
        empty,
        wall,
        hole,
        management,
        player,
        box
    }

    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Color32 color;

    [SerializeField]
    TheGrid grid;
    [SerializeField]
    Box box;
    [SerializeField]
    Status status;

    [SerializeField]
    Point index;


    // Start is called before the first frame update
    void Start()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Init(Point p, Color32 c, TheGrid g, Status s = Status.empty)
    {
        name = "Tile( " + p.x + ", " + p.y +")";
        index = p;
        color = c;
        spriteRenderer.color = color;
        status = s;
        grid = g;
    }

    public void resetPosition()
    {
       transform.position = grid.getExpectedPositionFromPoint(index);
    }



    public Status GetStatus()
    {
        return status;
    }
    public void setStatus(Status s)
    {
        status = s;
        if(s == Status.management)
        {
            color = new Color32(248,174, 255,255);
            spriteRenderer.color = color;
        }
    }

    
}
