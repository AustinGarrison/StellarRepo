using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CallSOS.Utilities.CursorController;

namespace CallSOS.Utilities
{
    public enum CursorType
    {
        None,
        UI,
        Point,
        Inspect,
        Grab,
    }

    public class CursorController : MonoBehaviour
    {
        public static CursorController Instance { get; private set; }

        [System.Serializable]
        public struct CursorMapping
        {
            public string cursorName;
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        public bool useCursors = false;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance);
            }
            else
            {
                Instance = this;
            }
        }

        public void SetCursor(CursorType type)
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
}