using UnityEngine;
using UnityEditor;

public class HierarchySeparator : MonoBehaviour
{
    [Line("Title")]
    public string title = "Header";
    [Range(16, 48)] public int MaxCharacter = 24;
    [ShowIf("type", (int)SeparatorPrefixType.Custom)] public char CharacterBefore = '<';
    [ShowIf("type", (int)SeparatorPrefixType.Custom)] public char CharacterAfter = '>';

    [Line("Decoration")]
    public SeparatorPrefixType type;

#if UNITY_EDITOR

    private void OnValidate()
    {
        switch (type)
        {
            case SeparatorPrefixType.Comment:
                // Prefix
                string start_line = ""; string end_line = "";
                for (int i = 0; i < Mathf.RoundToInt((MaxCharacter - title.Length - 2) / 2); i++) { start_line += " "; end_line += " "; }
                // Title
                this.name = $"/* {start_line}" + title.ToUpperInvariant() + $"{end_line} */";

                break;

            case SeparatorPrefixType.Equal:
                // Prefix
                string start_Equal = ""; string end_Equal = "";
                for (int i = 0; i < Mathf.RoundToInt((MaxCharacter - title.Length - 2) / 2); i++) { start_Equal += "="; end_Equal += "="; }
                // Title
                this.name = $"{start_Equal} " + title.ToUpperInvariant() + $" {end_Equal}";

                break;

            case SeparatorPrefixType.Custom:
                // Prefix
                string start_custom = ""; string end_custom = "";
                for (int i = 0; i < Mathf.RoundToInt((MaxCharacter - title.Length - 2) / 2); i++) { start_custom += CharacterBefore; end_custom += CharacterAfter; }
                // Title
                this.name = $"{start_custom} " + title.ToUpperInvariant() + $" {end_custom}";

                break;

            default:
                // Prefix
                string start = ""; string end = "";
                for (int i = 0; i < Mathf.RoundToInt((MaxCharacter - title.Length - 2) / 2); i++) { start += "━"; end += "━"; }
                // Title
                this.name = $"{start} {title.ToUpperInvariant()} {end}";

                break;
        }
        transform.position = Vector3.zero;
        transform.gameObject.SetActive(false);
    }

    [MenuItem("GameObject/Add Separator", false, 0)]
    private static void CreateHeader()
    {
        var header = new GameObject();
        header.tag = "EditorOnly";
        header.AddComponent<HierarchySeparator>();
        header.transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        Undo.RegisterCreatedObjectUndo(header, "Add Hierarchy Separator");
        Selection.activeGameObject = header;
    }

#endif
}


public enum SeparatorPrefixType
{
    Line, Comment, Equal, Custom
}