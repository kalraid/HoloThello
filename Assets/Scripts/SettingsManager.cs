using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq; // 해상도 검색을 위해 추가

public class SettingsManager : MonoBehaviour
{
    [Header("볼륨 설정")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("화면 설정")]
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    
    [Header("캐릭터 타입 설정")]
    public Button typeAButton;
    public Button typeBButton;
    public Text typeLabel;
    
    [Header("기타")]
    public Button backButton;
    
    // 해상도 목록 (중복 제거)
    private readonly string[] resolutions = {
        "1920x1080", "1600x900", "1366x768", "1280x720", "1024x768"
    };

    #region Unity Lifecycle Methods
    
    void Start()
    {
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }

        LoadAndApplySettings();
        InitializeUIValues();
        RegisterEventListeners();
    }

    #endregion

    #region Initialization

    bool ValidateComponents()
    {
        if (masterVolumeSlider == null || bgmVolumeSlider == null || sfxVolumeSlider == null ||
            fullscreenToggle == null || resolutionDropdown == null || typeAButton == null ||
            typeBButton == null || typeLabel == null || backButton == null)
        {
            Debug.LogError("SettingsManager의 일부 UI 컴포넌트가 연결되지 않았습니다!");
            return false;
        }
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance가 없습니다. 씬에 GameData 오브젝트를 추가하세요.");
            return false;
        }
        return true;
    }

    void LoadAndApplySettings()
    {
        // PlayerPrefs에서 설정값 로드
        GameData.Instance.LoadSettings();

        // 로드된 설정값을 실제 게임에 적용
        ApplyAudioSettings();
        ApplyScreenSettings();
    }

    void InitializeUIValues()
    {
        // UI 컨트롤에 로드된 설정값 반영
        masterVolumeSlider.value = GameData.Instance.masterVolume;
        bgmVolumeSlider.value = GameData.Instance.bgmVolume;
        sfxVolumeSlider.value = GameData.Instance.sfxVolume;
        fullscreenToggle.isOn = GameData.Instance.isFullscreen;
        
        SetupResolutionDropdown();
        UpdateTypeUI();
    }

    void RegisterEventListeners()
    {
        // 이벤트 리스너 등록 (중복 방지를 위해 기존 리스너 제거)
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        fullscreenToggle.onValueChanged.RemoveAllListeners();
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        typeAButton.onClick.RemoveAllListeners();
        typeAButton.onClick.AddListener(OnTypeAButtonClicked);

        typeBButton.onClick.RemoveAllListeners();
        typeBButton.onClick.AddListener(OnTypeBButtonClicked);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    
    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.ToList());
        
        string currentRes = $"{GameData.Instance.screenWidth}x{GameData.Instance.screenHeight}";
        int currentIndex = System.Array.IndexOf(resolutions, currentRes);
        if (currentIndex >= 0)
        {
            resolutionDropdown.value = currentIndex;
        }
        else
        {
            // 목록에 없는 해상도일 경우 첫 번째 항목 선택
            resolutionDropdown.value = 0;
        }
    }
    
    #endregion
    
    #region Settings Application

    void ApplyAudioSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(GameData.Instance.masterVolume);
            AudioManager.Instance.SetBGMVolume(GameData.Instance.bgmVolume);
            AudioManager.Instance.SetSFXVolume(GameData.Instance.sfxVolume);
        }
    }

    void ApplyScreenSettings()
    {
        Screen.SetResolution(GameData.Instance.screenWidth, GameData.Instance.screenHeight, GameData.Instance.isFullscreen);
    }

    #endregion

    #region UI Event Handlers
    
    public void OnMasterVolumeChanged(float value)
    {
        GameData.Instance.masterVolume = value;
        ApplyAudioSettings();
        GameData.Instance.SaveSettings();
    }
    
    public void OnBGMVolumeChanged(float value)
    {
        GameData.Instance.bgmVolume = value;
        ApplyAudioSettings();
        GameData.Instance.SaveSettings();
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        GameData.Instance.sfxVolume = value;
        ApplyAudioSettings();
        GameData.Instance.SaveSettings();
    }
    
    public void OnFullscreenChanged(bool isFullscreen)
    {
        GameData.Instance.isFullscreen = isFullscreen;
        ApplyScreenSettings();
        GameData.Instance.SaveSettings();
    }
    
    public void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutions.Length) return;
        
        string[] parts = resolutions[index].Split('x');
        if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
        {
            GameData.Instance.screenWidth = width;
            GameData.Instance.screenHeight = height;
            ApplyScreenSettings();
            GameData.Instance.SaveSettings();
        }
    }

    // 캐릭터 타입 설정 이벤트
    public void OnTypeAButtonClicked()
    {
        GameData.Instance.SetCharacterType(CharacterType.TypeA);
        UpdateTypeUI();
    }
    
    public void OnTypeBButtonClicked()
    {
        GameData.Instance.SetCharacterType(CharacterType.TypeB);
        UpdateTypeUI();
    }
    
    // 돌아가기 버튼
    public void OnBackButtonClicked()
    {
        GameData.Instance.SaveSettings();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    void UpdateTypeUI()
    {
        if (typeLabel != null)
        {
            typeLabel.text = GameData.Instance.selectedCharacterType == CharacterType.TypeA ? "Hololive 타입" : "고양이 타입";
        }
        
        if (typeAButton != null)
        {
            typeAButton.interactable = GameData.Instance.selectedCharacterType != CharacterType.TypeA;
        }
        
        if (typeBButton != null)
        {
            typeBButton.interactable = GameData.Instance.selectedCharacterType != CharacterType.TypeB;
        }
    }

    #endregion
} 