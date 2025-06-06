using System;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SolarPanel : MonoBehaviour
{
    [Header("Solar Panel Settings")]
    [SerializeField] float efficiency = 0.20f; // 20% efficiency (typical for real panels)
    [SerializeField] float maxPowerOutput = 400f; // Watts (optimal power in full sunlight)
    [SerializeField] float area = 1.0f; // m² (size of the panel, scale your cube accordingly)
    [SerializeField] TextMeshPro textOutput; // TextMeshPro component to display output
    
    private Light sunLight;
    private float maxSunIntensity = 1000f; // W/m² (standard solar irradiance at optimal conditions)
    private void Start()
    {
        SunPositionCalculator sunPositionCalculator = FindFirstObjectByType<SunPositionCalculator>();
        if (!sunPositionCalculator) return;
        maxSunIntensity = sunPositionCalculator.MaxSunIntensity;
        sunLight = sunPositionCalculator.SunLight;
    }

    void Update()
    {
        if (!sunLight) return;
        float energyOutput = Mathf.Clamp(CalculateSolarEnergy(), 0, maxPowerOutput);
        if (textOutput)
        {
            textOutput.text = "Output: " + energyOutput.ToString("F2") + " Watts";
        }
    }

    float CalculateSolarEnergy()
    {
        // 0. Check if the sun is above the horizon. If below, return 0 energy output
        if (sunLight.transform.rotation.eulerAngles.x < 0 || sunLight.transform.rotation.eulerAngles.x > 180)
        {
            return 0;
        }
        
        // 1. Get angle between sun and panel normal
        float angle = Vector3.Angle(transform.forward, -sunLight.transform.forward);

        // 2. Calculate irradiance (how much light hits the panel)
        float irradiance = maxSunIntensity * Mathf.Cos(angle * Mathf.Deg2Rad);

        // 3. Clamp to avoid negative values (when sun is below horizon)
        irradiance = Mathf.Max(0, irradiance);

        // 4. Calculate power output (Watts)
        float powerOutput = irradiance * area * efficiency;

        return powerOutput;
    }
}
