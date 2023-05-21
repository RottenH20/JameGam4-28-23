
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class TinySaveEditor : EditorWindow
{
    static string fileName = "HellishBattle.tss";
    Vector2 scrollPosition;

    [MenuItem("Tools/Tiny Save Editor")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TinySaveEditor));
    }

    void OnGUI()
    {
        TinySaveSystem.Initialize(fileName);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginVertical("Toolbar", GUILayout.Height(30));
        EditorGUILayout.LabelField("Loaded Record: " + TinySaveSystem.data.items.Count.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        fileName = EditorGUILayout.TextField("Save File Name", fileName);
        EditorGUILayout.EndVertical();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < TinySaveSystem.data.items.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(30));
            EditorGUILayout.LabelField(TinySaveSystem.data.items[i].Key, EditorStyles.boldLabel);
            if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.Width(75)))
            {

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Value: " + DeCrypt(TinySaveSystem.data.items[i].Value));
            if (GUILayout.Button("Edit", GUILayout.Width(75)))
            {

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

    }

    static string DeCrypt(string text)
    {
        string result = string.Empty;
        foreach (char j in text) result += (char)((int)j! ^ 42);
        return result;
    }
}

#endif