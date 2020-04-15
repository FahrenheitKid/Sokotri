using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class BoxElementEqualityComparer : IEqualityComparer<Box>
{
    public bool Equals(Box b1, Box b2)
    {
        if (b2 == null && b1 == null)
            return true;
        else if (b1 == null || b2 == null)
            return false;
        else if (b1.GetElement() == b2.GetElement())
            return true;
        else
            return false;
    }

    public int GetHashCode(Box bx)
    {
        // int hCode = ;
        //return hCode.GetHashCode();
        return (bx.GetElement(), bx.GetPoint(), bx.GetTile()).GetHashCode();
    }
}

public class Box : MonoBehaviour
{
    public enum Element
    {
        ground, //brown
        fire, // red
        water, // blue
        grass, //green
        steel, //gray
        quintessential //special
    }

    [SerializeField]
    public Sprite[] elementSprites = new Sprite[6];

    [SerializeField]
    private Element element;

    [SerializeField]
    private Point index;

    [SerializeField]
    private Tile tile;

    [SerializeField]
    private TriBox tribox;

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public static Color32 quintessentialColor = Color.magenta;

    //probability of each type of box
    private static float[] typeWeights = { 18, 18, 18, 18, 18, 100 };

    // Start is called before the first frame update
    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public bool isCenterBox()
    {
        if (tribox == null) return false;

        return (this != tribox.GetBoxes().First() && this != tribox.GetBoxes().Last());
    }

    public bool isBoxAtEdge(UtilityTools.Directions dir)
    {
        if (tribox == null || isCenterBox()) return false;

        //tribox.rearrangeBoxesArray();

        return (this == tribox.GetBoxes().First() || this == tribox.GetBoxes().Last());
    }

    //check if this box is at the direction-most position of tribox
    public bool isDirectionMost(UtilityTools.Directions dir)
    {
        if (!isBoxAtEdge(dir)) return false;

        //tribox.rearrangeBoxesArray();

        switch (dir)
        {
            case UtilityTools.Directions.up:
                if (tribox.IsVertical())
                {
                    return this == tribox.GetBoxes().First();
                }
                else
                {
                    return false;
                }

                break;

            case UtilityTools.Directions.right:
                if (!tribox.IsVertical())
                {
                    return this == tribox.GetBoxes().Last();
                }
                else
                {
                    return false;
                }

                break;

            case UtilityTools.Directions.down:
                if (tribox.IsVertical())
                {
                    return this == tribox.GetBoxes().Last();
                }
                else
                {
                    return false;
                }

                break;

            case UtilityTools.Directions.left:
                if (!tribox.IsVertical())
                {
                    return this == tribox.GetBoxes().First();
                }
                else
                {
                    return false;
                }

                break;

            default:
                return false;
                break;
        }
    }

    public bool isMyNeighbour(Box b, bool diagonals)
    {
        if (b == null) return false;
        return index.isMyNeighbour(b.GetPoint().Clone(), diagonals);
    }

    public bool isMyNeighbour(Point p, bool diagonals)
    {
        if (grid_ref.isPointOutOfBounds(p.Clone())) return false;

        return index.isMyNeighbour(p.Clone(), diagonals);
    }

    public bool areMyNeighbours(List<Box> boxList, bool diagonals)
    {
        if (!boxList.Any() || boxList.FindAll(x => x == null).Any()) return false;

        foreach (Box b in boxList)
        {
            if (!isMyNeighbour(b, diagonals)) return false;
        }

        return true;
    }

    public bool areMyNeighbours(List<Point> pointList, bool diagonals)
    {
        if (!pointList.Any() || pointList.FindAll(x => x == null).Any() ||
            pointList.FindAll(x => grid_ref.isPointOutOfBounds(x.Clone())).Any()) return false;

        foreach (Point p in pointList)
        {
            if (!isMyNeighbour(p, diagonals)) return false;
        }

        return true;
    }

    public bool isMyNeighbourEmpty(UtilityTools.Directions dir)
    {
        Tile neighbour = getNeighbourTile(dir);
        return (neighbour == null) ? false : neighbour.isEmpty();
    }

