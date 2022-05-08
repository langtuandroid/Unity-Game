using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombatWindow : EditorWindow
{
    private SerializedObject combat;
    public static void OpenWindow(SerializedObject c) { 
        CombatWindow window = GetWindow<CombatWindow>();
        window.combat = c;
        window.minSize = new Vector2(500, 400);
        window.Show();
    }

    private void OnEnable()
    {
        
    }

    private void OnGUI()
    {
        
    }


}
