using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
    public Collider2D boxCollider2d;
    internal Universe universe;
    private Vector3 center;

    public void Init(Transform gridCenter, Universe universe)
    {
        this.universe = universe;
        center = gridCenter.position;
        transform.position = center;
    }

    private void Update()
    {
        if (boxCollider2d == null)
        {
            Debug.LogError("BoxCollider not assigned");
        }

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

        UpdateCurrentGrid();

        // Teleport the object to the new position
        transform.position = new Vector2(newX, newY);
    }

    private void UpdateCurrentGrid()
    {
        bool hasLeftX = false;
        bool hasLeftY = false;

        // Exited on the Right
        if (transform.position.x > boxCollider2d.bounds.max.x && !hasLeftX)
        {
            hasLeftX = true;
            universe.MoveChunks(Universe.MoveChunkDir.East);
            //universe.currentCenterChunkX += 1;
        }

        // Exited on the Left
        if (transform.position.x < boxCollider2d.bounds.min.x && !hasLeftX)
        {
            hasLeftX = true;
            universe.MoveChunks(Universe.MoveChunkDir.West);
            //universe.currentCenterChunkX -= 1;
        }

        // Exited on the Top
        if (transform.position.y > boxCollider2d.bounds.max.y && !hasLeftY)
        {
            hasLeftY = true;
            universe.MoveChunks(Universe.MoveChunkDir.North);
            //universe.currentCenterChunkY += 1;
        }

        // Exited on the bottom
        if (transform.position.y < boxCollider2d.bounds.min.y && !hasLeftY)
        {
            hasLeftY = true;
            universe.MoveChunks(Universe.MoveChunkDir.South);
            //universe.currentCenterChunkY -= 1;
        }
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
