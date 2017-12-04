using UnityEngine;
using System.Collections;

public class DataSky : MonoBehaviour
{
    public float timeOfDay;
    public float fogDensity = 0.00002f;
    public Color colorSun;
    public Color colorShadow;
    public Color colorSky;

    public DataSky(float pTimeOfDay, string pColorSun, string pColorShadow, string pColorSky)
    {
        Init(pTimeOfDay, pColorSun, pColorShadow, pColorSky, 0.00002f);
    }

    public DataSky(float pTimeOfDay, string pColorSun, string pColorShadow, string pColorSky, float pFogDensity)
    {
        Init(pTimeOfDay, pColorSun, pColorShadow, pColorSky, pFogDensity);
    }

    private void Init(float pTimeOfDay, string pColorSun, string pColorShadow, string pColorSky, float pFogDensity)
    {
        timeOfDay = pTimeOfDay;
        colorSun = ToColor(pColorSun);
        colorShadow = ToColor(pColorShadow);
        colorSky = ToColor(pColorSky);
        fogDensity = pFogDensity;
    }

    private Color ToColor(string pHex)
    {
        var tHexString = "#" + pHex;
        Color tColor;
        ColorUtility.TryParseHtmlString(tHexString, out tColor);

        return tColor;
    }
}
