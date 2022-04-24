using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to generate termporary objects in the scene, can be used for displaying ui indicators etc
public static class TemporaryObject
{
    // Generate a temporary object
    public static GameObject Cylinder(Transform transform, Vector3 position, Vector3 scale, Color color, float time_in_sec, int layer, int sortingLayer) {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
        cylinder.GetComponent<Renderer>().material.color = color;
        cylinder.transform.localScale = scale;
        Vector3 eulers = cylinder.transform.localEulerAngles;
        cylinder.transform.rotation = Quaternion.Euler(eulers.x - 90, eulers.y, eulers.z);
        cylinder.transform.SetParent(transform);
        Object.Destroy(cylinder, time_in_sec);
        cylinder.layer = layer;
        cylinder.GetComponent<Renderer>().sortingLayerID = sortingLayer;
        return cylinder;
    }


    public static GameObject SpriteCircle(Transform transform, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float time_in_sec, int layer, int sortingLayer) {
        GameObject circle = new GameObject();
        circle.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = circle.GetComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingLayerID = sortingLayer;
        sr.sprite = ResourceStorage.GetSprite("Circle");

        Vector3 bounds = sr.sprite.bounds.size;
        circle.transform.position = position;
        circle.GetComponent<Renderer>().material.color = color;
        circle.transform.localScale = new Vector3(scale.x / bounds.x, scale.y / bounds.y, scale.z / bounds.z);
        Vector3 eulers = circle.transform.localEulerAngles;
        circle.transform.rotation = rotation;
        circle.transform.SetParent(transform);
        circle.layer = layer;
        
        Object.Destroy(circle, time_in_sec);
        return circle;
    }
}
