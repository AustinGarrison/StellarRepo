using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CallSOS.Printer.UI
{
    public class WorldSpaceGraphicRaycaster : GraphicRaycaster
    {
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            // Check if the canvas is a world-space canvas
            Canvas canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                // Set the pointer event position to the center of the screen
                eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            }

            base.Raycast(eventData, resultAppendList);
        }
    }
}