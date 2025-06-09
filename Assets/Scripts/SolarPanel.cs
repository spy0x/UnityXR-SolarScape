using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class SolarPanel : MonoBehaviour
{
    [Header("Solar Panel Settings")] [SerializeField]
    float efficiency = 0.20f; // 20% efficiency (typical for real panels)

    [SerializeField] float maxPowerOutput = 400f; // Watts (optimal power in full sunlight)
    [SerializeField] float area = 1.0f; // m² (size of the panel, scale your cube accordingly)
    [SerializeField] TMP_Text textOutput; // TextMeshPro component to display output
	[SerializeField] GameObject panelPrefab;
    [SerializeField] InteractableObjectLabel interactableObjectLabel;

    private Light sunLight;
    private float maxSunIntensity = 1000f; // W/m² (standard solar irradiance at optimal conditions)

    private void Awake()
    {
        interactableObjectLabel.playerHead = GameObject.FindWithTag("MainCamera").transform;
    }

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
            textOutput.text = $"{energyOutput.ToString("F2")} W";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            Destroy(gameObject);
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

    public void SetZRotation()
    {
        StopAllCoroutines();
        StartCoroutine(SetZRotationCoroutine());
    }

    private IEnumerator SetZRotationCoroutine()
    {
        yield return new WaitForSeconds(1f);
        if (transform.rotation.eulerAngles.z > -45 && transform.rotation.eulerAngles.z < 45)
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        else if (transform.rotation.eulerAngles.z > 315 && transform.rotation.eulerAngles.z < 360)
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        else if (transform.rotation.eulerAngles.z < -135 && transform.rotation.eulerAngles.z > -225)
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        else if (transform.rotation.eulerAngles.z > 135 && transform.rotation.eulerAngles.z < 225)
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 90);
        }
    }

    public void DuplicateSolarPanel(Transform spawnPoint)
    {
        Instantiate(panelPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}