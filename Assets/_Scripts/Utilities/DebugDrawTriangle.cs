using Mono.CSharp;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugDrawTriangle : MonoBehaviour
{
    public float length;
    public float angleOne;
    public float angleTwo;
    public Material triangleShader;
    public Button drawButton;

    public TextMeshProUGUI text;

    public GameObject triangle;

    // Scaling factor for the triangle
    public float scale = 1.0f;

    void Start()
    {
        drawButton.onClick.AddListener(DrawTriangle);
    }

    void DrawTriangle()
    {
        float lengthTwo, lengthThree, angleC;

        if(triangle != null)
        {
            Destroy(triangle);
        }

        CalculateTriangle(angleOne, angleTwo, length, out lengthTwo, out lengthThree, out angleC);

        // Create a new GameObject for the triangle
        GameObject triangleObject = new GameObject("Triangle");
        triangle = triangleObject;

        // Add LineRenderer component to draw lines for the triangle
        LineRenderer lineRenderer = triangleObject.AddComponent<LineRenderer>();

        // Set the material or shader for the lines
        lineRenderer.material = triangleShader;

        // Set the number of vertices (points) of the line renderer
        lineRenderer.positionCount = 4; // Three vertices for the triangle and one to close the loop

        // Set the width of the lines
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Set the positions of the vertices to form the triangle
        lineRenderer.SetPosition(0, Vector3.zero); // Start from the origin (0,0,0)
        lineRenderer.SetPosition(1, new Vector3(length, 0, 0)); // Move along the x-axis by the length of side 1
        lineRenderer.SetPosition(2, new Vector3(lengthThree * Mathf.Cos(angleC * Mathf.Deg2Rad), lengthThree * Mathf.Sin(angleC * Mathf.Deg2Rad), 0)); // Calculate position for side 3
        lineRenderer.SetPosition(3, Vector3.zero); // Close the triangle by connecting back to the origin
    }

    public void CalculateTriangle(float angleA, float angleB, float lengthA, out float lengthB, out float lengthC, out float angleC)
    {
        float angleARad = angleA * (Mathf.PI / 180);
        float angleBRad = angleB * (Mathf.PI / 180);

        lengthB = Mathf.Sin(angleARad) * lengthA / Mathf.Sin(angleBRad);
        lengthC = Mathf.Sin(angleBRad) * lengthA / Mathf.Sin(angleARad);

        angleC = 180f - angleA - angleB;

        text.text = String.Format("Side 1: {0}\nSide 2: {1}\nSide 3: {2}\nAngle 1: {3}\nAngle 2: {4}\nAngle 3: {5}", lengthA, lengthB, lengthC, angleA, angleB, angleC);
    }
}
