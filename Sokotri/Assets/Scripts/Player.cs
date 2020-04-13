using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    TheGrid grid_ref;
    [SerializeField]
    Point index = Point.zero;
    [SerializeField]
    Point lastIndex;
    [SerializeField]
    Tile tile;
    [SerializeField]
    bool isMoving = false;
    [SerializeField]
    float moveTime = 0.5f;

    // Start is called before the first frame update
    void Start ()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag ("Grid").GetComponent<TheGrid> ();

    }

    // Update is called once per frame
    void Update ()
    {
        Move (getInputDirection ());

    }

    public void Init (Point spawn, TheGrid grid = null)
    {
        if (grid_ref == null && grid != null) grid_ref = grid;

        index = spawn;

        SpawnAt (spawn);

    }

    public void SpawnAt (Point p)
    {
        if (grid_ref.isPointOutOfBounds (p))
        {
            return;
        }

        if (grid_ref.GetTile (p) == null)
        {

            return;
        }

        if (!grid_ref.GetTile (p).isEmpty ()) return;

        index = p.Clone ();
        tile = grid_ref.GetTile (p);
        grid_ref.GetTile (p).setStatus (Tile.Status.player);
        resetPosition ();
    }
    bool Move (UtilityTools.Directions dir)
    {
        if (isMoving == true)
        {

            return false;
        }
        else if (isMoving == false && checkDestination (dir)) //move only if not already moving and if destination is available
        {
            //thren update player's and tile's new values/statuses
            isMoving = true;
            Tile destination = grid_ref.GetTile (index.getNeighbourPoint (dir));
            if (destination == null) return false;
            Vector3 destinationPos = destination.transform.position;
            destinationPos.z = -1;
            transform.DOMove (destinationPos, moveTime).OnComplete (() =>
            {
                isMoving = false;
                tile.setStatus (Tile.Status.empty);
                destination.setStatus (Tile.Status.player);
                tile = destination;
                lastIndex.Copy (index);
                setIndex (destination.GetPoint ().Clone ());

            });
            return true;
        }
        else
        {
            return false;
        }

    }

    bool checkDestination (UtilityTools.Directions dir)
    {
        if (dir == UtilityTools.Directions.upLeft || dir == UtilityTools.Directions.upRight ||
            dir == UtilityTools.Directions.downRight || dir == UtilityTools.Directions.downLeft ||
            grid_ref.GetTile (index.getNeighbourPoint (dir)) == null)
        {
            return false;
        }
        else
        {

            if (grid_ref.GetTile (index.getNeighbourPoint (dir)).isEmpty ())
            {
                //print("destination is empty: " + index.getNeighbourPoint (dir).print());
                return true;
            }
            else
            {
                /*
                switch (dir)
                {
                    case UtilityTools.Directions.up:

                        break;

                    case UtilityTools.Directions.upRight:

                        break;

                    case UtilityTools.Directions.right:

                        break;

                    case UtilityTools.Directions.downRight:

                        break;

                    case UtilityTools.Directions.down:

                        break;

                    case UtilityTools.Directions.downLeft:
                        break;

                    case UtilityTools.Directions.left:
                        break;

                    case UtilityTools.Directions.upLeft:
                        break;

                }
                */
                return false;

            }
        }

    }

    public void setIndex (Point p)
    {
        index.Copy (p);

    }
    public void resetPosition ()
    {
        transform.position = grid_ref.getExpectedPositionFromPoint (index);
        Vector3 pos = transform.position;
        pos.z = -1;
        transform.position = pos;
    }

    UtilityTools.Directions getInputDirection ()
    {
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

        if (input.magnitude < 1 || input == Vector2.zero || input.magnitude > 1)
        {
            return UtilityTools.Directions.upRight;
        }

        else
        {

            switch (input)
            {
                case Vector2 v when v.x >= 1 && v.y == 0: // right
                    //(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.right;
                    break;

                case Vector2 v when v.x <= -1 && v.y == 0: // left
                    // print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.left;
                    break;

                case Vector2 v when v.x == 0 && v.y >= 1: // up
                    // print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.up;
                    break;

                case Vector2 v when v.x == 0 && v.y <= -1: // down
                    //print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.down;
                    break;

                default:
                    return UtilityTools.Directions.upRight;
                    break;

            }
        }

    }
}