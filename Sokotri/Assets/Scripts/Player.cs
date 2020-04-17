using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private Point index = Point.zero;

    [SerializeField]
    private Point lastIndex;

    [SerializeField]
    private Tile tile;

    [SerializeField]
    private bool isMoving = false;

    [SerializeField]
    private List<TriBox> neighbourTriboxes;

    [SerializeField]
    private List<TriBox> lastNeighbourTriboxes;

    [SerializeField]
    private AudioClip stepAudioClip;

    [SerializeField]
    private AudioClip missStepAudioClip;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float audioVolume;

    [SerializeField]
    private bool isMuted = false;

    private bool canMissStepPlay;

    // Start is called before the first frame update
    private void Start()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioVolume = audioSource.volume;
    }

    // Update is called once per frame
    private void Update()
    {
        Move(getInputDirection());
    }

    public void Init(Point spawn, TheGrid grid = null)
    {
        if (grid_ref == null && grid != null) grid_ref = grid;

        index = spawn;

        SpawnAt(spawn);
    }

    public void SpawnAt(Point p)
    {
        if (grid_ref.isPointOutOfBounds(p))
        {
            return;
        }

        if (grid_ref.GetTile(p) == null)
        {
            return;
        }

        if (!grid_ref.GetTile(p).isEmpty()) return;

        index = p.Clone();
        tile = grid_ref.GetTile(p);
        grid_ref.GetTile(p).setStatus(Tile.Status.player);
        resetPosition();
    }

    private bool Move(UtilityTools.Directions dir)
    {
        if (isMoving == true)
        {
            return false;
        }
        else if (isMoving == false && checkDestination(dir)) //move only if not already moving and if destination is available
        {
            //thren update player's and tile's new values/statuses
            isMoving = true;
            Tile destination = grid_ref.GetTile(index.getNeighbourPoint(dir));
            if (destination == null) return false;

            if (destination.GetStatus() == Tile.Status.box) // need to move or rotate tribox
            {
                destination.GetBox().Push(dir);
            }

            Vector3 destinationPos = destination.transform.position;
            destinationPos.z = -1;
            transform.DOMove(destinationPos, TheGrid.moveTime).OnComplete(() =>
          {
              isMoving = false;
          });

            tile.setStatus(Tile.Status.empty);
            destination.setStatus(Tile.Status.player);
            tile = destination;
            lastIndex.Copy(index);
            setIndex(destination.GetPoint().Clone());

            lastNeighbourTriboxes = neighbourTriboxes;
            neighbourTriboxes = GetAllNeighbourTriboxes();
            foreach (TriBox tri in neighbourTriboxes)
            {
                tri.Highlight(true);
            }

            foreach (TriBox tri in lastNeighbourTriboxes)
            {
                if (!neighbourTriboxes.Contains(tri))
                {
                    tri.Highlight(false);
                }
            }

            if (stepAudioClip)
            {
                audioSource.clip = stepAudioClip;
                audioSource.Play();
            }

            return true;
        }
        else
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if ((input.magnitude < 1 || input == Vector2.zero || input.magnitude > 1) == false)
            {
                if (missStepAudioClip)
                {
                    audioSource.clip = missStepAudioClip;
                    if (audioSource.clip == missStepAudioClip && canMissStepPlay)
                    {
                        canMissStepPlay = false;
                        audioSource.Play();
                    }
                }
            }
            else if (input == Vector2.zero)
            {
                canMissStepPlay = true;
            }

            return false;
        }
    }

    private bool checkDestination(UtilityTools.Directions dir)
    {
        Tile destination = grid_ref.GetTile(index.getNeighbourPoint(dir));
        if (dir == UtilityTools.Directions.upLeft || dir == UtilityTools.Directions.upRight ||
            dir == UtilityTools.Directions.downRight || dir == UtilityTools.Directions.downLeft ||
            destination == null)
        {
            return false;
        }
        else
        {
            if (destination.isEmpty())
            {
                //print("destination is empty: " + index.getNeighbourPoint (dir).print());
                return true;
            }
            else if (destination.GetStatus() == Tile.Status.box && destination.GetBox().isInTribox()) //if destination has a box part of tribox is eligible to Move or Rotate
            {
                TriBox tri = destination.GetBox().GetTriBox();

                /*
                if (destination.GetBox ().isCenterBox ())
                {
                    print("tried to move center");
                    //need to check if can move, then move

                    return false;
                }
                */

                //check if can rotate, then rotate (check already inside rotate)
                //or if can move from below edges

                bool clockw = true;
                switch (dir)
                {
                    case UtilityTools.Directions.up:
                        if (tri.IsVertical())
                        {
                            //if moving from the lowest box, need to move up
                            if (destination.GetBox().isDirectionMost(UtilityTools.OppositeDirection(dir)))
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (destination.GetBox().isCenterBox())
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                //try to rotate
                                //print("trying to UP, colliding box " + destination.GetBox() + " vs actual First" + tri.GetBoxes ().First ());
                                clockw = (destination.GetBox().isDirectionMost(UtilityTools.Directions.left));
                                return tri.canRotate(clockw);
                            }
                        }

                        break;

                    case UtilityTools.Directions.upRight:

                        break;

                    case UtilityTools.Directions.right:

                        if (!tri.IsVertical())
                        {
                            if (destination.GetBox().isDirectionMost(UtilityTools.OppositeDirection(dir)))
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (destination.GetBox().isCenterBox())
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                //try to rotate
                                //print("trying to Right, colliding box " + destination.GetBox() + " vs actual First" + tri.GetBoxes ().First ());
                                clockw = (destination.GetBox().isDirectionMost(UtilityTools.Directions.up));
                                return tri.canRotate(clockw);
                            }
                        }

                        break;

                    case UtilityTools.Directions.downRight:

                        break;

                    case UtilityTools.Directions.down:
                        if (tri.IsVertical())
                        {
                            //need to move down
                            if (destination.GetBox().isDirectionMost(UtilityTools.OppositeDirection(dir)))
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (destination.GetBox().isCenterBox())
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                //try to rotate
                                //print("trying to DOWN, colliding box " + destination.GetBox() + " vs actual last" + tri.GetBoxes ().Last ());
                                clockw = (destination.GetBox().isDirectionMost(UtilityTools.Directions.right));
                                return tri.canRotate(clockw);
                            }
                        }

                        break;

                    case UtilityTools.Directions.downLeft:
                        break;

                    case UtilityTools.Directions.left:

                        if (!tri.IsVertical())
                        {
                            //need to move left
                            if (destination.GetBox().isDirectionMost(UtilityTools.OppositeDirection(dir)))
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (destination.GetBox().isCenterBox())
                            {
                                return tri.canMove(dir);
                            }
                            else
                            {
                                //try to rotate
                                //print("trying to LEFT, colliding box " + destination.GetBox() + " vs actual last" + tri.GetBoxes ().Last ());
                                clockw = (destination.GetBox().isDirectionMost(UtilityTools.Directions.down));
                                return tri.canRotate(clockw);
                            }
                        }

                        break;

                    case UtilityTools.Directions.upLeft:
                        break;

                    default:
                        return false;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }

    public List<TriBox> GetAllNeighbourTriboxes(bool diagonals = false)
    {
        List<TriBox> result = new List<TriBox>();

        List<UtilityTools.Directions> directionsToCheck = UtilityTools.allDirections.ToList();

        if (!diagonals)
        {
            directionsToCheck = UtilityTools.axials.ToList();
        }

        foreach (UtilityTools.Directions dir in UtilityTools.axials)
        {
            Tile t = getNeighbourTile(dir);

            if (t != null)
            {
                if (t.GetStatus() == Tile.Status.box && t.GetBox().isInTribox())
                {
                    result.Add(t.GetBox().GetTriBox());
                }
            }
        }

        return result;
    }

    public List<TriBox> GetAllNonNeighbourTriboxes(bool diagonals = false)
    {
        List<TriBox> result = new List<TriBox>();
        if (grid_ref == null) return result;

        result = grid_ref.GetAllTriBoxes();

        if (!result.Any()) return result;

        if (!diagonals)
        {
            foreach (UtilityTools.Directions dir in UtilityTools.allDirections)
            {
                Tile t = getNeighbourTile(dir);

                if (t != null)
                {
                    if (t.GetStatus() == Tile.Status.box && t.GetBox().isInTribox())
                    {
                        result.Add(t.GetBox().GetTriBox());
                    }
                }
            }
        }

        return result;
    }

    public bool isTriboxMyNeighbour(TriBox tri)
    {
        if (tri == null) return false;

        if (tri.GetBoxes().ToList().Any(x => x == null)) return false;

        foreach (Box b in tri.GetBoxes())
        {
            if (isMyNeighbour(b, true) == false) return false;
        }

        return true;
    }

    public bool isMyNeighbour(Tile t, bool diagonals)
    {
        if (t == null) return false;

        return t.GetPoint().Clone().isMyNeighbour(index.Clone(), diagonals);
    }

    public bool isMyNeighbour(Box b, bool diagonals)
    {
        if (b == null) return false;

        return b.GetPoint().Clone().isMyNeighbour(index.Clone(), diagonals);
    }

    public Tile getNeighbourTile(UtilityTools.Directions dir)
    {
        if (grid_ref == null) return null;

        return grid_ref.GetTile(index.getNeighbourPoint(dir));
    }

    public void setIndex(Point p)
    {
        index.Copy(p);
    }

    public void resetPosition()
    {
        transform.position = grid_ref.getExpectedPositionFromPoint(index);
        Vector3 pos = transform.position;
        pos.z = -1;
        transform.position = pos;
    }

    public void ToggleMute()
    {
        MusicPlayer.ToggleMute(audioSource, ref isMuted, TheGrid.moveTime, audioVolume);
    }

    private UtilityTools.Directions getInputDirection()
    {
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.magnitude < 1 || input == Vector2.zero || input.magnitude > 1)
        {
            return UtilityTools.Directions.upRight;
        }
        else
        {
            switch (input)
            {
                case Vector2 v when v.x >= 1 && v.y == 0: // right
                    //(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.right;
                    break;

                case Vector2 v when v.x <= -1 && v.y == 0: // left
                    // print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.left;
                    break;

                case Vector2 v when v.x == 0 && v.y >= 1: // up
                    // print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.up;
                    break;

                case Vector2 v when v.x == 0 && v.y <= -1: // down
                    //print(input.x + "," + input.y + "mag: " + input.magnitude);
                    return UtilityTools.Directions.down;
                    break;

                default:
                    return UtilityTools.Directions.upRight;
                    break;
            }
        }
    }
}