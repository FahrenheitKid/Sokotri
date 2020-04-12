using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x_, int y_)
    {
        x = x_;
        y = y_;
    }


    public Point getNeighbourPoint(UtilityTools.Directions dir)
    {
        Point p = new Point(x,y);

        switch(dir)
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

    public void Copy(Point p)
    {
        x = p.x;
        y = p.y;
    }
    public  bool Equals(Point p)
    {
        return (p.x == x && p.y == y);
    }

    public Point Clone()
    {
        return new Point(x,y);
    }

    public Point Clone(Point p)
    {
        return new Point(p.x,p.y);
    }

    public string print()
    {
        return ("(" + x +", " + y +")");
    }

}
