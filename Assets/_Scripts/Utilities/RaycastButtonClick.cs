using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastButtonClick : MonoBehaviour
{
    public void TestButton()
    {
        Debug.Log("TestButton");
    }

    void Update()
    {
        //// Check for mouse click or touch input
        //if (Input.GetMouseButtonDown(0))
        //{
        //    PointerEventData eventData = new PointerEventData(EventSystem.current);

        //    // Set the position of the pointer event data to the center of the screen
        //    eventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //    // Raycast to detect UI elements
        //    List<RaycastResult> results = new List<RaycastResult>();

        //    EventSystem.current.RaycastAll(eventData, results);

        //    if (results.Count > 0)
        //    {
        //        // Simulate a click on the first UI element in the list
        //        ExecuteEvents.Execute(results[1].gameObject, eventData, ExecuteEvents.pointerClickHandler);
        //    }
        //}
    }
}
