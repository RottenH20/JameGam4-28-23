using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(StringLocalization))]
public class StringLocalizationEditor : Editor
{
    SerializedProperty wordlist;
    ReorderableList list;
    StringLocalization ld;

    private void OnEnable()
    {
        wordlist = serializedObject.FindProperty("wordList");
        list = new ReorderableList(serializedObject, wordlist, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
        ld = target as StringLocalization;
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        list.elementHeight = (EditorGUIUtility.singleLineHeight + 2) * (2 + ld._manager.lang.Count) + 5;

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 250, EditorGUIUtility.singleLineHeight), "Key");
        EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y, rect.width - 250, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("key"),GUIContent.none);
        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, 125, EditorGUIUtility.singleLineHeight), "English (Defualt)");
        EditorGUI.PropertyField(new Rect(rect.x + 125, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width - 125, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("englishText"), GUIContent.none);

        for(int i = 0; i < ld._manager.lang.Count; i++)
        {
            while (ld.wordList[index].stringList.Count < ld._manager.lang.Count) { ld.wordList[index].stringList.Add(""); }
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((EditorGUIUtility.singleLineHeight + 2) * (2 + i)), 125, EditorGUIUtility.singleLineHeight), ld._manager.lang[i].ToString());
            EditorGUI.PropertyField(new Rect(rect.x + 125, rect.y + ((EditorGUIUtility.singleLineHeight + 2) * (2 + i)), rect.width - 125, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("stringList").GetArrayElementAtIndex(i), GUIContent.none);
        }


    }
    void DrawHeader(Rect rect)
    {
        string name = $"{ld.name} - String Table Localization";
        EditorGUI.LabelField(rect, name);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