    public bool areMyNeighboursEmpty(List<UtilityTools.Directions> dirList)
    {
        if (!dirList.Any()) return false;

        foreach (UtilityTools.Directions d in dirList)
        {
            if (!isMyNeighbourEmpty(d)) return false;
        }
        return true;
    }

    public bool isMyNeighbourThisKind(UtilityTools.Directions dir, Tile.Kind k)
    {
        if (getNeighbourTile(dir) == null) return false;

        return (getNeighbourTile(dir).GetKind() == k);
    }

    public bool isMyNeighbourThisStatus(UtilityTools.Directions dir, Tile.Status s)
    {
        if (getNeighbourTile(dir) == null) return false;

        return (getNeighbourTile(dir).GetStatus() == s);
    }

    public bool areMyNeighboursThisKind(List<UtilityTools.Directions> dirList, Tile.Kind k)
    {
        if (!dirList.Any()) return false;
        foreach (UtilityTools.Directions d in dirList)
        {
            if (isMyNeighbourThisKind(d, k) == false) return false;
        }
        return true;
    }

    public bool areMyNeighboursThisStatus(List<UtilityTools.Directions> dirList, Tile.Status s)
    {
        if (!dirList.Any()) return false;
        foreach (UtilityTools.Directions d in dirList)
        {
            if (isMyNeighbourThisStatus(d, s) == false) return false;
        }
        return true;
    }

    public bool haveAnyNeighbourThisKind(List<UtilityTools.Directions> dirList, Tile.Kind k)
    {
        if (!dirList.Any()) return false;

        foreach (UtilityTools.Directions d in dirList)
        {
            if (isMyNeighbourThisKind(d, k) == true) return true;
        }

        return false;
    }

    public bool haveAnyNeighbourThisStatus(List<UtilityTools.Directions> dirList, Tile.Status s)
    {
        if (!dirList.Any()) return false;

        foreach (UtilityTools.Directions d in dirList)
        {
            if (isMyNeighbourThisStatus(d, s) == true) return true;
        }
        return false;
    }

    public List<List<Box>> getAllConnectedMatches(bool diagonals = false)
    {
        List<List<Box>> matches = new List<List<Box>>();
        //getConnectedMatches(dir, matches);

        if (diagonals == false)
        {
            matches.Add(getConnectedMatches(UtilityTools.Directions.right));
            matches.Add(getConnectedMatches(UtilityTools.Directions.up));
        }

        return matches;
    }

