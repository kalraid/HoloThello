using UnityEngine;
using UnityEngine.UI;

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
    
    void Start()
    {
        LoadSettings();
        SetupUI();
    }
    
    void LoadSettings()
    {
        // 설정값 로드
        GameData.Instance.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        GameData.Instance.bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GameData.Instance.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        GameData.Instance.isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        GameData.Instance.screenWidth = PlayerPrefs.GetInt("ScreenWidth", 1920);
        GameData.Instance.screenHeight = PlayerPrefs.GetInt("ScreenHeight", 1080);
        
        // 캐릭터 타입 설정 로드
        int typeIndex = PlayerPrefs.GetInt("CharacterType", 0);
        GameData.Instance.selectedCharacterType = (CharacterType)typeIndex;
    }
    
    void SetupUI()
    {
        // 볼륨 슬라이더 설정
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = GameData.Instance.masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = GameData.Instance.bgmVolume;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = GameData.Instance.sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // 전체화면 토글 설정
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = GameData.Instance.isFullscreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
        
        // 해상도 드롭다운 설정
        if (resolutionDropdown != null)
        {
            SetupResolutionDropdown();
        }
        
        // 캐릭터 타입 버튼 설정
        if (typeAButton != null)
        {
            typeAButton.onClick.AddListener(OnTypeAButtonClicked);
        }
        
        if (typeBButton != null)
        {
            typeBButton.onClick.AddListener(OnTypeBButtonClicked);
        }
        
        // 돌아가기 버튼 설정
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        UpdateTypeUI();
    }
    
    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        
        // 일반적인 해상도들 추가
        string[] resolutions = {
            "1920x1080",
            "1600x900", 
            "1366x768",
            "1280x720",
            "1024x768"
        };
        
        resolutionDropdown.AddOptions(new System.Collections.Generic.List<string>(resolutions));
        
        // 현재 해상도 선택
        string currentRes = $"{GameData.Instance.screenWidth}x{GameData.Instance.screenHeight}";
        int currentIndex = System.Array.IndexOf(resolutions, currentRes);
        if (currentIndex >= 0)
        {
            resolutionDropdown.value = currentIndex;
        }
        
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
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
    
    // 볼륨 설정 이벤트
    public void OnMasterVolumeChanged(float value)
    {
        GameData.Instance.masterVolume = value;
        GameData.Instance.SaveSettings();
    }
    
    public void OnBGMVolumeChanged(float value)
    {
        GameData.Instance.bgmVolume = value;
        GameData.Instance.SaveSettings();
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        GameData.Instance.sfxVolume = value;
        GameData.Instance.SaveSettings();
    }
    
    // 화면 설정 이벤트
    public void OnFullscreenChanged(bool isFullscreen)
    {
        GameData.Instance.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        GameData.Instance.SaveSettings();
    }
    
    public void OnResolutionChanged(int index)
    {
        string[] resolutions = {
            "1920x1080",
            "1600x900", 
            "1366x768",
            "1280x720",
            "1024x768"
        };
        
        if (index >= 0 && index < resolutions.Length)
        {
            string[] parts = resolutions[index].Split('x');
            if (parts.Length == 2)
            {
                int width = int.Parse(parts[0]);
                int height = int.Parse(parts[1]);
                
                GameData.Instance.screenWidth = width;
                GameData.Instance.screenHeight = height;
                
                Screen.SetResolution(width, height, GameData.Instance.isFullscreen);
                GameData.Instance.SaveSettings();
            }
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
} 