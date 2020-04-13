using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Element element;

    [SerializeField]
    Point index;
    [SerializeField]
    Tile tile;

    [SerializeField]
    TriBox tribox;

    TheGrid grid_ref;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    public static Color32 quintessentialColor = Color.magenta;

    //probability of each type of box
    static float[] typeWeights = { 18, 18, 18, 18, 18, 10 };

    // Start is called before the first frame update
    void Start ()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public bool isCenterBox ()
    {
        if (tribox == null) return false;

        bool center = false;

        foreach (Box b in tribox.GetBoxes ())
        {
            if (b == this) continue;

            if (isMyNeighbour (b, false))
            {
                center = true;
            }
            else
            {
                center = false;
            }
        }

        return center;

    }

    public bool isMyNeighbour (Box b, bool diagonals)
    {

        return index.isMyNeighbour (b.GetPoint ().Clone (), diagonals);

    }

    public Point GetPoint ()
    {
        return index;
    }
    public void setElement (Element e)
    {
        if (e == element) return;

        element = e;
        if ((int) e > 0 && (int) e < elementSprites.Length)
        {
            if (elementSprites[(int) e] != null)
                spriteRenderer.sprite = elementSprites[(int) e];
        }

        if (element == Element.quintessential)
        {
            spriteRenderer.color = quintessentialColor;
        }
    }

    public void setTile (Tile t)
    {
        if (t == tile) return;

        tile = t;

        if (tile.GetStatus () != Tile.Status.box) tile.setStatus (Tile.Status.box, this);

    }

    public Element GetElement ()
    {
        return element;
    }

    public static Box.Element getRandomBoxElement ()
    {
        //get a random box based on their probabilities
        int i = UtilityTools.GetRandomWeightedIndex (typeWeights);
        //if by any chance the random doesn't work, random again but without the special one
        if (i < 0) i = Random.Range (0, System.Enum.GetNames (typeof (Box.Element)).Length - 1);

        return (Box.Element) i;

    }

    public static Box.Element[] getRandomBoxElements (int quantity, bool rollQuintessentialOnlyOnce = true)
    {
        Box.Element[] result = new Box.Element[quantity];

        bool hasQuint = (rollQuintessentialOnlyOnce && Random.value < typeWeights[(int) Box.Element.quintessential] / 100) ? true : false;
        int quintIndex = Random.Range (0, quantity);

        bool same = false;
        for (int i = 0; i < quantity; i++)
        {
            if (i == quintIndex && hasQuint)
            {
                result[quintIndex] = Box.Element.quintessential;

            }
            else
            {
                result[i] = (Box.Element) Random.Range (0, System.Enum.GetNames (typeof (Box.Element)).Length - 1);
            }

            if (i > 0)
            {
                if (result[i - 1] == result[i])
                {
                    if (!same)
                    {
                        same = true;
                    }
                    else
                    {
                        int loops = 0;
                        while (result[i - 1] == result[i] && loops < 100)
                        {
                            result[i] = (Box.Element) Random.Range (0, System.Enum.GetNames (typeof (Box.Element)).Length - 1);
                            loops++;
                        }
                        same = false;

                    }

                }
                else
                {
                    same = false;
                }
            }
        }
        return result;

    }

}