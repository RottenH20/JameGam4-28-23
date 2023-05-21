using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TextureLocalization))]
public class TextureLocalizationEditor : Editor
{
    SerializedProperty wordlist;
    ReorderableList list;
    TextureLocalization ld;

    private void OnEnable()
    {
        wordlist = serializedObject.FindProperty("wordList");
        list = new ReorderableList(serializedObject, wordlist, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
        ld = target as TextureLocalization;
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        list.elementHeight = (EditorGUIUtility.singleLineHeight + 2) * (2 + ld._manager.lang.Count) + 5;

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 250, EditorGUIUtility.singleLineHeight), "Key");
        EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y, rect.width - 250, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("key"), GUIContent.none);
        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, 125, EditorGUIUtility.singleLineHeight), "English (Defualt)");
        EditorGUI.ObjectField(new Rect(rect.x + 125, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width - 125, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("englishText"), typeof(Texture2D), GUIContent.none);

        for (int i = 0; i < ld._manager.lang.Count; i++)
        {
            while (ld.wordList[index].textureList.Count < ld._manager.lang.Count) { ld.wordList[index].textureList.Add(null); }
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((EditorGUIUtility.singleLineHeight + 2) * (2 + i)), 125, EditorGUIUtility.singleLineHeight), ld._manager.lang[i].ToString());
            EditorGUI.ObjectField(new Rect(rect.x + 125, rect.y + ((EditorGUIUtility.singleLineHeight + 2) * (2 + i)), rect.width - 125, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("textureList").GetArrayElementAtIndex(i), typeof(Texture2D), GUIContent.none);
        }


    }
    void DrawHeader(Rect rect)
    {
        string name = $"{ld.name} - Texture Table Localization";
        EditorGUI.LabelField(rect, name);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
