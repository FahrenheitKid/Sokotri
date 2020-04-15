using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriBox : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject box_prefab;

    [SerializeField]
    private Box[] boxes = new Box[3];

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private Tile centerTile;

    [SerializeField]
    private bool isVertical;

    private bool isRotating = false;
    private bool isMoving = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();
    }

    public void Init(Tile center, Box.Element[] elements, TheGrid grid = null, bool _isVertical = true)
    {
        if (grid_ref == null && grid != null) grid_ref = grid;

        setVerticalInit(_isVertical);

        for (int i = 0; i < boxes.Length && i < elements.Length; i++)
        {
            boxes[i].setElement(elements[i]);
        }

        setCenterTile(center);
        resetPosition();
    }

    public void setCenterTile(Tile t)
    {
        if (t.GetStatus() != Tile.Status.empty || t.GetKind() == Tile.Kind.wall ||
            t.GetKind() == Tile.Kind.hole || centerTile == t || t == null) return;

        centerTile = t;

        Tile neighbour1 = (isVertical) ? grid_ref.GetTile(centerTile.GetPoint().getNeighbourPoint(UtilityTools.Directions.up)) :
            grid_ref.GetTile(centerTile.GetPoint().getNeighbourPoint(UtilityTools.Directions.left));

        Tile neighbour2 = (isVertical) ? grid_ref.GetTile(centerTile.GetPoint().getNeighbourPoint(UtilityTools.Directions.down)) :
            grid_ref.GetTile(centerTile.GetPoint().getNeighbourPoint(UtilityTools.Directions.right));

        Tile[] tiles = { neighbour1, centerTile, neighbour2 };

        if (System.Array.FindAll(tiles, x => x == null).Any()) return;

        for (int i = 0; i < tiles.Length && i < boxes.Length; i++)
        {
            boxes[i].setTile(tiles[i]);
        }

        rearrangeBoxesArray();
    }

    public Box getCenterBox()
    {
        if (boxes.Length < 3) return null;

        for (int i = 0; i < boxes.Length; i++)
        {
            int count = 0;
            for (int j = 0; j < boxes.Length; j++)
            {
                if (i == j) continue;

                count++;

                if (!boxes[i].isMyNeighbour(boxes[j], false)) break;

                if (count >= 2)
                {
                    return boxes[i];
                }
            }
        }

        return null;
    }

    public bool IsVertical()
    {
        return isVertical;
    }

    public void setVerticalInit(bool v)
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

    public void setVertical(bool v)
    {
        if (v == isVertical) return;

        isVertical = v;
    }

    public void Rotate(bool clockwise)
    {
        //after checkups
        Box center = getCenterBox();
        if (center == null || isRotating || isMoving)
        {
            print("center null");
            return;
        }

        if (!canRotate(clockwise)) return;

        Tile destination1 = null;
        Tile destination2 = null;
        if (isVertical)
        {
            destination1 = (clockwise) ? center.getNeighbourTile(UtilityTools.Directions.right) : center.getNeighbourTile(UtilityTools.Directions.left);
            destination2 = (clockwise) ? center.getNeighbourTile(UtilityTools.Directions.left) : center.getNeighbourTile(UtilityTools.Directions.right);
        }
        else
        {
            destination1 = (clockwise) ? center.getNeighbourTile(UtilityTools.Directions.up) : center.getNeighbourTile(UtilityTools.Directions.down);
            destination2 = (clockwise) ? center.getNeighbourTile(UtilityTools.Directions.down) : center.getNeighbourTile(UtilityTools.Directions.up);
        }

        if (clockwise)
        {
            isRotating = true;
            transform.DORotate(transform.eulerAngles + Vector3.back * 90, TheGrid.moveTime).OnComplete(() => { isRotating = false; getBoxMatches(); });
        }
        else
        {
            isRotating = true;
            transform.DORotate(transform.eulerAngles + Vector3.forward * 90, TheGrid.moveTime).OnComplete(() => { isRotating = false; getBoxMatches(); });
        }

        //print("first box: " + boxes.First() + boxes.First().GetPoint().print() + " is moving to position" + destination1.GetPoint().Clone().print());
        boxes.First().setTile(destination1);
        boxes.Last().setTile(destination2);

        isVertical = !isVertical;
        rearrangeBoxesArray();
    }

    public void resetPosition()
    {
        Vector3 pos = Vector3.zero;
        if (grid_ref != null && getCenterBox() != null)
            pos = grid_ref.getExpectedPositionFromPoint(getCenterBox().GetPoint());

        pos.z = -0.1f;
        transform.position = pos;
    }

    //rearrange arrrays so the left most or topmost is always at index 0
    public void rearrangeBoxesArray()
    {
        /*

        for(int i = 0; i < boxes.Length;i ++)
        {
            print("BEFORE My index is " + i + "and I'm " + boxes[i].name);
        }
        */
        boxes = (!isVertical) ? boxes.OrderBy(bi => bi.GetPoint().x).ToArray() : boxes.OrderBy(bi => bi.GetPoint().y).ToArray();

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].name = "Box " + (i + 1) + boxes[i].GetPoint().print();
            boxes[i].transform.SetSiblingIndex(i);
            //boxes[i].GetComponent<SpriteRenderer> ().color = Color.white;
        }
        //boxes.First().GetComponent<SpriteRenderer>().color = Color.green;
        //boxes.Last().GetComponent<SpriteRenderer>().color = Color.red;

