using System;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class WeatherManager : NetworkBehaviour
{
    [Header("Time Settings")]
    [OdinSerialize, Unit(Units.Second, Units.Minute)] public float DayLength { get; protected set; }
    [OdinSerialize, Unit(Units.Hour)] public float Sunrise { get; protected set; } = 6f;
    [OdinSerialize, Unit(Units.Hour)] public float Moonrise { get; protected set; } = 18f;
    [OdinSerialize, Unit(Units.Hour)] public float HoursInDay { get; protected set; } = 24f;
    [PropertySpace]
    [OdinSerialize] public DateTime StartDate { get; protected set; } = new DateTime(2050, 6, 1, 12, 0, 0);

    [Header("Light")]
    [OdinSerialize] public Light Directional { get; protected set; }
    [OdinSerialize] public float LightY { get; protected set; } = -150f;

    [Header("Weather Settings")]
    [OdinSerialize] public WeatherSettings[] Weathers { get; protected set; }

    [SyncVar(hook = nameof(OnDateChanged))]
    private double networkedTime; // текущее время в виде OADate (double)
    private const double secondsPerDay = 86400;

    public DateTime CurrentDateTime { get; protected set; }



    protected virtual void Awake()
    {
        CurrentDateTime = StartDate;
    }

    protected virtual void Update()
    {
        if (isServer) // сервер считает время
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
        RenderSettings.ambientLight = Color.Lerp(weather.AmbientColorNight, weather.AmbientColorDay, tNoon);
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
    [Header("Light"), MinMaxSlider(0f, 3f)]
    [OdinSerialize] public Vector2 LightIntensity { get; protected set; }
    [OdinSerialize] public Color LightColorDay { get; protected set; }
    [OdinSerialize] public Color LightColorNight { get; protected set; }
    [OdinSerialize] public float ShadowsStrength { get; protected set; } = 1f;
    [Header("Skybox"), MinMaxSlider(0f, 3f)]
    [OdinSerialize] public Vector2 SkyboxExposure { get; protected set; }
    [OdinSerialize] public Color AmbientColorDay { get; protected set; }
    [OdinSerialize] public Color AmbientColorNight { get; protected set; }
    [OdinSerialize] public Material Skybox { get; protected set; }
    [OdinSerialize] public bool ControlExposure { get; protected set; } = true;
    [OdinSerialize] public bool ControlRotation { get; protected set; } = true;

    public float LightIntensityDay => LightIntensity.y;
    public float LightIntensityNight => LightIntensity.x;

    public float SkyboxExposureDay => SkyboxExposure.y;
    public float SkyboxExposureNight => SkyboxExposure.x;
}