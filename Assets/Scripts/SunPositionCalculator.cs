using System;
using UnityEngine;

public class SunPositionCalculator : MonoBehaviour
{
    [SerializeField] float latitude = 37.7749f;    // Example: San Francisco
    [SerializeField] float longitude = -122.4194f;
    [SerializeField] float utcOffset = -7;         // UTC-7 for PDT
    [SerializeField] Light sunLight; // Reference to the directional light representing the sun
    [SerializeField] float updateInterval = 10f; // Update interval in seconds
    [SerializeField] float maxSunIntensity = 1000f; // W/m² (standard solar irradiance at optimal conditions)
    public Light SunLight => sunLight;
    public float MaxSunIntensity => maxSunIntensity;

    void Start()
    {
        InvokeRepeating(nameof(UpdateSunPosition), 0f, updateInterval); // Update every minute
    }

    public void UpdateSunPosition()
    {
        DateTime now = DateTime.Now;
        Debug.Log("Current Time: " + now.ToString("yyyy-MM-dd HH:mm:ss"));
        SunPosition sunPos = CalculateSunPosition(now, latitude, longitude, utcOffset);
        
        // Apply rotation to directional light
        transform.rotation = Quaternion.Euler(-sunPos.altitude, sunPos.azimuth, 0);
    }

    public static SunPosition CalculateSunPosition(DateTime dateTime, float lat, float lon, float utcOffset)
    {
        // Convert to UTC and calculate Julian Day
        DateTime utcTime = dateTime.AddHours(-utcOffset);
        double julianDate = GetJulianDate(utcTime);

        // Solar calculations (simplified)
        double solarMeanAnomaly = 357.5291 + 0.98560028 * julianDate;
        double equationOfCenter = 1.9148 * Math.Sin(solarMeanAnomaly * Mathf.Deg2Rad) 
                               + 0.02 * Math.Sin(2 * solarMeanAnomaly * Mathf.Deg2Rad);
        double eclipticLongitude = (solarMeanAnomaly + equationOfCenter + 180) % 360;

        // Convert to altitude/azimuth
        double declination = Math.Asin(Math.Sin(eclipticLongitude * Mathf.Deg2Rad) * Math.Sin(23.44 * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
        double hourAngle = (GetGMST(julianDate) + lon - eclipticLongitude) % 360;

        // Altitude & Azimuth
        double altitude = Math.Asin(
            Math.Sin(lat * Mathf.Deg2Rad) * Math.Sin(declination * Mathf.Deg2Rad) +
            Math.Cos(lat * Mathf.Deg2Rad) * Math.Cos(declination * Mathf.Deg2Rad) * Math.Cos(hourAngle * Mathf.Deg2Rad)
        ) * Mathf.Rad2Deg;

        double azimuth = Math.Atan2(
            Math.Sin(hourAngle * Mathf.Deg2Rad),
            Math.Cos(hourAngle * Mathf.Deg2Rad) * Math.Sin(lat * Mathf.Deg2Rad) - 
            Math.Tan(declination * Mathf.Deg2Rad) * Math.Cos(lat * Mathf.Deg2Rad)
        ) * Mathf.Rad2Deg + 180;

        return new SunPosition((float)altitude, (float)azimuth);
    }

    private static double GetJulianDate(DateTime date)
    {
        return date.ToOADate() + 2415018.5;
    }

    private static double GetGMST(double julianDate)
    {
        double t = (julianDate - 2451545.0) / 36525.0;
        return 280.46061837 + 360.98564736629 * (julianDate - 2451545.0) + 0.000387933 * t * t - t * t * t / 38710000.0;
    }
}

public struct SunPosition
{
    public float altitude; // Up/down angle (0 = horizon, 90 = zenith)
    public float azimuth;  // Compass direction (0 = North, 90 = East)

    public SunPosition(float alt, float azm)
    {
        altitude = alt;
        azimuth = azm;
    }
}