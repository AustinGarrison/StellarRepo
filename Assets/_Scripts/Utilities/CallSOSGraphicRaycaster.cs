using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace CallSOS.Utilities
{
    /// <summary>
    /// If canvas is worldspace and clickable, must change 
    /// eventData.Position to remain in the center of screen
    /// </summary>
    public class CallSOSGraphicRaycaster : GraphicRaycaster
    {
        public bool isClickableWorldSpace;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {

            base.Raycast(eventData, resultAppendList);

            // Check if the canvas is a world-space canvas
            Canvas canvas = GetComponent<Canvas>();
            if(canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Debug.Log(canvas.renderMode);
                return;
            }

            if (canvas.renderMode == RenderMode.WorldSpace && isClickableWorldSpace)
            {
                // Set the pointer event position to the center of the screen
                eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            }

            Debug.Log(eventData.position);

        }
    }
}