using UnityEngine;

public static class Extensions
{
    public static void SetParent(this GameObject go, GameObject newParent)
    {
        go.transform.parent = newParent.transform;
    }

    public static void SetParent(this GameObject go, Transform newParent)
    {
        go.transform.parent = newParent;
    }

    public static void SetParentRelative(this GameObject go, GameObject newParent)
    {
        var newPos = go.transform.localPosition;
        var savedScale = go.transform.localScale;
        go.transform.parent = newParent.transform;
        go.transform.localPosition = newPos;
        go.transform.localScale = savedScale;
    }

    public static void SetParentRelative(this GameObject go, Transform newParent)
    {
        go.transform.position += newParent.position;
        var savedScale = go.transform.localScale;
        go.transform.parent = newParent;
        go.transform.localScale = savedScale;
    }
}