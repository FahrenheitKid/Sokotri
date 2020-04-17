﻿using UnityEngine;

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
    private Vector2 pos;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (movingBox)
            {
                Point p = movingBox.GetPoint().Clone();
                p.Add(2, 0);
                movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(p) + (Vector2)grid_ref.transform.position, true, false, true);
            }
        }

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

            movingBox.transform.position = pos;

            //if()
        }
    }

    private void handleMouseMovement()
    {
        if (!grid_ref.IsMatch3Phase()) return;
        if (movingBox != null)
        {
            movingBox.Highlight(true);
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

            if (!destinationPoint.Equals(movingBox.GetPoint().Clone()))
            {
                //pos += Point.byMagnitude(add.Clone(), -(int)boxWidth / 4).ToVector2();
                // pos += new Vector2(add.x / 2, add.y / 2 );
            }

            if (movingBox.transform.parent != null)
            {
                movingBox.MoveTo(pos);
            }
            else
            {
                movingBox.MoveTo(pos);
            }
        }
    }

    public void GrabBox(Box b)
    {
        if (movingBox != null || !grid_ref.IsMatch3Phase()) return;

        movingBox = b;

        mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // mouseStart = Input.mousePosition;
    }

    public void DropBox()
    {
        movingBox.Highlight(false);

        if (movingBox == null || !grid_ref.IsMatch3Phase()) return;

        if (destinationPoint == null || destination == null) return;
        if (destinationPoint.Equals(movingBox.GetPoint().Clone()) || !destination.isMatchable())
        {
            movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(movingBox.GetPoint()));
        }
        else
        {
            // movingBox.MoveTo(grid_ref.getExpectedPositionFromPoint(destinationPoint),true, true);

            print("should try to swap");

            movingBox.Swap(destination.GetBox());
        }

        //if destination point is a a new point
        // check for match

        //if match == true
        // MoveTo(destination)

        movingBox = null;
    }
}