    public List<Box> getConnectedMatches(UtilityTools.Directions dir)
    {
        Box.Element matchElement = this.element;
        List<Box> result = new List<Box>();
        Tile t = getNeighbourTile(dir);

        if (t == null) return result;

        if (UtilityTools.horizontals.Contains(dir))
        {
            //get all connected ones left and right
            List<Box> mixedRight = (getConnectedBoxes(UtilityTools.Directions.right));
            List<Box> mixedLeft = (getConnectedBoxes(UtilityTools.Directions.left));

            if (this.element == Element.quintessential)
            {
                Element leftElement = Element.quintessential;
                Element rightElement = Element.quintessential;
                Box neigh = getNextNonElementNeighbour(UtilityTools.Directions.right, this.element);
                if (neigh != null)
                {
                    rightElement = neigh.GetElement();
                }

                neigh = getNextNonElementNeighbour(UtilityTools.Directions.left, this.element);

                if (neigh != null)
                {
                    leftElement = neigh.GetElement();
                }

                //if different, need to check for matches at the left and at the right
                if (leftElement != rightElement)
                {
                    mixedLeft = mixedLeft.Distinct().OrderBy(x => x.GetPoint().x).ToList();
                    mixedRight = mixedRight.Distinct().OrderBy(x => x.GetPoint().x).ToList();

                    mixedLeft.RemoveAll(x => x.GetElement() != leftElement && x.GetElement() != Box.Element.quintessential);
                    mixedRight.RemoveAll(x => x.GetElement() != rightElement && x.GetElement() != Box.Element.quintessential);

                    bool leftPass = false;
                    bool rightPass = false;
                    if (areAllConnected(dir, mixedLeft) && mixedLeft.Count >= TheGrid.matchSize)
                    {
                        result.AddRange(mixedLeft);
                        leftPass = true;
                    }
                    if (areAllConnected(dir, mixedRight) && mixedRight.Count >= TheGrid.matchSize)
                    {
                        rightPass = true;
                    }

                    if (rightPass)
                    {
                        if ((leftPass && mixedRight.Count > mixedLeft.Count) || leftPass == false)
                        {
                            result.AddRange(mixedRight);
                        }
                    }
                    else if (leftPass)
                    {
                        if ((rightPass && mixedRight.Count < mixedLeft.Count) || rightPass == false)
                        {
                            result.AddRange(mixedRight);
                        }
                    }

                    return result;
                }
                else
                {
                    matchElement = rightElement;
                }
            }

            result.AddRange(mixedLeft.Distinct());
            result.AddRange(mixedRight.Distinct());

            //order them by left to right
            result = result.Distinct().OrderBy(x => x.GetPoint().x).ToList();

            //remove all elements that arent a match (itself or quint)
            result.RemoveAll(x => x.GetElement() != matchElement && x.GetElement() != Box.Element.quintessential);

            //matches!!!
            if (!areAllConnected(dir, result) || result.Count < TheGrid.matchSize)
            {
                result.Clear();
            }

            return result;
        }
        else if (UtilityTools.verticals.Contains(dir))
        {
            //get all connected ones left and right
            List<Box> mixedUp = (getConnectedBoxes(UtilityTools.Directions.up));
            List<Box> mixedDown = (getConnectedBoxes(UtilityTools.Directions.down));

            if (this.element == Element.quintessential)
            {
                Element downElement = Element.quintessential;
                Element upElement = Element.quintessential;
                Box neigh = getNextNonElementNeighbour(UtilityTools.Directions.up, this.element);
                if (neigh != null)
                {
                    upElement = neigh.GetElement();
                }

                neigh = getNextNonElementNeighbour(UtilityTools.Directions.down, this.element);

                if (neigh != null)
                {
                    downElement = neigh.GetElement();
                }

                //if different, need to check for matches at the left and at the right
                if (downElement != upElement)
                {
                    mixedDown = mixedDown.Distinct().OrderBy(x => x.GetPoint().y).ToList();
                    mixedUp = mixedUp.Distinct().OrderBy(x => x.GetPoint().y).ToList();

                    mixedDown.RemoveAll(x => x.GetElement() != downElement && x.GetElement() != Box.Element.quintessential);
                    mixedUp.RemoveAll(x => x.GetElement() != upElement && x.GetElement() != Box.Element.quintessential);

                    bool downPass = false;
                    bool upPass = false;
                    if (areAllConnected(dir, mixedDown) && mixedDown.Count >= TheGrid.matchSize)
                    {
                        result.AddRange(mixedDown);
                        downPass = true;
                    }
                    if (areAllConnected(dir, mixedUp) && mixedUp.Count >= TheGrid.matchSize)
                    {
                        upPass = true;
                    }

                    if (upPass)
                    {
                        if ((downPass && mixedUp.Count > mixedDown.Count) || downPass == false)
                        {
                            result.AddRange(mixedUp);
                        }
                    }
                    else if (downPass)
                    {
                        if ((upPass && mixedUp.Count < mixedDown.Count) || upPass == false)
                        {
                            result.AddRange(mixedUp);
                        }
                    }

                    return result;
                }
            }

            result.AddRange(mixedDown.Distinct());
            result.AddRange(mixedUp.Distinct());

            //order them by left to right
            result = result.Distinct().OrderBy(x => x.GetPoint().y).ToList();

            //remove all elements that arent a match (itself or quint)
            result.RemoveAll(x => x.GetElement() != this.element && x.GetElement() != Box.Element.quintessential);

            //matches!!!
            if (!areAllConnected(dir, result) || result.Count < TheGrid.matchSize)
            {
                result.Clear();
            }

            return result;
        }
        else
        {
            return result;
        }
    }

