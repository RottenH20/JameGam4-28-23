/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.2.0a
    Updated in Version: 1.2.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 1f;
    public AnimationCurve curve;

    Vector3 _offset;

    private void Start()
    {
        _offset = transform.localPosition;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration) { 

            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = _offset + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = _offset;
    }
}
