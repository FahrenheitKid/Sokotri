using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

public class TheGrid : MonoBehaviour
{
    Tile.Status[,] gridLayout;
    Tile [,] grid;

    [Header("Prefabs")]
    [SerializeField]
    GameObject tile_Prefab;

    [SerializeField]
    int width;
    [SerializeField]
    int height;
    [SerializeField]    
    string seed;

    [SerializeField]
    string loadMapFilename;


    // Start is called before the first frame update
    void Start()
    {

        Random.InitState(seed.GetHashCode());

        if(loadMapFilename.Any())
        {

        loadGridLayoutFromFile(Application.streamingAssetsPath + "\\" + loadMapFilename);
        width = gridLayout.GetLength(0);
        height = gridLayout.GetLength(1);

        }

        //making sure default values are valid
        width = (width < 0) ? 0 : width;
        height = (width < 0) ? 0 : width;
        seed = (!seed.Any()) ? getRandomSeed() : seed;

        grid = new Tile[width,height];

        setupGrid();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setupGrid(bool useGridLayout = true)
    {
        Transform gridParent = GameObject.FindGameObjectWithTag("GridLayout").transform;

        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                Tile tile = Instantiate(tile_Prefab,gridParent).GetComponent<Tile>();
                tile.Init(new Point(j,i), Color.white,this);
                if(useGridLayout)
                {
                    tile.setStatus(gridLayout[j,i]);
                }
                tile.resetPosition();
            }
        }
        return;
    }


    void loadGridLayoutFromFile(string filep)
    {
        gridLayout = Load(filep);
        
    }

    public int getWidth() { return width;}
    public int getHeight(){ return height;}

    public Vector2 getExpectedPositionFromPoint(Point p)
    {
        return new Vector2(p.x -width / 2 , -p.y + height / 2);
    }

     //load txt file with the map
        private Tile.Status[, ] Load (string filePath)
        {
            try
            {
#if MAP_LOADING_DEBUG
                Debug.Log ("Loading File...");
#endif

                using (StreamReader sr = new StreamReader (filePath))
                {

                    // read first line to detect width and height size
                    string width_height = sr.ReadLine ();
                    string[] aux = width_height.Split (new []
                    {
                        ','
                    });
                    int aux_val;
                    if (int.TryParse (aux[0], out aux_val))
                        width = aux_val;
                    if (int.TryParse (aux[1], out aux_val))
                        height = aux_val;

#if MAP_LOADING_DEBUG
                    print ("Map Width: " + width + " | Map Height: " + height);
#endif

                    // read the rest of the file
                    string input = sr.ReadToEnd ();
                    string[] lines = input.Split (new []
                    {
                        '\r',
                        '\n'
                    }, System.StringSplitOptions.RemoveEmptyEntries);
                    int[, ] tiles = new int[lines.Length, width];

#if MAP_LOADING_DEBUG
                    Debug.Log ("Parsing...");
#endif

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string st = lines[i];
                        string[] nums = st.Split (new []
                        {
                            ','
                        });
                        if (nums.Length != width)
                        {

                        }
                        for (int j = 0; j < Mathf.Min (nums.Length, width); j++)
                        {
                            int val;
                            if (int.TryParse (nums[j], out val))
                            {
                                tiles[i, j] = val;
                            }
                            else
                            {
                                tiles[i, j] = 0;
                            }
                        }
                    }
#if MAP_LOADING_DEBUG
                    Debug.Log ("Parsing Completed!");
#endif              
                    return (Tile.Status[,]) (object) tiles;
                }
            }
            catch (IOException e)
            {

                Debug.Log (e.Message);
            }
            return null;
        }


    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "!#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        Random.InitState(System.Environment.TickCount);
        for(int i = 0; i < 20; i++)
        {
            seed+= acceptableChars[Random.Range(0,acceptableChars.Length)];
        }
        return seed;
    }
}
