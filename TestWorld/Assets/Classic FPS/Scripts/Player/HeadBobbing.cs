/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using UnityEngine;
using System.Collections;

public class HeadBobbing : MonoBehaviour
{
    TinyInput input;

    public float bobbingSpeed = 0.04f;
    public float bobbingHeight = 0.06f;
    public float midpoint = 0.75f;
    public bool isHeadBobbing = true;

    private float timer = 0.0f;

    private void Awake()
    {
        input = InputManager.Instance.input;
    }

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = input.Player.Move.ReadValue<Vector2>().x;
        float vertical = input.Player.Move.ReadValue<Vector2>().y;

        Vector3 cSharpConversion = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingHeight;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            if (isHeadBobbing == true)
                cSharpConversion.y = midpoint + translateChange;
            else if (isHeadBobbing == false)
                cSharpConversion.x = translateChange;
        }
        else
        {
            if (isHeadBobbing == true)
                cSharpConversion.y = midpoint;
            else if (isHeadBobbing == false)
                cSharpConversion.x = 0;
        }

        transform.localPosition = cSharpConversion;
    }



}