    public List<Box> getConnectedBoxes(UtilityTools.Directions dir)
    {
        List<Box> connectedBoxes = new List<Box>();
        _getConnectedBoxes(dir, connectedBoxes);

        return connectedBoxes.Distinct().ToList();
    }

    private void _getConnectedBoxes(UtilityTools.Directions dir, List<Box> connected)
    {
        Tile t = getNeighbourTile(dir);

        if (t == null) return;

        if (t.GetBox() != null && t.GetStatus() == Tile.Status.box)
        {
            if (!connected.Contains(t.GetBox()))
            {
                connected.Add(t.GetBox());
                if (!connected.Contains(this))
                {
                    connected.Add(this);
                }
            }

            t.GetBox()._getConnectedBoxes(dir, connected);
        }
    }

    //return the next connected neighbour in that direction that is NOT that element
    public Box getNextNonElementNeighbour(UtilityTools.Directions dir, Element el)
    {
        Tile t = getNeighbourTile(dir);

        if (t == null) return null;

        if (t.GetBox() != null && t.GetStatus() == Tile.Status.box)
        {
            if (t.GetBox().GetElement() != el) return t.GetBox();
            else getNextNonElementNeighbour(dir, el);
        }

        return null;
    }

    public Point GetPoint()
    {
        return index;
    }

    public void setElement(Element e)
    {
        if (e == element) return;

        element = e;
        if ((int)e > 0 && (int)e < elementSprites.Length)
        {
            if (elementSprites[(int)e] != null)
                spriteRenderer.sprite = elementSprites[(int)e];
        }

        if (element == Element.quintessential)
        {
            spriteRenderer.color = quintessentialColor;
        }
    }

    public void setTile(Tile t, Tile.Status oldTileNewStatus = Tile.Status.empty)
    {
        if (t == tile) return;

        if (oldTileNewStatus == Tile.Status.empty && tile != null)
            tile.setStatus(oldTileNewStatus);

        t.setStatus(Tile.Status.box, this);
        index = t.GetPoint().Clone();

        tile = t;

        if (t.GetStatus() == Tile.Status.empty)
        {
            print("teste");
        }
    }

    public Tile GetTile()
    {
        return tile;
    }

    public Element GetElement()
    {
        return element;
    }

    public bool isInTribox()
    {
        return (tribox != null);
    }

    public TriBox GetTriBox()
    {
        return tribox;
    }

