using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource source;
    [SerializeField] private TextMeshProUGUI ValueText;

    void Awake()
    {
        float volumeCheck;
        mixer.GetFloat("Volume", out volumeCheck);
        
        if(volumeCheck == 0.0f)
        {
            ValueText.SetText("100%");
            mixer.SetFloat("Volume", Mathf.Log10(1) * 20);
        }
        else
        {
            ValueText.SetText($"{(PlayerPrefs.GetFloat("Volume")*100).ToString("N0")}%");
            mixer.SetFloat("Volume", PlayerPrefs.GetFloat("Volume"));
        }
    }

    public void OnSlide(float value)
    {
        ValueText.SetText($"{(value*100).ToString("N0")}%");

        mixer.SetFloat("Volume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Volume", Mathf.Log10(value) * 20);
    }
}
