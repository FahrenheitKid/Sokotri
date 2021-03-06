using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheGrid : MonoBehaviour
{
    private Tile.Kind[,] gridLayout;

    public static int[,] defaultLayoutMap = new int[13, 13] {
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,3,3,3,3,3,0,0,0,0},
                    {0,0,0,0,3,3,4,3,3,0,0,0,0},
                    {0,0,0,0,3,3,4,3,3,0,0,0,0},
                    {0,0,0,0,3,3,4,3,3,0,0,0,0},
                    {0,0,0,0,3,3,3,3,3,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0 } };

    private Tile[,] grid;

    [SerializeField]
    private List<Box> boxes = new List<Box>();

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject gridLayout_ref;

    [SerializeField]
    private Match3 match3_ref;

    [SerializeField]
    private PreviewBoxes previewBoxes;

    [SerializeField]
    private ScoreUI scoreUI;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject tile_Prefab;

    [SerializeField]
    private GameObject tribox_prefab;

    [SerializeField]
    private GameObject box_prefab;

    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private string seed;

    [SerializeField]
    private string loadMapFilename;

    [SerializeField]
    public static float moveTime = 0.25f;

    [SerializeField]
    public static int matchSize = 3;

    [SerializeField]
    public static float boxKillDelay = 0.25f;

    [SerializeField]
    public static float boxKillAnimationTime = 0.35f;

    public static string highScoreKey = "highscore";

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int match3Uses = 3;

    [SerializeField]
    private bool isMatch3Phase = false;

    [Header("Audio")]
    [SerializeField]
    private AudioClip matchAudioClip;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float audioVolume;

    [SerializeField]
    private bool isMuted = false;

    //check i seed

    private void Awake()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = 60;

#if UNITY_STANDALONE
        Screen.SetResolution(1920, 1080, true);
#endif

#if UNITY_WEBGL
        Screen.SetResolution(1280, 720, false);
#endif
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (previewBoxes == null)
            previewBoxes = GameObject.FindGameObjectWithTag("PreviewBoxes").GetComponent<PreviewBoxes>();

        if (scoreUI == null)
            scoreUI = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreUI>();

        if (match3_ref == null)
            match3_ref = GetComponent<Match3>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioVolume = audioSource.volume;
        isMuted = false;

        Random.InitState(seed.GetHashCode());

#if UNITY_STANDALONE
         if (loadMapFilename.Any())
        {
            loadGridLayoutFromFile(Application.streamingAssetsPath + "\\" + loadMapFilename);
        }
#endif

#if UNITY_WEBGL
        loadGridLayoutFromArray(defaultLayoutMap);
