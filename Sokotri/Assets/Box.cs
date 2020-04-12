using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public enum type{
        ground, //brown
        fire, // red
        water, // blue
        grass, //green
        steel, //gray
        quintessential //special 

    }

    public static Color32 quintessentialColor = Color.magenta;

    //probability of each type of box
    static float[] typeWeights = {18,18,18,18,18,10};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public static Box.type  getRandomBoxType()
    {
        //get a random box based on their probabilities
        int i = UtilityTools.GetRandomWeightedIndex(typeWeights);
        //if by any chance the random doesn't work, random again but without the special one
        if(i < 0) i = Random.Range(0, System.Enum.GetNames(typeof(Box.type)).Length - 1);

        return (Box.type) i;

    }

}
