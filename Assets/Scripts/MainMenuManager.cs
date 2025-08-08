using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 메뉴의 버튼들을 관리하는 매니저
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI 버튼")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("UI 텍스트")]
    public Text startText;
    public Text settingsText;
    public Text quitText;
    
    [Header("게임 제목")]
    public Text gameTitleText;
    public Text subTitleText;
    public Text versionText;
    
    [Header("오디오")]
    public AudioSource buttonClickSource;
    public AudioClip buttonClickSound;
    
    [Header("UI 이미지")]
    public Image backgroundImage;
    public Image characterImage;
    
    void Start()
    {
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }
        
        InitializeUI();
        SetupButtonListeners();
        LoadGameData();
    }
    
    bool ValidateComponents()
    {
        bool isValid = true;
        
        if (startButton == null)
        {
            Debug.LogError("MainMenuManager: startButton이 연결되지 않았습니다.");
            isValid = false;
        }
        
        if (settingsButton == null)
        {
            Debug.LogError("MainMenuManager: settingsButton이 연결되지 않았습니다.");
            isValid = false;
        }
        
        if (quitButton == null)
        {
            Debug.LogError("MainMenuManager: quitButton이 연결되지 않았습니다.");
            isValid = false;
        }
        
        return isValid;
    }
    
    void InitializeUI()
    {
        // 게임 제목 설정
        if (gameTitleText != null)
            gameTitleText.text = "OSELLO";
        
        if (subTitleText != null)
            subTitleText.text = "전략 보드 게임";
        
        if (versionText != null)
            versionText.text = "v1.0.0";
        
        // 버튼 텍스트 설정
        if (startText != null)
            startText.text = "게임 시작";
        
        if (settingsText != null)
            settingsText.text = "설정";
        
        if (quitText != null)
            quitText.text = "종료";
    }
    
    void SetupButtonListeners()
    {
        // 기존 리스너 제거
        startButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        
        // 새로운 리스너 추가
        startButton.onClick.AddListener(OnStartButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    
    void LoadGameData()
    {
        // GameData가 없으면 생성
        if (GameData.Instance == null)
        {
            GameObject gameDataGO = new GameObject("GameData");
            gameDataGO.AddComponent<GameData>();
        }
        
        // AudioManager가 없으면 생성
        if (AudioManager.Instance == null)
        {
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManagerGO.AddComponent<AudioManager>();
        }
    }
    
    /// <summary>
    /// 게임 시작 버튼 클릭
    /// </summary>
    public void OnStartButtonClicked()
    {
        PlayButtonClickSound();
        Debug.Log("게임 시작 버튼 클릭됨");
        
        // CharacterSelectScene으로 이동
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadCharacterSelectScene();
        }
        else
        {
            // SceneController가 없으면 직접 로드
            SceneManager.LoadScene("CharacterSelectScene");
        }
    }
    
    /// <summary>
    /// 설정 버튼 클릭
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        PlayButtonClickSound();
        Debug.Log("설정 버튼 클릭됨");
        
        // SettingsScene으로 이동
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadSettingsScene();
        }
        else
        {
            // SceneController가 없으면 직접 로드
            SceneManager.LoadScene("SettingsScene");
        }
    }
    
    /// <summary>
    /// 종료 버튼 클릭
    /// </summary>
    public void OnQuitButtonClicked()
    {
        PlayButtonClickSound();
        Debug.Log("종료 버튼 클릭됨");
        
        // 게임 종료
        if (SceneController.Instance != null)
        {
            SceneController.Instance.QuitGame();
        }
        else
        {
            // SceneController가 없으면 직접 종료
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    /// <summary>
    /// 버튼 클릭 사운드 재생
    /// </summary>
    void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        else if (buttonClickSource != null && buttonClickSound != null)
        {
            buttonClickSource.PlayOneShot(buttonClickSound);
        }
    }
    
    /// <summary>
    /// 키보드 입력 처리
    /// </summary>
    void Update()
    {
        HandleKeyboardInput();
    }
    
    void HandleKeyboardInput()
    {
        // Enter 키: 게임 시작
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnStartButtonClicked();
        }
        
        // S 키: 설정
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSettingsButtonClicked();
        }
        
        // Escape 키: 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitButtonClicked();
        }
    }
} 