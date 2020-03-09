using UnityEngine;

public static class RaycastUtil
{
    public static Tile RaycastTile(Vector3? position = null)
    {
        Ray ray;
        if (position.HasValue)
        {
            var startPos = Camera.main == null ? new Vector3(0, 0, -10) : Camera.main.transform.position;
            ray = new Ray(startPos, position.Value - startPos);
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        if (hit.collider != null)
        {
            return hit.collider.transform.gameObject.GetComponent<Tile>();
        }
        return null;
    }

    public static bool IsMine(Vector3 location)
    {
        var tile = RaycastTile(location);
        if (tile != null && tile.GetType() == typeof(Mine))
        {
            return true;
        }
        return false;
    }
}
