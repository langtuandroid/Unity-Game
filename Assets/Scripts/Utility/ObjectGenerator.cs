using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to generate termporary objects in the scene, can be used for displaying ui indicators etc
public static class ObjectGenerator
{
    public static void ModifyObject(GameObject obj, Transform transform, Vector3 position, Vector3 scale, Color color, int layer, int sortingLayer) {
        obj.transform.position = position;
        obj.GetComponent<Renderer>().material.color = color;
        obj.transform.localScale = scale;
        Vector3 eulers = obj.transform.localEulerAngles;
        obj.transform.rotation = Quaternion.Euler(eulers.x - 90, eulers.y, eulers.z);
        obj.transform.SetParent(transform);
        obj.layer = layer;
        obj.GetComponent<Renderer>().sortingLayerID = sortingLayer;
    }

    // Generate a temporary object
    public static GameObject Cylinder(Transform transform, Vector3 position, Vector3 scale, Color color, int layer, int sortingLayer, float time_in_sec) {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ModifyObject(cylinder, transform, position, scale, color, layer, sortingLayer);
        Object.Destroy(cylinder, time_in_sec);
        
        return cylinder;
    }


    public static GameObject GenerateSprite(Transform transform, Sprite sprite, Vector3 position, Quaternion rotation, Vector3 scale, Color color, int layer, int sortingLayer) {
        GameObject circle = new GameObject();
        circle.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = circle.GetComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingLayerID = sortingLayer;
        sr.sprite = sprite;

        Vector3 bounds = sr.sprite.bounds.size;
        circle.transform.position = position;
        circle.GetComponent<Renderer>().material.color = color;
        circle.transform.localScale = new Vector3(scale.x / bounds.x, scale.y / bounds.y, scale.z / bounds.z);
        Vector3 eulers = circle.transform.localEulerAngles;
        circle.transform.rotation = rotation;
        circle.transform.SetParent(transform);
        circle.layer = layer;
        
        return circle;
    }
}
