using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceStorage
{
    private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    public static void LoadResource() {
        sprites["Circle"] = Resources.Load<Sprite>("Brackeys/2D Mega Pack/Shapes/Circle");
    }

    public static void SaveResource() { }

    public static Sprite GetSprite(string name) { 
        return GameObject.Instantiate(sprites[name]);
    }
}
