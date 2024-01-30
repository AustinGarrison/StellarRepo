using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CursorType
{
    None,
    UI,
    Point,
    Open,
    Grab,
}

public class CursorController : MonoBehaviour
{
    [System.Serializable]
    struct CursorMapping
    {
        public string cursorName;
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot;
    }

    [SerializeField] CursorMapping[] cursorMappings = null;
    [SerializeField] float raycastRadius = 1f;

    public bool useCursors = false;
    bool isDraggingUI = false;

    private void Update()
    {
        if (InteractWithUI()) return;

        SetCursor(CursorType.None);
    }

    private bool InteractWithUI()
    {
        if(Input.GetMouseButtonUp(0))
        {
            isDraggingUI = false;
        }

        if(EventSystem.current.IsPointerOverGameObject(0))
        {
            if(Input.GetMouseButtonDown(0))
            {
                isDraggingUI = true;
            }

            SetCursor(CursorType.UI);

            return true;
        }    

        if(isDraggingUI)
        {
            return true;
        }

        return false;
    }

    void InteractWith3D()
    {

    }

    private void SetCursor(CursorType type)
    {
        if (useCursors)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
    }

    private CursorMapping GetCursorMapping(CursorType type)
    {
        foreach (CursorMapping mapping in cursorMappings)
        {
            if (mapping.type == type)
            {
                return mapping;
            }
        }
        return cursorMappings[0];
    }
}