    public void Push(UtilityTools.Directions dir)
    {
        if (!isInTribox()) return;

        bool clockw = true;
        TriBox tri = tribox;
        switch (dir)
        {
            case UtilityTools.Directions.up:
                if (tri.IsVertical())
                {
                    //if im the lowest box
                    if (isDirectionMost(UtilityTools.OppositeDirection(dir)))
                    {
                        //if the upside is empty
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                }
                else
                {
                    if (isCenterBox())
                    {
                        //try to move
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                    else
                    {
                        //try to rotate
                        clockw = (isDirectionMost(UtilityTools.Directions.left));
                        tribox.Rotate(clockw);
                        return;
                    }
                }

                break;

            case UtilityTools.Directions.upRight:

                break;

            case UtilityTools.Directions.right:

                if (!tri.IsVertical())
                {
                    //if im the leftmost box
                    if (isDirectionMost(UtilityTools.OppositeDirection(dir)))
                    {
                        //if the side is empty
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                }
                else
                {
                    if (isCenterBox())
                    {
                        //try to move
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                    else
                    {
                        //try to rotate
                        clockw = (isDirectionMost(UtilityTools.Directions.up));
                        tribox.Rotate(clockw);
                        return;
                    }
                }

                break;

            case UtilityTools.Directions.downRight:

                break;

            case UtilityTools.Directions.down:
                if (tri.IsVertical())
                {
                    //need to move down
                    //if im the toppest box
                    if (isDirectionMost(UtilityTools.OppositeDirection(dir)))
                    {
                        //if the downside is empty
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                }
                else
                {
                    if (isCenterBox())
                    {
                        //try to move
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                    else
                    {
                        //try to rotate
                        clockw = (isDirectionMost(UtilityTools.Directions.right));
                        tribox.Rotate(clockw);
                        return;
                    }
                }

                break;

            case UtilityTools.Directions.downLeft:
                break;

            case UtilityTools.Directions.left:

                if (!tri.IsVertical())
                {
                    //if im the leftmost box
                    if (isDirectionMost(UtilityTools.OppositeDirection(dir)))
                    {
                        //if the side is empty
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                }
                else
                {
                    if (isCenterBox())
                    {
                        //try to move
                        if (tri.isSideEmpty(dir))
                        {
                            tribox.Move(dir);
                        }
                    }
                    else
                    {
                        //try to rotate
                        clockw = (isDirectionMost(UtilityTools.Directions.down));
                        tribox.Rotate(clockw);
                        return;
                    }
                }

                break;

            case UtilityTools.Directions.upLeft:
                break;

            default:
                return;
        }
    }

    public Tile getNeighbourTile(UtilityTools.Directions dir)
    {
        if (grid_ref == null) return null;

        return grid_ref.GetTile(index.getNeighbourPoint(dir));
    }

    public static bool areAllConnected(UtilityTools.Directions dir, List<Box> boxList)
    {
        if (UtilityTools.horizontals.Contains(dir))
        {
            boxList = boxList.OrderBy(b => b.GetPoint().x).ToList();
        }
        else if (UtilityTools.verticals.Contains(dir))
        {
            boxList = boxList.OrderBy(b => b.GetPoint().y).ToList();
        }
        else return false;

        for (int i = 0; i < boxList.Count; i++)
        {
            if (i < boxList.Count - 1)
            {
                if (UtilityTools.horizontals.Contains(dir))
                {
                    //if index difference is above one, they are not neighbours
                    if (Mathf.Abs(boxList[i].GetPoint().Clone().x - boxList[i + 1].GetPoint().Clone().x) > 1)
                    {
                        return false;
                    }
                }
                else if (UtilityTools.verticals.Contains(dir))
                {
                    if (Mathf.Abs(boxList[i].GetPoint().Clone().y - boxList[i + 1].GetPoint().Clone().y) > 1)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public static Box.Element getRandomBoxElement()
    {
        //get a random box based on their probabilities
        int i = UtilityTools.GetRandomWeightedIndex(typeWeights);
        //if by any chance the random doesn't work, random again but without the special one
        if (i < 0) i = Random.Range(0, System.Enum.GetNames(typeof(Box.Element)).Length - 1);

        return (Box.Element)i;
    }

    public static Box.Element[] getRandomBoxElements(int quantity, bool rollQuintessentialOnlyOnce = true)
    {
        Box.Element[] result = new Box.Element[quantity];

        bool hasQuint = (rollQuintessentialOnlyOnce && Random.value < typeWeights[(int)Box.Element.quintessential] / typeWeights.Sum()) ? true : false;
        int quintIndex = Random.Range(0, quantity);

        int sameCount = 0;
        for (int i = 0; i < quantity; i++)
        {
            if (i == quintIndex && hasQuint)
            {
                result[quintIndex] = Box.Element.quintessential;
            }
            else
            {

                result[i] = (Box.Element)Random.Range(0, System.Enum.GetNames(typeof(Box.Element)).Length - 1);
                if (hasQuint)
                {
                    if(result.ToList().FindAll(x=> x == result[i]).Count > 1)
                    {
                        Box.Element repeated = result[i];
                        int loopcount = 0;
                        while((result[i] == repeated || result[i] == Box.Element.quintessential) && loopcount < 100)
                        {
                            result[i] = (Box.Element)Random.Range(0, System.Enum.GetNames(typeof(Box.Element)).Length - 1);
                            loopcount++;
                        }
                    }
                    
                }
                else
                {
                    if (result.ToList().FindAll(x => x == result[i]).Count >2)
                    {
                        Box.Element repeated = result[i];
                        int loopcount = 0;
                        while ((result[i] == repeated || result[i] == Box.Element.quintessential) && loopcount < 100)
                        {
                            result[i] = (Box.Element)Random.Range(0, System.Enum.GetNames(typeof(Box.Element)).Length - 1);
                            loopcount++;
                        }
                    }
                }
                
            }

           
        }


        return result;
    }
}