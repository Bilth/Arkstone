using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

    public static Sun instance;
    //public static Color pinkish = new Color(219, 96, 254, 255);

    Color colorStart = Color.red;
    Color colorEnd = Color.cyan;
    float step = 0;
    float duration = 200;
    float timer = 0;
    int colorIndex = 0;
    Light _sunlight;


    Sunsetting _sunsettingCurrent;
    Sunsetting _sunsettingNext;
    List<Sunsetting> _sunsettings;

    // Use this for initialization
    void Start () {
        step = 0;

        _sunlight = GetComponent<Light>();
        _sunsettings = new List<Sunsetting>();

        // Sunlight, Shadow, Sky / Fog
        _sunsettings.Add(new Sunsetting(2f, "FFC07AFF", "720101", "FF676700")); // Warm Sunset
        _sunsettings.Add(new Sunsetting(2f, "14253BFF", "171A29", "1C2232", 0.00001f)); // Twilight
        _sunsettings.Add(new Sunsetting(2f, "FEFFB0FF", "2A96A6", "FEFFB0FF")); // Spring Morning
        _sunsettings.Add(new Sunsetting(2f, "2D76D9FF", "171A29", "1C51B8FF", 0.00001f)); // Twilight
        _sunsettings.Add(new Sunsetting(2f, "FF6D0DFF", "981294", "975CC800")); // Soft Heat
        _sunsettings.Add(new Sunsetting(2f, "FF6D0DFF", "981294", "975CC800")); // Soft Heat
        
        _sunsettings.Add(new Sunsetting(2f, "FEFFB0FF", "2A96A6", "FEFFB0FF")); // Spring Morning
        
        _sunsettings.Add(new Sunsetting(2f, "2D76D9FF", "171A29", "1C51B8FF", 0.00001f)); // Twilight
        
        
       
        colorIndex = 0;
       // setSetting(_sunsettings[colorIndex]);


        //Color tColor = Color.red;
        //tColor.

        //RenderSettings.ambientSkyColor = colorStart;

        //RenderSettings.fogColor = tColor;
        //RenderSettings.fogDensity = .02f;

        //colorEnd = new Color(219f / 255f, 96f / 255f, 254f / 255f);
    }

    void setSetting(Sunsetting pSetting)
    {
        _sunsettingCurrent = pSetting;

        // Sunlight
        _sunlight.color = _sunsettingCurrent.colorSun;
        
        // Shadow
        RenderSettings.ambientSkyColor = _sunsettingCurrent.colorShadow;

        // Sky / Fog
        Camera.main.backgroundColor = RenderSettings.fogColor = _sunsettingCurrent.colorSky;
        //RenderSettings.fogDensity = _sunsettingCurrent.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera.main.backgroundColor = RenderSettings.fogColor = _sunsettingCurrent.colorSky;
        //timer += Time.deltaTime;

        if(timer > 5)
        {
            Debug.Log("Time: " + Mathf.Round((transform.eulerAngles.x)));
            //transform.Rotate(new Vector3(0 * Time.deltaTime, 10 * Time.deltaTime, 0 * Time.deltaTime));

            //RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorStart, colorEnd, step));
            
            // Shadow Color
            RenderSettings.ambientSkyColor = Color.Lerp(_sunsettings[0].colorShadow, _sunsettings[1].colorShadow, step);

            // Sky Color
            RenderSettings.fogColor = Color.Lerp(_sunsettings[0].colorSky, _sunsettings[1].colorSky, step);
            //RenderSettings.fogDensity = Mathf.Lerp(_sunsettings[0].fogDensity, _sunsettings[1].fogDensity, step);
            //RenderSettings.skybox.SetColor("_Tint", Color.Lerp(_sunsettings[0].colorSky, _sunsettings[1].colorSky, step));
            Camera.main.backgroundColor = Color.Lerp(_sunsettings[0].colorSky, _sunsettings[1].colorSky, step);

            // Sunlight Color
            _sunlight.color = Color.Lerp(_sunsettings[0].colorSun, _sunsettings[1].colorSun, step);
            step += Time.deltaTime / duration;
        }

    }
}
