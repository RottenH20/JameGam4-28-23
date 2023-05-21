/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.1.0a
    Updated in Version: 1.2.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSourceSetup : MonoBehaviour
{
    [Line("Property")]
    public Light lightSource;
    public SpriteRenderer spriteRenderer;

    [Line("Modes")]
    public LightMode lightMode;

    [Line("Colors")]
    public Color lightColor;
    [ColorUsage(false, true)] public Color spriteColor;

    [Line("Textures")]
    public Texture2D EmissionTexture;

    [Line("Values")]
    public float lightRange;
    [ShowIf("lightMode", 0)] public float lightIntensity;
    [ShowIf("lightMode", 1)] public float lightWaveMaxIntensity;
    [ShowIf("lightMode", 1)] public float lightWaveMinIntensity;
    [ShowIf("lightMode", 1)] public float lightWaveTime;


    bool _endWave = false;
    float _timer = 0;


    void Start()
    {
        Material mymat = spriteRenderer.material;
        mymat.SetColor("_EmissionColor", spriteColor);

        if (EmissionTexture != null) mymat.SetTexture("_EmissionMap", EmissionTexture);
        if (EmissionTexture != null) spriteRenderer.sprite = Sprite.Create(EmissionTexture, new Rect(0.0f, 0.0f, EmissionTexture.width, EmissionTexture.height), new Vector2(0.5f, 0.0f), 32);

        lightSource.color = lightColor;
        lightSource.intensity = lightIntensity;
        lightSource.range = lightRange;

        if (lightMode == LightMode.Wave) { lightSource.intensity = lightWaveMinIntensity; }
    }

    private void OnValidate()
    {
        lightSource.color = lightColor;
        lightSource.intensity = lightIntensity;
        lightSource.range = lightRange;
    }

    private void Update()
    {
        if(lightMode == LightMode.Wave)
        {
            /*float lightWaveValueIntensity = lightWaveMaxIntensity - lightWaveMinIntensity;

            if (_endWave) { lightSource.intensity += lightWaveValueIntensity * Time.deltaTime * (1 / lightWaveTime);  }
            else { lightSource.intensity -= lightWaveValueIntensity * Time.deltaTime * (1 / lightWaveTime); }

            if((_endWave && lightSource.intensity > lightWaveMaxIntensity) || (!_endWave && lightSource.intensity < lightWaveMinIntensity)) { _endWave = !_endWave; }*/

            float lightWaveValueIntensity = lightWaveMaxIntensity - lightWaveMinIntensity;

            _timer += Time.deltaTime;

            if(_timer > lightWaveTime) { _timer = 0; _endWave = !_endWave; }

            if (_endWave) { lightSource.intensity -= lightWaveValueIntensity * (Time.deltaTime / lightWaveTime); }
            else { lightSource.intensity += lightWaveValueIntensity * (Time.deltaTime / lightWaveTime); }
        }
    }
}

public enum LightMode
{
    Static = 0, Wave = 1
}
