using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriBox : MonoBehaviour
{

    [Header ("Prefabs")]
    [SerializeField]
    GameObject box_prefab;

    [SerializeField]
    Box[] boxes = new Box[3];
    [SerializeField]
    TheGrid grid_ref;

    [SerializeField]
    Tile centerTile;

    [SerializeField]
    bool isVertical;

    // Start is called before the first frame update
    void Start ()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag ("Grid").GetComponent<TheGrid> ();
    }

    public void Init (Tile center, Box.Element[] elements, TheGrid grid = null, bool _isVertical = true)
    {
        if (grid_ref == null && grid != null) grid_ref = grid;

        setVertical (_isVertical);

        for (int i = 0; i < boxes.Length && i < elements.Length; i++)
        {
            boxes[i].setElement (elements[i]);
        }

        setCenterTile (center);
        resetPosition ();

    }

    public void setCenterTile (Tile t)
    {
        if (t.GetStatus () != Tile.Status.empty || t.GetKind () == Tile.Kind.wall ||
            t.GetKind () == Tile.Kind.hole || centerTile == t || t == null) return;

        centerTile = t;

        Tile neighbour1 = (isVertical) ? grid_ref.GetTile (centerTile.GetPoint ().getNeighbourPoint (UtilityTools.Directions.up)) :
            grid_ref.GetTile (centerTile.GetPoint ().getNeighbourPoint (UtilityTools.Directions.left));

        Tile neighbour2 = (isVertical) ? grid_ref.GetTile (centerTile.GetPoint ().getNeighbourPoint (UtilityTools.Directions.down)) :
            grid_ref.GetTile (centerTile.GetPoint ().getNeighbourPoint (UtilityTools.Directions.right));

        Tile[] tiles = { neighbour1, centerTile, neighbour2 };

        for (int i = 0; i < tiles.Length && i < boxes.Length; i++)
        {
            boxes[i].setTile (tiles[i]);
        }

    }

    public Box getCenterBox ()
    {
        if (boxes.Length < 3) return null;

        return boxes[2];
    }
    public bool IsVertical ()
    {
        return isVertical;
    }

    public void setVertical (bool v)
    {
        if (v == isVertical) return;

        isVertical = v;
        if (isVertical)
        {
            boxes[0].transform.localPosition = Vector3.up;
            boxes[1].transform.localPosition = Vector3.zero;
            boxes[2].transform.localPosition = Vector3.down;
        }
        else
        {
            boxes[0].transform.localPosition = Vector3.left;
            boxes[1].transform.localPosition = Vector3.zero;
            boxes[2].transform.localPosition = Vector3.right;
        }

    }

    public void resetPosition ()
    {
        Vector3 pos = Vector3.zero;
        if (grid_ref != null && centerTile != null)
            pos = grid_ref.getExpectedPositionFromPoint (centerTile.GetPoint ());

        pos.z = -0.1f;
        transform.position = pos;

    }
    // Update is called once per frame
    void Update ()
    {

    }

    public Box[] GetBoxes ()
    {
        return boxes;
    }
}