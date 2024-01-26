using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip_SOSShipLocator : MonoBehaviour
{
    public List<SOSShipVisual> shipsInRange;

    private void Awake()
    {
        shipsInRange = new List<SOSShipVisual>();   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SOSShipMap"))
        {
            SOSShipVisual sosShipVisual = other.GetComponent<SOSShipVisual>();
            shipsInRange.Add(sosShipVisual);
            sosShipVisual.ShowSelectedSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SOSShipMap"))
        {
            SOSShipVisual sosShipVisual = other.GetComponent<SOSShipVisual>();
            shipsInRange.Remove(sosShipVisual);
            sosShipVisual.ShowUnselectedSprite();
        }
    }
}
