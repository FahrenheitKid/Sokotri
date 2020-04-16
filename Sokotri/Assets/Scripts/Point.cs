using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    static public Point zero = new Point (0, 0);

    public Point (int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public void Add(Point p)
    {
        x += p.x;
        y += p.y;
    }
    public void Add(int _x, int _y)
    {
        x += _x;
        y +=_y;
    }

    public static Point byMagnitude(Point p, int magnitude)
    {
        return new Point(p.x * magnitude, p.y * magnitude);
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public Point getNeighbourPoint (UtilityTools.Directions dir)
    {
        Point p = new Point (x, y);

        switch (dir)
        {
            case UtilityTools.Directions.up:
                p.y--;
                break;

            case UtilityTools.Directions.upRight:
                p.y--;
                p.x++;

                break;

            case UtilityTools.Directions.right:
                p.x++;
                break;

            case UtilityTools.Directions.downRight:
                p.y++;
                p.x++;
                break;

            case UtilityTools.Directions.down:
                p.y++;
                break;

            case UtilityTools.Directions.downLeft:
                p.y++;
                p.x--;
                break;

            case UtilityTools.Directions.left:
                p.x--;
                break;

            case UtilityTools.Directions.upLeft:
                p.y--;
                p.x--;
                break;

        }

        return p;
    }

    public bool isMyNeighbour (Point p, bool diagonals)
    {
        if (p.Equals (getNeighbourPoint (UtilityTools.Directions.up)) ||
            p.Equals (getNeighbourPoint (UtilityTools.Directions.down)) ||
            p.Equals (getNeighbourPoint (UtilityTools.Directions.left)) ||
            p.Equals (getNeighbourPoint (UtilityTools.Directions.right)))
        {
            return true;
        }

        if (diagonals)
        {
            if (p.Equals (getNeighbourPoint (UtilityTools.Directions.upRight)) ||
                p.Equals (getNeighbourPoint (UtilityTools.Directions.downRight)) ||
                p.Equals (getNeighbourPoint (UtilityTools.Directions.downLeft)) ||
                p.Equals (getNeighbourPoint (UtilityTools.Directions.upLeft)))
            {
                return true;
            }
        }

        return false;
    }

    public void Copy (Point p)
    {
        x = p.x;
        y = p.y;
    }
    public bool Equals (Point p)
    {
        return (p.x == x && p.y == y);
    }

    public Point Clone ()
    {
        return new Point (x, y);
    }

    public static Point Clone (Point p)
    {
        return new Point (p.x, p.y);
    }

    public string print ()
    {
        return ("(" + x + ", " + y + ")");
    }

}