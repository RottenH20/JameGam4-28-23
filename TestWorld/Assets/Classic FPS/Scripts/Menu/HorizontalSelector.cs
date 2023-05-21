using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HorizontalSelector : MonoBehaviour
{
    public bool tryLocalizedText = false;
    public StringLocalization localization;

    public List<string> KeyList;
    public TMP_Text Text;
    public int startIndex;
    public UnityEvent OnChange;

    public int index;

    private void Start()
    {
        index = startIndex;
        OnChange.Invoke();
    }

    public void Update()
    {
        if (tryLocalizedText) { Text.text = localization.GetString(KeyList[index]); }
        else { Text.text = KeyList[index]; }
    }

    public void Previous()
    {
        index -= 1;
        if (index < 0) { index = KeyList.Count - 1; }
        OnChange.Invoke();
    }

    public void Next()
    {
        index += 1;
        if (index > KeyList.Count - 1) { index = 0; }
        OnChange.Invoke();
    }

    public void ChangeLanguage(SettingsManager sm)
    {
        sm.ChangeLanguage(index - 1);
    }
}
