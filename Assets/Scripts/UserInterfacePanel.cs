using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfacePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab; // Prefab for the user interface panel
    [SerializeField] private Transform menuPosition;
    [SerializeField] private TMP_Text titleText; // Text component for the title of the panel
    [SerializeField] private TMP_Text solarPanelCountText; // Text component to display the number of solar panels
    [SerializeField] private TMP_Text energyOutputText;
    [SerializeField] private TMP_Text sliderText; // Text component for the slider value
    [SerializeField] private Slider energyOutputSlider;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        solarPanelCountText.text = $"You have <b>{SolarPanel.SolarPanels.Count}</b> <i>500W</i> Solar Panels.";
        energyOutputText.text = $"You are currently producing <b>{CalculateTotalEnergyOutput().ToString("F1")}W</b> out of a maximum of <b>{GetTotalMaxPowerOutput().ToString("F1")}W</b>.";
        energyOutputSlider.value = GetTotalMaxPowerOutput() != 0 ? CalculateTotalEnergyOutput()/GetTotalMaxPowerOutput() : 0f;
        sliderText.text = $"{(energyOutputSlider.value * 100).ToString("F1")}%";
    }

    private float GetTotalMaxPowerOutput()
    {
        float totalMaxPower = 0;
        foreach (var solarPanel in SolarPanel.SolarPanels)
        {
            totalMaxPower += solarPanel.MaxPowerOutput;
        }
        return totalMaxPower;
    }

    private float CalculateTotalEnergyOutput()
    {
        float totalEnergyOutput = 0f;
        foreach (var solarPanel in SolarPanel.SolarPanels)
        {
            totalEnergyOutput += solarPanel.EnergyOutput;
        }
        return totalEnergyOutput;
    }

    public void TogglePanel()
    {
        // Toggle the visibility of the user interface panel
        if (panelPrefab)
        {
            panelPrefab.SetActive(!panelPrefab.activeSelf);
            panelPrefab.transform.position = menuPosition.position;
            panelPrefab.transform.rotation = menuPosition.rotation;
        }
    }

    public void SetTitleText(string title)
    {
        if (titleText)
        {
            titleText.text = title;
        }
    }
}