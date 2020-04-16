using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
{
    private IEqualityComparer<T> comparer;

    public SequenceComparer(IEqualityComparer<T> comparer = null)
    {
        this.comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
    {
        return x.SequenceEqual(y, comparer);
    }

    public int GetHashCode(IEnumerable<T> sequence)
    {
        unchecked
        {
            int hash = 19;
            foreach (var item in sequence)
                hash = hash * 79 + comparer.GetHashCode(item);
            return hash;
        }
    }
}

public class ListEqualityComparer<T> : IEqualityComparer<List<T>>
{
    private readonly IEqualityComparer<T> _itemEqualityComparer;

    public ListEqualityComparer() : this(null)
    {
    }

    public ListEqualityComparer(IEqualityComparer<T> itemEqualityComparer)
    {
        _itemEqualityComparer = itemEqualityComparer ?? EqualityComparer<T>.Default;
    }

    public static readonly ListEqualityComparer<T> Default = new ListEqualityComparer<T>();

    public bool Equals(List<T> x, List<T> y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
        return x.Count == y.Count && !x.Except(y, _itemEqualityComparer).Any();
    }

    public int GetHashCode(List<T> list)
    {
        int hash = 17;
        foreach (var itemHash in list.Select(x => _itemEqualityComparer.GetHashCode(x))
                                     .OrderBy(h => h))
        {
            hash += 31 * itemHash;
        }
        return hash;
    }
}

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
        return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
    }

    public static List<T> Repeated<T>(this List<T> list, T value, int count)
    {
        List<T> ret = new List<T>(count);
        ret.AddRange(Enumerable.Repeat(value, count));
        return ret;
    }
}

static public class UtilityTools
{
    public enum Directions
    {
        up,
        upRight,
        right,
        downRight,
        down,
        downLeft,
        left,
        upLeft
    }

    public static Directions[] allDirections = {
        Directions.up,
        Directions.upRight,
        Directions.right,
        Directions.downRight,
        Directions.down,
        Directions.downLeft,
        Directions.left,
        Directions.upLeft
    };

    public static Directions[] axials = {
        Directions.up,
        Directions.right,
        Directions.down,
        Directions.left
    };

    public static Directions[] horizontals = {
        Directions.right,
        Directions.left
    };

    public static Directions[] verticals = {
        Directions.up,
        Directions.down
    };

    public static Directions[] diagonals = {
        Directions.upRight,
        Directions.downRight,
        Directions.downLeft,
        Directions.upLeft
    };

    public static Directions OppositeDirection(Directions dir, bool diagonoalsXYInverted = true)
    {
        switch (dir)
        {
            case UtilityTools.Directions.up:
                return Directions.down;
                break;

            case UtilityTools.Directions.upRight:

                return (diagonoalsXYInverted) ? Directions.downLeft : Directions.downRight;

                break;

            case UtilityTools.Directions.right:
                return Directions.left;
                break;

            case UtilityTools.Directions.downRight:
                return (diagonoalsXYInverted) ? Directions.upLeft : Directions.upRight;
                break;

            case UtilityTools.Directions.down:
                return Directions.up;
                break;

            case UtilityTools.Directions.downLeft:
                return (diagonoalsXYInverted) ? Directions.upRight : Directions.upLeft;
                break;

            case UtilityTools.Directions.left:
                return Directions.right;
                break;

            case UtilityTools.Directions.upLeft:
                return (diagonoalsXYInverted) ? Directions.downRight : Directions.downLeft;
                break;

            default:
                return Directions.right;
                break;
        }
    }

    public static Vector3 getDirectionVector(UtilityTools.Directions dir)
    {
        switch (dir)
        {
            case UtilityTools.Directions.up:
                return Vector3.up;
                break;

            case UtilityTools.Directions.upRight:

                return (Vector3.up + Vector3.right).normalized;

                break;

            case UtilityTools.Directions.right:
                return Vector3.right;
                break;

            case UtilityTools.Directions.downRight:
                return (Vector3.down + Vector3.right).normalized;
                break;

            case UtilityTools.Directions.down:
                return Vector3.down;
                break;

            case UtilityTools.Directions.downLeft:
                return (Vector3.down + Vector3.left).normalized;
                break;

            case UtilityTools.Directions.left:
                return Vector3.left;
                break;

            case UtilityTools.Directions.upLeft:
                return (Vector3.up + Vector3.left).normalized;
                break;

            default:
                return Vector3.zero;
                break;
        }
    }

    public static int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }

    public static bool isPointInViewport(Vector3 screenPoint) // returns true if point is inside viewport
    {
        if (screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f)
        {
            return true;
        }
        else
            return false;
    }

    public static bool isPointInViewport(Vector3[] points) // returns true if all points are visible
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

    public static bool IsSameOrSubclass(System.Type potentialBase, System.Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase) ||
            potentialDescendant == potentialBase;
    }

    static public double linear(double x, double x0, double x1, double y0, double y1)
    {
        if ((x1 - x0) == 0)
        {
            return (y0 + y1) / 2;
        }
        return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
    }

    private static bool HasGenericBase(System.Type myType, System.Type t)
    {
        Debug.Assert(t.IsGenericTypeDefinition);
        while (myType != typeof(object))
        {
            if (myType.IsGenericType && myType.GetGenericTypeDefinition() == t)
            {
                return true;
            }
            myType = myType.BaseType;
        }
        return false;
    }

    private static int getRealConnectedJoysticksCount()
    {
        int count = 0;

        foreach (string t in Input.GetJoystickNames())
        {
            if (t != "") count++;
        }

        return count;
    }

    public static void QuitGame()
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