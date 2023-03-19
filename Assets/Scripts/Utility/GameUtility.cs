using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtility
{
    public static void SetAbsoluteScale(GameObject obj, Vector2 size) {
        Transform trans = obj.transform;
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        Vector3 scale = trans.lossyScale;
        trans.localScale = new Vector3(size.x / scale.x, size.y / scale.y, 1);
    }
}
