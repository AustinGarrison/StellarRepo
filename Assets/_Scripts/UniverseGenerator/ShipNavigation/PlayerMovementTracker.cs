using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
    public Collider2D boxCollider2d;
    internal Universe universe;

    // Called from PlayerShipNavController
    public void Init(Transform gridCenter, Universe universe)
    {
        this.universe = universe;

        // Set position to center
        transform.position = gridCenter.position;
    }

    private void Update()
    {
        if (boxCollider2d == null)
        {
            Debug.LogError("BoxCollider not assigned");
        }

        // When the Mover crosses the bounds of its parent boxcollider, teleport it
        if(!boxCollider2d.bounds.Contains(transform.position))
        {
            TeleportToOtherSide();
        }
    }

    private void TeleportToOtherSide()
    {
        // Get the center and size of the box collider
        Vector2 colliderCenter = boxCollider2d.bounds.center;
        Vector2 colliderSize = boxCollider2d.bounds.size;

        // Calculate the new position on the other side
        float newX = WrapCoordinate(transform.position.x, colliderCenter.x - colliderSize.x / 2, colliderCenter.x + colliderSize.x / 2);
        float newY = WrapCoordinate(transform.position.y, colliderCenter.y - colliderSize.y / 2, colliderCenter.y + colliderSize.y / 2);

        CalculateNewGrid();

        // Teleport the object to the new position
        transform.position = new Vector2(newX, newY);
    }

    private void CalculateNewGrid()
    {
        bool hasLeft = false;

        Universe.MoveChunkDir moveDirection = Universe.MoveChunkDir.None;

        // Exited on the Right
        if (transform.position.x > boxCollider2d.bounds.max.x && !hasLeft)
        {
            hasLeft = true;
            moveDirection = Universe.MoveChunkDir.East;
        }

        // Exited on the Left
        if (transform.position.x < boxCollider2d.bounds.min.x && !hasLeft)
        {
            hasLeft = true;
            moveDirection = Universe.MoveChunkDir.West;
        }

        // Exited on the Top
        if (transform.position.y > boxCollider2d.bounds.max.y && !hasLeft)
        {
            hasLeft = true;
            moveDirection = Universe.MoveChunkDir.North;
        }

        // Exited on the bottom
        if (transform.position.y < boxCollider2d.bounds.min.y && !hasLeft)
        {
            moveDirection = Universe.MoveChunkDir.South;
        }
        universe.MoveChunks(moveDirection);
    }


    private float WrapCoordinate(float coordinate, float min, float max)
    {
        // Wrap the coordinate to the other side if it goes beyond the min or max
        if (coordinate < min)
        {
            return max - (min - coordinate);
        }
        else if (coordinate > max)
        {
            return min + (coordinate - max);
        }
        else
        {
            return coordinate;
        }
    }
}
