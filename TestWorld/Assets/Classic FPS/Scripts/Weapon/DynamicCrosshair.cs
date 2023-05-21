/*
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.1.0a
*/

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DynamicCrosshair : MonoBehaviour
{
    public float reducedGradually = 30f;
    public float maxSpread = 180f;

    // Timed
    public const int JUMP_SPREAD = 50;
    public const int WALK_SPREAD = 10;
    public const int RUN_SPREAD = 25;

    [Tooltip("Center Object of Crosshair")]
    public GameObject crosshair;


    static public float spread = 0;

    // Privates
    GameObject cross_topPart;
    GameObject cross_bottomPart;
    GameObject cross_leftPart;
    GameObject cross_rightPart;
    GameObject y_bottomPart;
    GameObject y_leftPart;
    GameObject y_rightPart;
    GameObject dot;

    // Find the relevant parts of the crosshair and their initial positions
    void Start()
    {
        cross_topPart = crosshair.transform.Find("Cross_Top").gameObject;
        cross_bottomPart = crosshair.transform.Find("Cross_Bottom").gameObject;
        cross_leftPart = crosshair.transform.Find("Cross_Left").gameObject;
        cross_rightPart = crosshair.transform.Find("Cross_Right").gameObject;
        y_bottomPart = crosshair.transform.Find("Y_Bottom").gameObject;
        y_leftPart = crosshair.transform.Find("Y_Left").gameObject;
        y_rightPart = crosshair.transform.Find("Y_Right").gameObject;
        dot = crosshair.transform.Find("Center").gameObject;

        if(TinySaveSystem.GetInt("settings_crosshairEnable") == 0) // Enable
        {
            // Type
            if (TinySaveSystem.GetInt("settings_crosshairType") == 0) // X
            {
                cross_topPart.SetActive(true); cross_bottomPart.SetActive(true); cross_leftPart.SetActive(true); cross_rightPart.SetActive(true);
                y_bottomPart.SetActive(false); y_leftPart.SetActive(false); y_rightPart.SetActive(false);
                dot.SetActive(true);
            }
            if (TinySaveSystem.GetInt("settings_crosshairType") == 1) // Y
            {
                cross_topPart.SetActive(false); cross_bottomPart.SetActive(false); cross_leftPart.SetActive(false); cross_rightPart.SetActive(false);
                y_bottomPart.SetActive(true); y_leftPart.SetActive(true); y_rightPart.SetActive(true);
                dot.SetActive(true);
            }
            if (TinySaveSystem.GetInt("settings_crosshairType") == 2) // - -
            {
                cross_topPart.SetActive(false); cross_bottomPart.SetActive(false); cross_leftPart.SetActive(true); cross_rightPart.SetActive(true);
                y_bottomPart.SetActive(false); y_leftPart.SetActive(false); y_rightPart.SetActive(false);
                dot.SetActive(true);
            }
            // Dot
            bool dotEnable = true;
            if(TinySaveSystem.GetInt("settings_crosshairDot") == 1) { dotEnable = false; }
            dot.SetActive(dotEnable);

            // Size

            float Size = 0;

            switch (TinySaveSystem.GetInt("settings_crosshairSize"))
            {
                case 0:
                    Size = .5f;
                    break;
                case 2:
                    Size = 1.5f;
                    break;
                case 3:
                    Size = 2f;
                    break;
                default:
                    Size = 1;
                    break;
            }
            crosshair.transform.localScale = new Vector3(Size, Size, Size);

            // Color
            dot.transform.GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            cross_topPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            cross_bottomPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            cross_leftPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            cross_rightPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            y_bottomPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            y_leftPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);
            y_rightPart.transform.Find("Image").GetComponent<Image>().color = new Color(TinySaveSystem.GetInt("settings_crosshairColor_r") / 255, TinySaveSystem.GetInt("settings_crosshairColor_g") / 255, TinySaveSystem.GetInt("settings_crosshairColor_b") / 255);

            // Width & Height
            float width = 0;
            float height = 0;

            switch (TinySaveSystem.GetInt("settings_crosshairWidth"))
            {
                case 0:
                    width = .5f;
                    break;
                case 2:
                    width = 2;
                    break;
                default:
                    width = 1;
                    break;
            }

            switch (TinySaveSystem.GetInt("settings_crosshairHeight"))
            {
                case 0:
                    height = 1;
                    break;
                case 2:
                    height = 4;
                    break;
                default:
                    height = 2;
                    break;
            }

            cross_topPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            cross_bottomPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            cross_leftPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            cross_rightPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            y_bottomPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            y_leftPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            y_rightPart.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            // OFFSET

            float offset = 0;
            switch (TinySaveSystem.GetInt("settings_crosshairOffset"))
            {
                case 0:
                    offset = 0;
                    break;
                case 1:
                    offset = .5f;
                    break;
                case 2:
                    offset = 1;
                    break;
                case 4:
                    offset = 2;
                    break;
                case 5:
                    offset = 2.5f;
                    break;
                case 6:
                    offset = 3f;
                    break;
                default:
                    offset = 1.5f;
                    break;
            }

            cross_topPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            cross_bottomPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            cross_leftPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            cross_rightPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            y_bottomPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            y_leftPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);
            y_rightPart.transform.Find("Image").localPosition = new Vector3(0, -offset, 0);

        }
        else // Disable
        {
            cross_topPart.SetActive(false); cross_bottomPart.SetActive(false); cross_leftPart.SetActive(false); cross_rightPart.SetActive(false);
            y_bottomPart.SetActive(false); y_leftPart.SetActive(false); y_rightPart.SetActive(false);
            dot.SetActive(false);
        }
    }

    void Update()
    {
        // Change the position of the crosshair part depending on the value of the variable 'spread'
        // If the variable 'spread' is greater than 0 then the spread is reduced gradually
        if (spread > 0 && TinySaveSystem.GetInt("settings_crosshairDynamic") == 0)
        {
            cross_topPart.GetComponent<RectTransform>().localPosition = new Vector3(0, spread, 0);
            cross_bottomPart.GetComponent<RectTransform>().localPosition = new Vector3(0, -spread, 0);
            cross_leftPart.GetComponent<RectTransform>().localPosition = new Vector3(-spread,0, 0);
            cross_rightPart.GetComponent<RectTransform>().localPosition = new Vector3(spread, 0, 0);

            y_rightPart.GetComponent<RectTransform>().localPosition = new Vector3((spread / 2) * Mathf.Sqrt(3), (spread / 2), 0);
            y_leftPart.GetComponent<RectTransform>().localPosition = new Vector3(-((spread / 2) * Mathf.Sqrt(3)), (spread / 2), 0);
            y_bottomPart.GetComponent<RectTransform>().localPosition = new Vector3(0, -spread, 0);

            spread -= reducedGradually * Time.deltaTime;
        }
        if (maxSpread < spread) { spread = maxSpread; }
    }
}