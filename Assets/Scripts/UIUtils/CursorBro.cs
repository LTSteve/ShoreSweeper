using UnityEngine;

public static class CursorBro
{
    private static Vector2 offset = new Vector2(12, 24);
    public static void Do(int cursor)
    {
        Cursor.SetCursor(Director.D.CursorTextures[cursor], offset, CursorMode.ForceSoftware);
    }
}