using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    

public class Tile : MonoBehaviour
{
public enum Status{
        empty,
        player,
        box
    }

    public enum Kind{
        floor,
        hole,
        wall,
        management,
        spawn


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
    Kind kind;

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

    public void Init(Point p, Color32 c, TheGrid g, Status s = Status.empty, Kind k = Kind.floor)
    {
        name = "Tile( " + p.x + ", " + p.y +")";
        index = p;
        color = c;
        spriteRenderer.color = color;
        status = s;
        kind = k;
        grid = g;

    }

    public void resetPosition()
    {
       transform.position = grid.getExpectedPositionFromPoint(index);
    }


    public Point GetPoint()
    {
        return index;
    }
    public Status GetStatus()
    {
        return status;
    }

    public bool isEmpty()
    {
        return (kind != Kind.hole && kind != Kind.wall && status != Status.box && status != Status.player);
    }

    
    public void setKind(Kind k)
    {
        kind = k;
        if(k == Kind.management)
        {
            color = new Color32(248,174, 255,255);
            spriteRenderer.color = color;
        }
        else if(k == Kind.spawn)
        {
            color = new Color32(196,137, 205,255);
            spriteRenderer.color = color;
        }
        else
        {
            color = Color.white;
            spriteRenderer.color = color;
        }

    }

    public void setStatus(Status s, Box b = null)
    {
        status = s;
        
        switch(status)
        {

            case Status ss when ss == Status.empty:
            box = null;
            break;

            case Status ss when ss == Status.box:
            box = (b!=null) ? b : null;
            break;

            case Status ss when ss == Status.player:
            box = null;
            break;
        }
        

    }

    
}
