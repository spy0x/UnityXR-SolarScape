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
    [SerializeField] private GroqChat groqChat; // Reference to the GroqChat component
    [SerializeField] private Toggle reviewButton;
    [SerializeField] private TMP_Text groqMessageText; // Text component to display Groq's response
    [SerializeField] private GroqTTS groqTTS; // Reference to the GroqTTS component for text-to-speech

    private void Start()
    {
        groqChat.OnMessageGenerated += OnGroqMessageGenerated;
    }

    private void OnGroqMessageGenerated(bool wasSuccessful, string message)
    {
        reviewButton.interactable = true;
        groqMessageText.text = message;
        if (groqTTS && wasSuccessful)
        {
            // get only the first paragraph from the message because its too long for TTS
            int firstParagraphEnd = message.IndexOf('\n');
            if (firstParagraphEnd > 0)
            {
                message = message.Substring(0, firstParagraphEnd);
            }

            groqTTS.Prompt = message;
            groqTTS.Generate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!panelPrefab.activeSelf) return;

        solarPanelCountText.text = $"You have <b>{SolarPanel.SolarPanels.Count}</b> <i>500W</i> Solar Panels.";
        energyOutputText.text =
            $"You are currently producing <b>{CalculateTotalEnergyOutput().ToString("F1")}W</b> out of a maximum of <b>{GetTotalMaxPowerOutput().ToString("F1")}W</b>.";
        
        energyOutputSlider.value =
            GetTotalMaxPowerOutput() != 0 ? CalculateTotalEnergyOutput() / GetTotalMaxPowerOutput() : 0f;
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
            groqMessageText.text = string.Empty; // Clear the Groq message text when toggling the panel
        }
    }

    public void SetTitleText(string title)
    {
        if (titleText)
        {
            titleText.text = title;
        }
    }

    public void ReviewWithGroq()
    {
        if (groqChat)
        {
            groqMessageText.text = "Generating review...";
            reviewButton.interactable = false;
            groqChat.UserInput =
                $"I have {SolarPanel.SolarPanels.Count} 500w solar panels, currently getting a total of {CalculateTotalEnergyOutput().ToString("F1")}W. For a residential house, what devices could I have connected without using power grid? Make a bullet list in plain text for Unity Text Mesh Pro GUI.";
            groqChat.SendRequest();
        }
    }
}