#endif

        if (gridLayout == null)
        {
            loadGridLayoutFromArray(defaultLayoutMap);
        }
        width = gridLayout.GetLength(0);
        height = gridLayout.GetLength(1);

        //making sure default values are valid
        width = (width < 0) ? 0 : width;
        height = (width < 0) ? 0 : width;
        seed = (!seed.Any()) ? getRandomSeed() : seed;

        grid = new Tile[width, height];

        setupGrid();

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.SpawnAt(new Point(width / 2 + 2, height / 2 + 2));

        previewBoxes.generateNextPreview();
        spawnTriBox(previewBoxes.getCurrentBoxElements());
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //spawnTriBox(previewBoxes.getCurrentBoxElements());
            //previewBoxes.generateNextPreview();
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UtilityTools.QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            toggleMatch3Phase();
        }
    }

    public void RestartGame()
    {
        if (PlayerPrefs.GetInt(highScoreKey, 0) < score)
            PlayerPrefs.SetInt(highScoreKey, score);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void setupGrid(bool useGridLayout = true)
    {
        Transform gridParent = GameObject.FindGameObjectWithTag("GridLayout").transform;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Tile tile = Instantiate(tile_Prefab, gridParent).GetComponent<Tile>();
                tile.Init(new Point(j, i), Color.white, this);
                if (useGridLayout)
                {
                    tile.setKind(gridLayout[i, j]);
                }
                tile.resetPosition();
                grid[j, i] = tile;
            }
        }
        return;
    }

    public void spawnTriBox(Box.Element[] elements, bool _isVertical = true, Tile center = null)
    {
        if (center != null)
        {
            if (center.AreMeAndNeighbours(Tile.Kind.spawn, Tile.Status.empty, _isVertical, !_isVertical))
            {
                print("center tile provided not valid");
                return;
            }
        }

        if (center == null)
        {
            List<Tile> spawnableTiles = GetCenterTiles(_isVertical, !_isVertical, Tile.Kind.spawn).FindAll(x => x.GetStatus() == Tile.Status.empty);
            List<TriBox> triboxesInsideSafe = GetAllTriBoxes();
            triboxesInsideSafe.RemoveAll(x => x.isOutsideSafeArea() == true);

            if (!spawnableTiles.Any() || triboxesInsideSafe.Any())
            {
                // print("no free spawn position available");
                return;
            }
            center = spawnableTiles[Random.Range(0, spawnableTiles.Count)];
        }

        TriBox tri = Instantiate(tribox_prefab, Vector3.zero, Quaternion.identity).GetComponent<TriBox>();
        tri.Init(center, elements, this, _isVertical);

        previewBoxes.generateNextPreview();
    }

    public void spawnTriBox()
    {
        spawnTriBox(previewBoxes.getCurrentBoxElements());
    }

    public void Match(List<List<Box>> boxMatchesList)
    {
        if (!boxMatchesList.Any() || !boxMatchesList.Any(x => x.Count >= TheGrid.matchSize)) return;

        int uniqueElementMatches = 0;
        uniqueElementMatches = boxMatchesList.SelectMany(x => x).Distinct() // get the lists of lists in a single list
            .GroupBy(y => y.GetElement()).Select(z => z.First() && z.Count() >= TheGrid.matchSize).Count(); // then distinctBy a specific field and get how many are there

        //need to substract 1 for each quintessential present in the matches
        int quintOffset = boxMatchesList.FindAll(x => x.FindAll(b => b.GetElement() == Box.Element.quintessential).Any()).Count();
        uniqueElementMatches -= quintOffset;

        if (uniqueElementMatches <= 0) uniqueElementMatches = 1;

        int totalScore = 0;

        List<TriBox> triboxList = new List<TriBox>();

        foreach (List<Box> lb in boxMatchesList)
        {
            totalScore += lb.Count();
            foreach (Box b in lb)
            {
                if (!triboxList.Contains(b.GetTriBox()) && b.GetTriBox() != null)
                {
                    triboxList.Add(b.GetTriBox());
                }

                b.Kill(boxKillDelay);
            }
        }

        foreach (TriBox tri in triboxList)
        {
            tri.Kill();
        }

        int multiplier = 1;

        if (TheGrid.matchSize > 0)
            multiplier = totalScore / TheGrid.matchSize;
        if (multiplier < 1) multiplier = 1;

        totalScore *= (uniqueElementMatches + multiplier < 2) ? 1 : uniqueElementMatches + multiplier;
        Color32 colorOfBiggestElementMatch = new Color();

        Box boxWithTheElement = null;

        if (boxMatchesList.OrderByDescending(item => item.Count).First().Any(x => x.GetElement() != Box.Element.quintessential))
            boxWithTheElement = boxMatchesList.OrderByDescending(item => item.Count).First().First(x => x.GetElement() != Box.Element.quintessential);

        if (boxWithTheElement == null) totalScore *= totalScore;

        colorOfBiggestElementMatch = (boxWithTheElement != null) ? Box.getElementColor(boxWithTheElement.GetElement()) : Box.getElementColor(Box.Element.quintessential); ;

        Score(totalScore, colorOfBiggestElementMatch);

        if (matchAudioClip)
        {
            audioSource.clip = matchAudioClip;
            audioSource.Play();
        }
    }

    public void Score(int valToSum, Color32 c)
    {
        score += valToSum;
        if (score < 0) score = 0;
        scoreUI.UpdateScore(score, c);
    }

    public void Score(int valToSum)
    {
        score += valToSum;
        if (score < 0) score = 0;
        scoreUI.UpdateScore(score, new Color32(0, 0, 0, 255));
    }

    public void SetScore(int val)
    {
        score = val;
    }

    public int GetScore()
    {
        return score;
    }

    private void loadGridLayoutFromFile(string filep)
    {
        gridLayout = Load(filep);
    }

    private void loadGridLayoutFromArray(int[,] map)
    {
        Tile.Kind[,] layout = new Tile.Kind[map.GetLength(0), map.GetLength(1)];

        for (int i = 0; i < map.GetLength(0) && i < layout.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(0) && j < layout.GetLength(0); j++)
            {
                layout[i, j] = (Tile.Kind)map[i, j];
            }
        }
        gridLayout = layout;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public bool IsMatch3Phase()
    {
        return isMatch3Phase;
    }

    public void SetMatch3Phase(bool b)
    {
        isMatch3Phase = b;
    }

    public int getMatch3Uses()
    {
        return match3Uses;
    }

    public void toggleMatch3Phase()
    {
        if (match3Uses < 0 && isMatch3Phase || match3_ref.GetMovingBox() != null) return;

        isMatch3Phase = !isMatch3Phase;

        if (isMatch3Phase)
        {
            match3Uses--;
            if (match3Uses < 0) match3Uses = 0;

            scoreUI.UpdateMatch3Uses(match3Uses);
        }
        else
        {
            foreach (Box b in UtilityTools.FindComponentsWithTag<Box>("Box"))
            {
                if (b.isHighlighted) b.Highlight(false);

                if (b.transform.localScale.x > 1)
                {
                    if (b.GetKillSequence() != null)
                    {
                        if (b.GetKillSequence().IsPlaying()) continue;
                    }

                    b.transform.DOScale(Vector3.one, TheGrid.moveTime);
                    Vector3 temp = b.transform.position;
                    temp.z = -1;
                    b.transform.position = temp;
                }
            }
        }

        //darrken some sprites when in this mode
        float h, s, v;

        if (gridLayout_ref)
        {
            foreach (Transform go in gridLayout_ref.transform)
            {
                SpriteRenderer tile = go.GetComponent<SpriteRenderer>();
                if (tile != null)
                {
                    Color normal = Tile.getKindColor(go.GetComponent<Tile>().GetKind());

                    Color.RGBToHSV(normal, out h, out s, out v);

                    v *= 0.5f;

                    Color desaturated = Color.HSVToRGB(h, s, v);

                    tile.DOColor((isMatch3Phase) ? desaturated : normal, TheGrid.moveTime * 2);
                }
            }
        }
        player.GetComponent<SpriteRenderer>().DOColor((isMatch3Phase) ? Color.gray : Color.white, TheGrid.moveTime * 2);
    }

    public void ToggleMute()
    {
        MusicPlayer.ToggleMute(audioSource, ref isMuted, TheGrid.moveTime, audioVolume);
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public Match3 GetMatch3()
    {
        return match3_ref;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Tile GetTile(Point p)
    {
        if (isPointOutOfBounds(p))
        {
            //print ("Point" + p.print () + "is out of bounds");
            return null;
        }

        return (grid[p.x, p.y].GetPoint().Equals(p)) ? grid[p.x, p.y] : null;
    }

    public List<TriBox> GetAllTriBoxes()
    {
        List<TriBox> result = new List<TriBox>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("TriBox"))
        {
            TriBox tri = go.GetComponent<TriBox>();
            if (tri == null) continue;

            result.Add(tri);
        }
        return result;
    }

    // get center tiles of specific Kind that have neighbours of the same type
    private List<Tile> GetCenterTiles(bool vertical, bool horizontal, Tile.Kind kind, bool diagonal = false)
    {
        //if(typeof(T) != typeof(Tile.Kind)) return null;

        List<Tile> allOfKinds = new List<Tile>();

        List<Tile> result = new List<Tile>();

        foreach (Tile k in grid)
        {
            if (k.GetKind() == Tile.Kind.spawn) allOfKinds.Add(k);
        }

        foreach (Tile t in allOfKinds)
        {
            Point neighbour1 = Point.zero;
            Point neighbour2 = Point.zero;
            if (vertical)
            {
                neighbour1 = t.GetPoint().getNeighbourPoint(UtilityTools.Directions.up);
                neighbour2 = t.GetPoint().getNeighbourPoint(UtilityTools.Directions.down);

                if (isPointOutOfBounds(neighbour1) || isPointOutOfBounds(neighbour2)) continue;

                if (GetTile(neighbour1).GetKind() == kind && GetTile(neighbour2).GetKind() == kind)
                {
                    result.Add(t);
                }
            }

            if (horizontal)
            {
                neighbour1 = t.GetPoint().getNeighbourPoint(UtilityTools.Directions.left);
                neighbour2 = t.GetPoint().getNeighbourPoint(UtilityTools.Directions.right);

                if (isPointOutOfBounds(neighbour1) || isPointOutOfBounds(neighbour2)) continue;

                if (GetTile(neighbour1).GetKind() == kind && GetTile(neighbour2).GetKind() == kind)
                {
                    result.Add(t);
                }
            }
        }

        return result;
    }

    public bool isPointOutOfBounds(Point p)
    {
        return (p.x < 0 || p.y < 0 || p.x >= width || p.y >= height || p.x >= grid.GetLength(0) || p.y >= grid.GetLength(1));
    }

    public Vector2 getExpectedPositionFromPoint(Point p)
    {
        return new Vector2(p.x - width / 2, -p.y + height / 2);
    }

    //load txt file with the map
    private Tile.Kind[,] Load(string filePath)
    {
        try
        {
#if MAP_LOADING_DEBUG
            Debug.Log ("Loading File...");
#endif

            using (StreamReader sr = new StreamReader(filePath))
            {
                // read first line to detect width and height size
                string width_height = sr.ReadLine();
                string[] aux = width_height.Split(new[]
                {
                    ','
                });
                int aux_val;
                if (int.TryParse(aux[0], out aux_val))
                    width = aux_val;
                if (int.TryParse(aux[1], out aux_val))
                    height = aux_val;

#if MAP_LOADING_DEBUG
                print ("Map Width: " + width + " | Map Height: " + height);
#endif

                // read the rest of the file
                string input = sr.ReadToEnd();
                string[] lines = input.Split(new[]
                {
                    '\r',
                    '\n'
                }, System.StringSplitOptions.RemoveEmptyEntries);
                int[,] tiles = new int[lines.Length, width];

#if MAP_LOADING_DEBUG
                Debug.Log ("Parsing...");
#endif

                for (int i = 0; i < lines.Length; i++)
                {
                    string st = lines[i];
                    string[] nums = st.Split(new[]
                    {
                        ','
                    });
                    if (nums.Length != width)
                    {
                    }
                    for (int j = 0; j < Mathf.Min(nums.Length, width); j++)
                    {
                        int val;
                        if (int.TryParse(nums[j], out val))
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
                return (Tile.Kind[,])(object)tiles;
            }
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
        return null;
    }

    private string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "!#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        Random.InitState(System.Environment.TickCount);
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }
}