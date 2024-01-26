using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{

    public LayerMask layerToIgnore;

    private Transform sweepTransform;
    private float rotationspeed;
    private float radarDistance;
    private List<Collider2D> colliderList;

    private void Awake()
    {
        sweepTransform = transform.Find("Sweep");
        rotationspeed = 140f;
        radarDistance = 150f; // Width of sweepsprite
        colliderList = new List<Collider2D>();
    }

    private void Update()
    {
        float previousRotation = (sweepTransform.eulerAngles.z % 360) - 180;
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationspeed * Time.deltaTime);
        float currentRotation = (sweepTransform.eulerAngles.z % 360) - 180;

        if (previousRotation < 0 && currentRotation >= 0)
        {
            // Half rotation
            colliderList.Clear();
        }

        RaycastHit2D raycastHit2D =  Physics2D.Raycast(transform.position, GetVectorFromAngle(sweepTransform.eulerAngles.z), radarDistance, ~layerToIgnore);

        if (raycastHit2D.collider != null)
        {
            // Hit something
            if (!colliderList.Contains(raycastHit2D.collider))
            {
                SOSShipVisual sosShipVisual = raycastHit2D.collider.GetComponent<SOSShipVisual>();

                if(sosShipVisual != null )
                {
                    sosShipVisual.SetColor(Color.white);
                }

                // Hit this one for the first time
                colliderList.Add(raycastHit2D.collider);
            }
        }
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