#if TRIBOX_DEBUG
        if (!isVertical)
        {
            //leftmost
            boxes.First (bo => bo.GetPoint ().x == boxes.Min (bi => bi.GetPoint ().x)).GetComponent<SpriteRenderer> ().color = Color.green;

            boxes.First (bo => bo.GetPoint ().x == boxes.Max (bi => bi.GetPoint ().x)).GetComponent<SpriteRenderer> ().color = Color.red;
        }
        else
        {
            boxes.First (bo => bo.GetPoint ().y == boxes.Min (bi => bi.GetPoint ().y)).GetComponent<SpriteRenderer> ().color = Color.green;
            boxes.First (bo => bo.GetPoint ().y == boxes.Max (bi => bi.GetPoint ().y)).GetComponent<SpriteRenderer> ().color = Color.red;
        }

#endif
    }

    public bool isSideEmpty(UtilityTools.Directions dir)
    {
        switch (dir)
        {
            case UtilityTools.Directions.up:
                if (isVertical)
                {
                    if (getDirectionMostBox(dir).getNeighbourTile(dir) == null) return false;

                    return getDirectionMostBox(dir).getNeighbourTile(dir).isEmpty();
                }
                else
                {
                    foreach (Box b in boxes)
                    {
                        Tile neighbour = b.getNeighbourTile(dir);
                        if (neighbour == null) return false;

                        if (!neighbour.isEmpty()) return false;
                    }
                    return true;
                }

                break;

            case UtilityTools.Directions.right:
                if (!isVertical)
                {
                    if (getDirectionMostBox(dir).getNeighbourTile(dir) == null) return false;

                    return getDirectionMostBox(dir).getNeighbourTile(dir).isEmpty();
                }
                else
                {
                    foreach (Box b in boxes)
                    {
                        Tile neighbour = b.getNeighbourTile(dir);
                        if (neighbour == null) return false;

                        if (!neighbour.isEmpty()) return false;
                    }
                    return true;
                }

                break;

            case UtilityTools.Directions.down:
                if (isVertical)
                {
                    if (getDirectionMostBox(dir).getNeighbourTile(dir) == null) return false;

                    return getDirectionMostBox(dir).getNeighbourTile(dir).isEmpty();
                }
                else
                {
                    foreach (Box b in boxes)
                    {
                        Tile neighbour = b.getNeighbourTile(dir);
                        if (neighbour == null) return false;

                        if (!neighbour.isEmpty()) return false;
                    }
                    return true;
                }

                break;

            case UtilityTools.Directions.left:
                if (!isVertical)
                {
                    if (getDirectionMostBox(dir).getNeighbourTile(dir) == null) return false;

                    return getDirectionMostBox(dir).getNeighbourTile(dir).isEmpty();
                }
                else
                {
                    foreach (Box b in boxes)
                    {
                        Tile neighbour = b.getNeighbourTile(dir);
                        if (neighbour == null) return false;

                        if (!neighbour.isEmpty()) return false;
                    }
                    return true;
                }

                break;

            default:
                return false;
                break;
        }
    }

    public bool canMove(UtilityTools.Directions dir)
    {
        return isSideEmpty(dir);
    }

    public void Move(UtilityTools.Directions dir)
    {
        //after checkups

        if (isRotating || isMoving)
        {
            print("already moving/rotating");
            return;
        }

        if (!canMove(dir)) return;

        Tile[] destinations = { null, null, null };

        bool shouldEmptyPreviousTile = false;

        switch (dir)
        {
            case UtilityTools.Directions.up:
                if (isVertical)
                {
                    destinations[0] = getDirectionMostBox(dir).getNeighbourTile(dir); // the uppermost is gonna go to his up neighbour
                    destinations[1] = getDirectionMostBox(dir).GetTile(); // the center is gonna go to current uppermost
                    destinations[2] = getCenterBox().GetTile(); // lowest is going to go to center

                    shouldEmptyPreviousTile = true;
                }
                else
                {
                    for (int i = 0; i < destinations.Length && i < boxes.Length; i++)
                    {
                        destinations[i] = boxes[i].getNeighbourTile(dir);
                    }
                }

                break;

            case UtilityTools.Directions.right:
                if (!isVertical)
                {
                    destinations[0] = getCenterBox().GetTile(); // leftmost is going to go to center
                    destinations[1] = getDirectionMostBox(dir).GetTile(); // the center is gonna go to current rightmost
                    destinations[2] = getDirectionMostBox(dir).getNeighbourTile(dir); // the rightmost is gonna go to his right neighbour
                    shouldEmptyPreviousTile = true;
                }
                else
                {
                    for (int i = 0; i < destinations.Length && i < boxes.Length; i++)
                    {
                        destinations[i] = boxes[i].getNeighbourTile(dir);
                    }
                }

                break;

            case UtilityTools.Directions.down:
                if (isVertical)
                {
                    destinations[0] = getCenterBox().GetTile(); // highest is going to go to center
                    destinations[1] = getDirectionMostBox(dir).GetTile(); // the center is gonna go to current downmost
                    destinations[2] = getDirectionMostBox(dir).getNeighbourTile(dir); // the downmost is gonna go to his down neighbour
                    shouldEmptyPreviousTile = true;
                }
                else
                {
                    for (int i = 0; i < destinations.Length && i < boxes.Length; i++)
                    {
                        destinations[i] = boxes[i].getNeighbourTile(dir);
                    }
                }

                break;

            case UtilityTools.Directions.left:
                if (!isVertical)
                {
                    destinations[0] = getDirectionMostBox(dir).getNeighbourTile(dir); // the leftmost is gonna go to his left neighbour
                    destinations[1] = getDirectionMostBox(dir).GetTile(); // the center is gonna go to current leftmost
                    destinations[2] = getCenterBox().GetTile(); // rightmost is going to go to center
                    shouldEmptyPreviousTile = true;
                }
                else
                {
                    for (int i = 0; i < destinations.Length && i < boxes.Length; i++)
                    {
                        destinations[i] = boxes[i].getNeighbourTile(dir);
                    }
                }

                break;

            default:
                return;
                break;
        }

        // all destinations are ok
        if (!System.Array.FindAll(destinations, x => x == null).Any())
        {
            isMoving = true;

            transform.DOMove(transform.position + UtilityTools.getDirectionVector(dir) * 1, TheGrid.moveTime).OnComplete(() =>
         {
             isMoving = false;
             getBoxMatches();
         });

            for (int i = 0; i < destinations.Length && i < boxes.Length; i++)
            {
                //move sprites

                //update tiles  isVertical && dir == UtilityTools.Directions.down && i > 0
                //fix when moving down it should not set previous tiles as empties
                if (shouldEmptyPreviousTile)
                {
                    boxes[i].setTile(destinations[i], Tile.Status.box);
                }
                else
                {
                    boxes[i].setTile(destinations[i]);
                }
            }
        }
        else
        {
            return;
        }

        //print("first box: " + boxes.First() + boxes.First().GetPoint().print() + " is moving to position" + destination1.GetPoint().Clone().print());
        rearrangeBoxesArray();
    }

    public Box getDirectionMostBox(UtilityTools.Directions dir)
    {
        foreach (Box b in boxes)
        {
            if (b.isDirectionMost(dir)) return b;
        }

        return null;
    }

    public bool canRotate(bool clockwise)
    {
        Box center = getCenterBox();
        if (center == null || isRotating || isMoving)
        {
            print("center null");
            return false;
        }
        List<UtilityTools.Directions> directionsCheckList = new List<UtilityTools.Directions>();

        // in relation to CenterBox
        if (isVertical && clockwise)
        {
            //check upright right downleft left
            directionsCheckList.Add(UtilityTools.Directions.upRight);
            directionsCheckList.Add(UtilityTools.Directions.right);
            directionsCheckList.Add(UtilityTools.Directions.downLeft);
            directionsCheckList.Add(UtilityTools.Directions.left);
        }
        else if (isVertical && !clockwise)
        {
            //check upleft left downright right
            directionsCheckList.Add(UtilityTools.Directions.upLeft);
            directionsCheckList.Add(UtilityTools.Directions.left);
            directionsCheckList.Add(UtilityTools.Directions.downRight);
            directionsCheckList.Add(UtilityTools.Directions.right);
        }
        else if (!isVertical && clockwise)
        {
            //check upleft up downright down
            directionsCheckList.Add(UtilityTools.Directions.upLeft);
            directionsCheckList.Add(UtilityTools.Directions.up);
            directionsCheckList.Add(UtilityTools.Directions.downRight);
            directionsCheckList.Add(UtilityTools.Directions.down);
        }
        else
        {
            //check downleft down upRight up

            directionsCheckList.Add(UtilityTools.Directions.downLeft);
            directionsCheckList.Add(UtilityTools.Directions.down);
            directionsCheckList.Add(UtilityTools.Directions.upRight);
            directionsCheckList.Add(UtilityTools.Directions.up);
        }

        if (directionsCheckList.Any())
        {
            if (!center.areMyNeighboursEmpty(directionsCheckList))
            {
                print("something is blocking the rotation! Clock=" + clockwise);
                return false;
            }
        }

        return true;
    }

    public List<List<Box>> getBoxMatches()
    {
        List<List<Box>> matches = new List<List<Box>>();
        List<Box> test = new List<Box>();

        if (!isNextToBox()) return matches;

        for (int i = 0; i < boxes.Length; i++)
        {
            foreach (List<Box> b in boxes[i].getAllConnectedMatches())
            {
                if (b.Any())
                    matches.Add(b);
            }
            //matches.Add(boxes[i].getConnectedMatches(UtilityTools.Directions.right));
            // matches.Add(boxes[i].getConnectedMatches(UtilityTools.Directions.up));
        }

        matches.RemoveAll(x => x.Count < TheGrid.matchSize);

        // removes dublicates matches list
        matches = matches.Distinct(new ListEqualityComparer<Box>(new BoxElementEqualityComparer())).ToList();

        bool hasMatches = matches.Any();

        int uniqueElementMatches = 0;
        uniqueElementMatches = matches.SelectMany(x => x).Distinct() // getthe lists of lists in a single list
            .GroupBy(y => y.GetElement()).Select(z => z.First() && z.Count() >= TheGrid.matchSize).Count(); // then distinctBy a specific field and get how many are there

        if (matches.FindAll(x => x.FindAll(b => b.GetElement() == Box.Element.quintessential).Any()).Any()) uniqueElementMatches--;
        if (hasMatches)
        {
            print("There are" + uniqueElementMatches + " different types of matches");
            foreach (List<Box> lb in matches)
            {
                if (lb.Count >= TheGrid.matchSize)
                {
                    foreach (Box b in lb)
                    {
                        print(b.name + "| " + b.GetElement());
                    }
                }
            }
        }

        return matches;
    }

    public bool isNextToBox(bool onlyTriboxes = false)
    {
        Box b = getCenterBox();
        bool firstMostNeighbour = false;
        bool secondMostNeighbour = false;
        List<UtilityTools.Directions> dirList = new List<UtilityTools.Directions>();

        if (onlyTriboxes == false)
        {
            if (isVertical)
            {
                dirList.Add(UtilityTools.Directions.left);
                dirList.Add(UtilityTools.Directions.right);
                dirList.AddRange(UtilityTools.diagonals);

                //also need to check if neighbour above/below edges are a box
                firstMostNeighbour = getDirectionMostBox(UtilityTools.Directions.up).isMyNeighbourThisStatus(UtilityTools.Directions.up, Tile.Status.box);
                secondMostNeighbour = getDirectionMostBox(UtilityTools.Directions.down).isMyNeighbourThisStatus(UtilityTools.Directions.down, Tile.Status.box);
                Tile neighbourUp = getDirectionMostBox(UtilityTools.Directions.up).getNeighbourTile(UtilityTools.Directions.up);
                Tile neighbourDown = getDirectionMostBox(UtilityTools.Directions.down).getNeighbourTile(UtilityTools.Directions.down);
            }
            else
            {
                dirList.Add(UtilityTools.Directions.up);
                dirList.Add(UtilityTools.Directions.down);
                dirList.AddRange(UtilityTools.diagonals);

                //also need to check if neighbour leftright edges are a box
                firstMostNeighbour = getDirectionMostBox(UtilityTools.Directions.left).isMyNeighbourThisStatus(UtilityTools.Directions.left, Tile.Status.box);
                secondMostNeighbour = getDirectionMostBox(UtilityTools.Directions.right).isMyNeighbourThisStatus(UtilityTools.Directions.right, Tile.Status.box);
            }

            return (b.haveAnyNeighbourThisStatus(dirList, Tile.Status.box) || firstMostNeighbour || secondMostNeighbour);
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Box[] GetBoxes()
    {
        return boxes;
    }
}