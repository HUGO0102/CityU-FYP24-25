using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;
    [SerializeField] private Slider bgMusicSlider;
    [SerializeField] private Slider fovSlider;

    [Header("Text References")]
    [SerializeField] private Text mouseSensitivityText;
    [SerializeField] private Text masterVolumeText;
    [SerializeField] private Text effectVolumeText;
    [SerializeField] private Text bgMusicText;
    [SerializeField] private Text fovText;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    [Header("Audio Mixer Reference")]
    [SerializeField] private AudioMixer audioMixer;

    private float initialMouseSensitivity;
    private float initialMasterVolume;
    private float initialEffectVolume;
    private float initialBGMusicVolume;
    private float initialFov;

    private void Start()
    {
        LoadSettings();

        mouseSensitivitySlider.value = initialMouseSensitivity;
        masterVolumeSlider.value = initialMasterVolume;
        effectVolumeSlider.value = initialEffectVolume;
        bgMusicSlider.value = initialBGMusicVolume;
        fovSlider.value = initialFov;

        mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivityText);
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolumeText);
        effectVolumeSlider.onValueChanged.AddListener(UpdateEffectVolumeText);
        bgMusicSlider.onValueChanged.AddListener(UpdateBGMusicText);
        fovSlider.onValueChanged.AddListener(UpdateFovText);

        UpdateMouseSensitivityText(mouseSensitivitySlider.value);
        UpdateMasterVolumeText(masterVolumeSlider.value);
        UpdateEffectVolumeText(effectVolumeSlider.value);
        UpdateBGMusicText(bgMusicSlider.value);
        UpdateFovText(fovSlider.value);

        ApplySettings();
    }

    private void UpdateMouseSensitivityText(float value)
    {
        if (mouseSensitivityText != null)
        {
            mouseSensitivityText.text = value.ToString("F1");
        }
    }

    private void UpdateMasterVolumeText(float value)
    {
        if (masterVolumeText != null)
        {
            masterVolumeText.text = Mathf.RoundToInt(value).ToString();
        }
    }

    private void UpdateEffectVolumeText(float value)
    {
        if (effectVolumeText != null)
        {
            effectVolumeText.text = Mathf.RoundToInt(value).ToString();
        }
    }

    private void UpdateBGMusicText(float value)
    {
        if (bgMusicText != null)
        {
            bgMusicText.text = Mathf.RoundToInt(value).ToString();
        }
    }

    private void UpdateFovText(float value)
    {
        if (fovText != null)
        {
            fovText.text = Mathf.RoundToInt(value).ToString();
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", effectVolumeSlider.value);
        PlayerPrefs.SetFloat("BGMusicVolume", bgMusicSlider.value);
        PlayerPrefs.SetFloat("FOV", fovSlider.value);
        PlayerPrefs.Save();

        ApplySettings();
    }

    private void LoadSettings()
    {
        initialMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        initialMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 100f);
        initialEffectVolume = PlayerPrefs.GetFloat("EffectVolume", 100f);
        initialBGMusicVolume = PlayerPrefs.GetFloat("BGMusicVolume", 100f);

        //// �ˬd�O�_�O�Ĥ@�����J�]�Y PlayerPrefs �����s�b BGMusicVolume ��^
        //if (!PlayerPrefs.HasKey("BGMusicVolume"))
        //{
        //    // �p�G�O�Ĥ@�����J�A�j��]�� 100
        //    initialBGMusicVolume = 100f;
        //    PlayerPrefs.SetFloat("BGMusicVolume", initialBGMusicVolume);
        //    PlayerPrefs.Save();
        //}
        //else
        //{
        //    // �_�h���J�O�s����
        //    initialBGMusicVolume = PlayerPrefs.GetFloat("BGMusicVolume", 100f);
        //}

        initialFov = PlayerPrefs.GetFloat("FOV", 80f);
    }

    public void CancelSettings()
    {
        mouseSensitivitySlider.value = initialMouseSensitivity;
        masterVolumeSlider.value = initialMasterVolume;
        effectVolumeSlider.value = initialEffectVolume;
        bgMusicSlider.value = initialBGMusicVolume;
        fovSlider.value = initialFov;
    }

    private void ApplySettings()
    {
        // �վ㹫���F�ӫ�
        PlayerLook look = FindObjectOfType<PlayerLook>();
        if (look != null)
        {
            look.SetSensitivity(mouseSensitivitySlider.value);
        }
        else
        {
            Debug.LogError("PlayerLook not found in the scene! Please ensure a PlayerLook component exists.");
        }

        // �վ㭵�q
        if (audioMixer != null)
        {
            // Master Volume (0-100 �M�g�� -80 �� 0 dB)
            float masterVolume = Mathf.Lerp(-80f, 0f, masterVolumeSlider.value / 100f);
            if (!audioMixer.SetFloat("MasterVolume", masterVolume))
            {
                Debug.LogError("Failed to set MasterVolume on Audio Mixer.");
            }

            // Effect Volume (0-100 �M�g�� -80 �� 0 dB)
            float effectVolume = Mathf.Lerp(-80f, 0f, effectVolumeSlider.value / 100f);
            if (!audioMixer.SetFloat("SFXVolume", effectVolume))
            {
                Debug.LogError("Failed to set SFXVolume on Audio Mixer.");
            }

            // BGMusic Volume (0-100 �M�g�� -80 �� 0 dB)
            float bgMusicVolume = Mathf.Lerp(-80f, 0f, bgMusicSlider.value / 100f);
            if (!audioMixer.SetFloat("MusicVolume", bgMusicVolume))
            {
                Debug.LogError("Failed to set MusicVolume on Audio Mixer.");
            }

            Debug.Log($"Audio Mixer Volumes set - Master: {masterVolume}, Effect: {effectVolume}, BGMusic: {bgMusicVolume}");
        }
        else
        {
            Debug.LogError("Audio Mixer reference is not assigned in SettingsManager!");
        }

        // �վ� FOV
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = fovSlider.value;
            Debug.Log($"FOV set to: {fovSlider.value}");
        }
        else
        {
            Debug.LogError("Main Camera reference is not assigned in SettingsManager!");
        }
    }
}