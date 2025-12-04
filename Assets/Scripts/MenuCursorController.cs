using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    [Tooltip("The custom cursor texture to display over menu elements")]
    public Texture2D menuCursor;

    [Tooltip("The default cursor texture")]
    public Texture2D defaultCursor;

    [Tooltip("Cursor hotspot offset (pivot point)")]
    public Vector2 cursorHotspot = Vector2.zero;

    private bool isOverMenu = false;

    void Start()
    {
        // Set the default cursor at start
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
        }
    }

    void Update()
    {
        // Check if mouse is over a UI element
        bool currentlyOverMenu = IsPointerOverUIElement();

        // Only change cursor if state has changed
        if (currentlyOverMenu != isOverMenu)
        {
            isOverMenu = currentlyOverMenu;

            if (isOverMenu && menuCursor != null)
            {
                // Mouse is over menu - use custom cursor
                Cursor.SetCursor(menuCursor, cursorHotspot, CursorMode.Auto);
            }
            else if (defaultCursor != null)
            {
                // Mouse left menu - use default cursor
                Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
            }
            else
            {
                // Reset to system cursor
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    private bool IsPointerOverUIElement()
    {
        // Check if the EventSystem exists
        if (EventSystem.current == null)
            return false;

        // Check if pointer is over a UI element
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Optional: Method to manually set cursor
    public void SetMenuCursor(bool useMenuCursor)
    {
        if (useMenuCursor && menuCursor != null)
        {
            Cursor.SetCursor(menuCursor, cursorHotspot, CursorMode.Auto);
        }
        else if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void OnDisable()
    {
        // Reset cursor when script is disabled
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}