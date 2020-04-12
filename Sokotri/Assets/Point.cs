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

}
