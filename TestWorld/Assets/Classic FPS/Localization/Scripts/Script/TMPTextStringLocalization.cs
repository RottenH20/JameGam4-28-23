using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPTextStringLocalization : MonoBehaviour
{
    public LocalizedString ShowText;

    public TMP_Text text_tmp;

    private void Update() { if (ShowText.localization != null && ShowText.key != "") text_tmp.text = ShowText.localization.GetString(ShowText.key); }
    private void OnEnable() { if (ShowText.localization != null && ShowText.key != "") text_tmp.text = ShowText.localization.GetString(ShowText.key); }

    /*private void OnValidate()
    {
        if (ShowText.localization == null && localization != null) { ShowText.localization = localization; }
        if (ShowText.key == "" && keyLocalization != "") { ShowText.key = keyLocalization; }
    }

    [ContextMenu("Manually Convert Old Localization")]
    void GetOldLocalization()
    {
        ShowText.localization = localization;
        ShowText.key = keyLocalization;
    }*/
}
