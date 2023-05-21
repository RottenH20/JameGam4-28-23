using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCampaign : MonoBehaviour
{
    [Line("Base")]
    public LevelCategory category;
    public CampaignManager campaign;

    [Line("Containers")]
    public GameObject ChapterContainer;
    public GameObject LevelContainer;
    public GameObject LevelPanel;

    [Line("Prefabs")]
    public GameObject ChapterPrefab;
    public GameObject LevelPrefab;

    public void Start()
    {
        LevelPanel.SetActive(false);
        foreach (Transform child in ChapterContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in LevelContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (category == LevelCategory.Chapter)
        {
            for (int i = 0; i < campaign.chapter.Count; i++)
            {
                int j = i;
                GameObject rocketInstantiated = (GameObject)Instantiate(ChapterPrefab, ChapterContainer.transform);
                rocketInstantiated.GetComponent<TMPTextStringLocalization>().ShowText.localization = campaign.localization;
                rocketInstantiated.GetComponent<TMPTextStringLocalization>().ShowText.key = campaign.chapter[i].key;

                Button btn = rocketInstantiated.GetComponent<Button>();
                btn.onClick.AddListener(delegate () { GenerateLevelList(j); });
            }
        }
        else if (category == LevelCategory.Gamemode)
        {
            for (int i = 0; i < campaign.gamemodes.Count; i++)
            {
                int j = i;
                GameObject rocketInstantiated = (GameObject)Instantiate(ChapterPrefab, ChapterContainer.transform);
                rocketInstantiated.GetComponent<TMPTextStringLocalization>().ShowText.localization = campaign.localization;
                rocketInstantiated.GetComponent<TMPTextStringLocalization>().ShowText.key = campaign.gamemodes[i].key;

                Button btn = rocketInstantiated.GetComponent<Button>();
                btn.onClick.AddListener(delegate () { GenerateLevelList(j); });
            }
        }
    }

    public void GenerateLevelList(int chapterID)
    {
        foreach (Transform child in LevelContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (category == LevelCategory.Chapter)
        {
            for (int i = 0; i < campaign.chapter[chapterID].Levels.Count; i++)
            {
                int j = i;
                GameObject rocketInstantiated = (GameObject)Instantiate(LevelPrefab, LevelContainer.transform);
                rocketInstantiated.transform.Find("name_txt").GetComponent<TMPTextStringLocalization>().ShowText.key = campaign.chapter[chapterID].Levels[i].key;
                Trophy _tmpTrophy = TinySaveSystem.GetTrophy(campaign.chapter[chapterID].Levels[i].sceneName);
                rocketInstantiated.transform.Find("trophy_txt").GetComponent<TMPTextStringLocalization>().ShowText.key = GenerateTrophy(_tmpTrophy);

                Button btn = rocketInstantiated.GetComponent<Button>();
                btn.onClick.AddListener(delegate () { OpenLevel(campaign.chapter[chapterID].Levels[j].sceneName); });
            }
        }
        else if (category == LevelCategory.Gamemode)
        {
            for (int i = 0; i < campaign.gamemodes[chapterID].Levels.Count; i++)
            {
                int j = i;
                GameObject rocketInstantiated = (GameObject)Instantiate(LevelPrefab, LevelContainer.transform);
                rocketInstantiated.transform.Find("name_txt").GetComponent<TMPTextStringLocalization>().ShowText.key = campaign.gamemodes[chapterID].Levels[i].key;
                Trophy _tmpTrophy = TinySaveSystem.GetTrophy(campaign.gamemodes[chapterID].Levels[i].sceneName);
                rocketInstantiated.transform.Find("trophy_txt").GetComponent<TMPTextStringLocalization>().ShowText.key = GenerateTrophy(_tmpTrophy);

                Button btn = rocketInstantiated.GetComponent<Button>();
                btn.onClick.AddListener(delegate () { OpenLevel(campaign.gamemodes[chapterID].Levels[j].sceneName); });
            }
        }
        LevelPanel.SetActive(true);

        void OpenLevel(string name)
        {
            LoadingScreen.instance.loadingScreen(name);
        }
    }

    string GenerateTrophy(Trophy trophy)
    {
        switch (trophy.GetCount())
        {
            case 1:
                return "bronze";
            case 2:
                return "silver";
            case 3:
                return "gold";
            case 4:
                return "platinium";
            case 5:
                return "diamond";
            default:
                return "none";
        }
    }

    public enum LevelCategory
    {
        Chapter, Gamemode
    }
}
