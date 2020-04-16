using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum Status
    {
        empty,
        player,
        box
    }

    public enum Kind
    {
        floor,
        hole,
        wall,
        management,
        spawn
    }

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color32 color;

    [SerializeField]
    private TheGrid grid;

    [SerializeField]
    private Box box;

    [SerializeField]
    private Status status;

    [SerializeField]
    private Kind kind;

    [SerializeField]
    private Point index;

    // Start is called before the first frame update
    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Init(Point p, Color32 c, TheGrid g, Status s = Status.empty, Kind k = Kind.floor)
    {
        name = "Tile( " + p.x + ", " + p.y + ")";
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

    public Box GetBox()
    {
        return box;
    }

    public bool isEmpty()
    {
        return (kind != Kind.hole && kind != Kind.wall && status != Status.box && status != Status.player);
    }

    public void setKind(Kind k)
    {
        kind = k;
        if (k == Kind.management)
        {
            color = new Color32(248, 174, 255, 255);
            spriteRenderer.color = color;
        }
        else if (k == Kind.spawn)
        {
            color = new Color32(196, 137, 205, 255);
            spriteRenderer.color = color;
        }
        else
        {
            color = Color.white;
            spriteRenderer.color = color;
        }
    }

    //check if this tile and neighbours are of a specific type and status
    public bool AreMeAndNeighbours(Kind k, Status s, bool vertical, bool horizontal)
    {
        Point neighbour1 = Point.zero;
        Point neighbour2 = Point.zero;

        if (kind != k || s != status) return false;

        if (vertical)
        {
            neighbour1 = index.getNeighbourPoint(UtilityTools.Directions.up);
            neighbour2 = index.getNeighbourPoint(UtilityTools.Directions.down);

            if (grid.isPointOutOfBounds(neighbour1) || grid.isPointOutOfBounds(neighbour2)) return false;

            if (grid.GetTile(neighbour1).GetKind() != k || grid.GetTile(neighbour2).GetKind() != k)
            {
                return false;
            }
        }

        if (horizontal)
        {
            neighbour1 = index.getNeighbourPoint(UtilityTools.Directions.left);
            neighbour2 = index.getNeighbourPoint(UtilityTools.Directions.right);

            if (grid.isPointOutOfBounds(neighbour1) || grid.isPointOutOfBounds(neighbour2)) return false;

            if (grid.GetTile(neighbour1).GetKind() != k || grid.GetTile(neighbour2).GetKind() != k)
            {
                return false;
            }
        }

        return true;
    }

    public Kind GetKind()
    {
        return kind;
    }

    public void setStatus(Status s, Box b = null)
    {
        status = s;

        switch (status)
        {
            case Status ss when ss == Status.empty:
                box = null;
                break;

            case Status ss when ss == Status.box:
                box = (b != null) ? b : null;
                break;

            case Status ss when ss == Status.player:
                box = null;
                break;
        }
    }

    public bool isMatchable()
    {
        return status == Status.box && kind == Kind.floor;
    }
}