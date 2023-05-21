/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    [Tooltip("Sprite List")]
    public List<Sprite> sprites;
    [Tooltip("Time between changes to the animation")]
    public float interval;
    [Tooltip("If true, the object will be destroyed after one animation")]
    public bool destroyAfter;

    // Privates
    int index = 0;
    float nextTimeChange = 0;
    float timer = 0;

    public void Awake()
    {
        nextTimeChange = interval;
        transform.GetComponent<SpriteRenderer>().sprite = sprites[index];
    }

    public void Update()
    {
        timer += Time.deltaTime;

        // If the timer exceeds a certain value, the picture changes
        if (timer >= nextTimeChange)
        {
            index++;
            if (sprites.Count == index)
            {
                // If destroy After is set to True, the object will be destroyed after the last frame
                if (destroyAfter)
                {
                    Destroy(this.gameObject);
                }
                // Otherwise, the animation will be looped
                else
                {
                    index = 0;
                }
            }
            nextTimeChange += interval;

            if (sprites.Count != index) transform.GetComponent<SpriteRenderer>().sprite = sprites[index];
        }
    }
}
