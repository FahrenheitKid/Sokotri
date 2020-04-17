using DG.Tweening;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public static Match3 instance;

    [SerializeField]
    private Vector2 mouseStart;

    [SerializeField]
    private GameObject box_prefab;

    [SerializeField]
    private Box movingBox;

    [SerializeField]
    private Tile destination = null;

    [SerializeField]
    private Point destinationPoint;

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private float boxWidth = 0f;

    [SerializeField]
    private Vector2 originPoint;

    [SerializeField]
    private Point add;

    [SerializeField]
    private Vector3 pos;

    private void Awake()
    {
        if (grid_ref == null)
            grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<TheGrid>();

        instance = this;

        if (box_prefab != null)
            boxWidth = box_prefab.GetComponent<SpriteRenderer>().sprite.rect.width;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!grid_ref.IsMatch3Phase()) return;

        handleMouseMovement();

        // handleMouse2();
    }

    private void handleMouse2()
    {
        if (!grid_ref.IsMatch3Phase()) return;
        if (movingBox != null)
        {
            originPoint = grid_ref.getExpectedPositionFromPoint(movingBox.GetPoint());
            pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            pos.x = Mathf.Clamp(pos.x, originPoint.x - 0.5f, originPoint.x + 0.5f);
            pos.y = Mathf.Clamp(pos.y, originPoint.y - 0.5f, originPoint.y + 0.5f);
            pos.z = -1;

            movingBox.transform.position = pos;

            //if()
        }
    }

    private void handleMouseMovement()
    {
        if (!grid_ref.IsMatch3Phase()) return;
        if (movingBox != null)
        {
            Vector2 direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseStart;
            Vector2 normalizedDir = direction.normalized;
            Vector2 absoluteDir = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            destinationPoint = movingBox.GetPoint().Clone();
            add = Point.zero.Clone();

            //if mouse moved one sprite distance from origin of the click
            if (direction.magnitude >= 0.5f)
            {
                if (absoluteDir.x > absoluteDir.y)
                {
                    add = new Point((normalizedDir.x > 0) ? 1 : -1, 0);
                }
                else if (absoluteDir.y > absoluteDir.x)
                {
                    add = new Point(0, (normalizedDir.y > 0) ? -1 : 1);
                }
            }

            destinationPoint.Add(add);

            destination = grid_ref.GetTile(destinationPoint);

            pos = destination.transform.position;
            pos.z = -1;

            if (!destinationPoint.Equals(movingBox.GetPoint().Clone()))
            {
                //pos += Point.byMagnitude(add.Clone(), -(int)boxWidth / 4).ToVector2();
                // pos += new Vector2(add.x / 2, add.y / 2 );
            }

            //print("trying to move from" + transform.position + "to " + pos);
            movingBox.MoveTo(pos);
        }
    }

    public void GrabBox(Box b)
    {
        if (movingBox != null || !grid_ref.IsMatch3Phase()) return;

        movingBox = b;
        movingBox.Highlight(true);

        mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // mouseStart = Input.mousePosition;
    }

    public void DropBox()
    {
        if (movingBox == null || !grid_ref.IsMatch3Phase()) return;

        if (destinationPoint == null || destination == null) return;

        bool cannotMatch = false;

        if (destination.GetBox() != null)
        {
            if (destination.GetBox().GetTriBox() != null)
            {
                cannotMatch = !destination.GetBox().GetTriBox().isMatchable();
            }
        }

        if (cannotMatch == false)
        {
            if (movingBox != null)
            {
                cannotMatch = !movingBox.GetTriBox().isMatchable();
            }
        }

        if (destinationPoint.Equals(movingBox.GetPoint().Clone()) || !destination.isMatchable() || !movingBox.GetTile().isMatchable() || cannotMatch == true)
        {
            // movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(movingBox.GetPoint()));
            movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(movingBox.GetPoint()), true, true, true, () => { });
            grid_ref.GetPlayer().playMissStep();

            movingBox.Highlight(false);
            Vector3 temp = movingBox.transform.position;
            temp.z = -1;
            movingBox.transform.position = temp;
            movingBox.transform.DOScale(Vector3.one, TheGrid.moveTime);

            movingBox = null;
        }
        else
        {
            //movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(destinationPoint),true, true);

            print("should try to swap");

            movingBox.TrySwap(destination.GetBox());

            //movingBox = null;
        }

        //if destination point is a a new point
        // check for match

        //if match == true
        // MoveTo(destination)
    }

    public void SetMovingBox(Box b)
    {
        movingBox = b;
    }

    public Box GetMovingBox()
    {
        return movingBox;
    }
}