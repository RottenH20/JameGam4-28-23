/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBillboardChange : MonoBehaviour {

    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    string[] animStates = new string[4] { "Forward", "Backward", "Right", "Left" };
    [SerializeField]
    string deathAnim = "Death";
    [SerializeField]
    Sprite deathSprite;
    [SerializeField]
    Enemy enemy;
    [SerializeField]
    bool isAnimated;

    Animator anim;
    SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        GetAngle();
    }

    void GetAngle()
    {
        if (enemy != null && enemy.health <= 0)
        {
            ChangeSprite(-1);
        }
        else
        {
            Vector3 playerDir = Camera.main.transform.forward;
            playerDir.y = 0;
            Vector3 enemyDir = transform.Find("Vision").forward;
            enemyDir.y = 0;

            float dotProduct = Vector3.Dot(playerDir, enemyDir);

            if (dotProduct < -0.5f && dotProduct >= -1.0f)
                ChangeSprite(0);
            else if (dotProduct > 0.5f && dotProduct <= 1.0f)
                ChangeSprite(1);
            else
            {
                Vector3 playerRight = Camera.main.transform.right;
                playerRight.y = 0;
                dotProduct = Vector3.Dot(playerRight, enemyDir);
                if (dotProduct >= 0)
                    ChangeSprite(2);
                else
                    ChangeSprite(3);
            }
        }
    }

    void ChangeSprite(int index)
    {
        if (index < 0)
        {
            if (isAnimated)
                anim.Play(deathAnim);
            else
                sr.sprite = deathSprite;
        }
        if (index >= 0)
        {
            if (isAnimated)
                anim.Play(animStates[index]);
            else
                sr.sprite = sprites[index];
        }
    }
}
