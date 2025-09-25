using System;
using Mirror;
using Unity.Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class WeatherManager : NetworkBehaviour
{
    [Header("Time Settings")]
    public float DayLength;
    public float Sunrise = 6f;
    public float Moonrise = 18f;
    public float HoursInDay = 24f;
    [Space]
    public DateTime CurrentDateTime = new DateTime(2050, 6, 1, 12, 0, 0);

    [Header("Light")]
    public Light Directional;
    public float LightY = -150f;

    [Header("Weather Settings")]
    public WeatherSettings[] Weathers;

    [SyncVar(hook = nameof(OnDateChanged))]
    private double networkedTime; // текущее время в виде OADate (double)
    private const double secondsPerDay = 86400;



    protected virtual void Update()
    {
        if (Application.isPlaying && isServer) // сервер считает время
        {
            double gameSecondsPerRealSecond = secondsPerDay / DayLength;

            CurrentDateTime = CurrentDateTime.AddSeconds(Time.deltaTime * gameSecondsPerRealSecond);
            networkedTime = CurrentDateTime.ToOADate(); // double для Mirror
        }

        UpdateEnvironment();
    }

    protected virtual void OnDateChanged(double oldValue, double newValue)
    {
        CurrentDateTime = DateTime.FromOADate(newValue);
    }

    protected virtual void UpdateEnvironment()
    {
        var hour = CurrentDateTime.Hour + CurrentDateTime.Minute / 60f + CurrentDateTime.Second / 3600f;
        var noon = (Sunrise + Moonrise) / 2f;
        var weather = Weathers[0];
        var isDay = hour >= Sunrise && hour < Moonrise;
        var tHour = hour / HoursInDay;

        float lightX;
        float tRise;

        if (isDay)
        {
            tRise = Mathf.InverseLerp(Sunrise, Moonrise, hour);
        }
        else
        {
            float nightHour = (hour >= Moonrise) ? hour - Moonrise : hour + (HoursInDay - Moonrise);
            tRise = nightHour / ((HoursInDay - Moonrise) + Sunrise);
        }

        lightX = Mathf.Lerp(0f, 180f, tRise);
        Directional.transform.rotation = Quaternion.Euler(lightX, LightY, 0f);

        var tNoon = Mathf.Sin(Mathf.InverseLerp(Sunrise, Moonrise, hour) * Mathf.PI);
        var tMidnight = Mathf.Sin(Mathf.InverseLerp(Moonrise, Sunrise + Moonrise, hour > Moonrise ? hour : hour + HoursInDay) * Mathf.PI);

        Directional.shadowStrength = Mathf.Lerp(0f, weather.ShadowsStrength, isDay ? tNoon : tMidnight);
        Directional.intensity = Mathf.Lerp(weather.LightIntensityNight, weather.LightIntensityDay, tNoon);
        Directional.color = Color.Lerp(weather.LightColorNight, weather.LightColorDay, tNoon);

        // Render Settings
        RenderSettings.ambientSkyColor = Color.Lerp(weather.AmbientColorNight.SkyColor, weather.AmbientColorDay.SkyColor, tNoon);
        RenderSettings.ambientEquatorColor = Color.Lerp(weather.AmbientColorNight.EquatorColor, weather.AmbientColorDay.EquatorColor, tNoon);
        RenderSettings.ambientGroundColor = Color.Lerp(weather.AmbientColorNight.GroundColor, weather.AmbientColorDay.GroundColor, tNoon);
        RenderSettings.skybox = weather.Skybox;

        // Skybox
        if (weather.Skybox != null)
        {
            if (weather.ControlExposure)
            {
                float exposure = Mathf.Lerp(weather.SkyboxExposureNight, weather.SkyboxExposureDay, tNoon);
                weather.Skybox.SetFloat("_Exposure", exposure);
            }

            if (weather.ControlExposure)
            {
                float rotation = Mathf.Lerp(0f, 360f, tHour);
                weather.Skybox.SetFloat("_Rotation", rotation);
            }
        }
    }
}

[Serializable]
public class WeatherSettings
{
    [Header("Light"), MinMaxRangeSlider(0f, 3f)]
    public Vector2 LightIntensity;
    public Color LightColorDay;
    public Color LightColorNight;
    public float ShadowsStrength = 1f;
    [Header("Skybox"), MinMaxRangeSlider(0f, 3f)]
    public Vector2 SkyboxExposure;
    public EnvironmentGradient AmbientColorDay;
    public EnvironmentGradient AmbientColorNight;
    public Material Skybox;
    public bool ControlExposure = true;
    public bool ControlRotation = true;

    public float LightIntensityDay => LightIntensity.y;
    public float LightIntensityNight => LightIntensity.x;

    public float SkyboxExposureDay => SkyboxExposure.y;
    public float SkyboxExposureNight => SkyboxExposure.x;
}

[Serializable]
public struct EnvironmentGradient
{
    [ColorUsage(false, true)] public Color SkyColor;
    [ColorUsage(false, true)] public Color EquatorColor;
    [ColorUsage(false, true)] public Color GroundColor;
}