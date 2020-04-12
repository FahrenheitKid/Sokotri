using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;


public static class CollectionsExtensions
{
    public static List<int> FindAllIndex<T>(this List<T> container, System.Predicate<T> match)
    {
        var items = container.FindAll(match);
        List<int> indexes = new List<int>();
        foreach (var item in items)
        {
            indexes.Add(container.IndexOf(item));
        }

        return indexes;
    }

     public static int[] FindAllIndexof<T>(this IEnumerable<T> values, T val)
    {
        return values.Select((b,i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
    }

   
    public static List<T> Repeated<T>(this List <T> list, T value, int count)
    {
        List<T> ret = new List<T>(count);
        ret.AddRange(Enumerable.Repeat(value, count));
        return ret;
    }
}


static public class UtilityTools
{
     public enum Directions{
        up,
        upRight,
        right,
        downRight,
        down,
        downLeft,
        left,
        upLeft

    }


    public static bool isPointInViewport (Vector3 screenPoint) // returns true if point is inside viewport
    {

        if (screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f)
        {
            return true;
        }
        else
            return false;
    }

    public static bool isPointInViewport (Vector3[] points) // returns true if all points are visible
    {

        bool result = true;
        foreach (Vector3 p in points)
        {

            if (!(p.z > 0f && p.x > 0f && p.x < 1f && p.y > 0f && p.y < 1f))
            {

                result = false;
            }

        }

        return result;
    }

    public  static bool IsSameOrSubclass(System.Type potentialBase, System.Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

            static public double linear(double x, double x0, double x1, double y0, double y1)
{
    if ((x1 - x0) == 0)
    {
        return (y0 + y1) / 2;
    }
    return y0 + (x - x0) * (y1 - y0) / (x1 - x0);


}

private static bool HasGenericBase(System.Type myType, System.Type t) {
    Debug.Assert(t.IsGenericTypeDefinition);
    while (myType != typeof(object)) {
        if (myType.IsGenericType && myType.GetGenericTypeDefinition() == t) {
            return true;
        }
        myType = myType.BaseType;
    }
    return false;
}

static int getRealConnectedJoysticksCount()
{
    int count = 0;

    foreach(string t in Input.GetJoystickNames())
    {
        if(t != "") count++;
    }

    return count;
}

public static void QuitGame ()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit ();
#endif
        }

}