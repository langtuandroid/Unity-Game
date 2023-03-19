using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace LobsterFramework
{
    public static class FindMissingScript
    {
        [MenuItem("Script/Find Missing Script In Prefab")]
        public static void FindMissingScriptInPrefab() { 
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (string prefabPath in prefabPaths) { 
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                foreach (Component component in prefab.GetComponentsInChildren<Component>()) {
                    if (component == null) {
                        Debug.Log("Prefab found with missing script " + prefabPath, prefab);
                        break;
                    }
                }
            }
        }

        [MenuItem("Script/Find Missing Script In Scene")]
        public static void FindMissingScriptInScene() {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>(true)) {
                foreach (Component component in obj.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        Debug.Log("Prefab found with missing script " + obj.name, obj);
                        break;
                    }
                }
            }
        }
    }
}
