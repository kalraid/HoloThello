using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("안내 메시지")]
    public GameObject messagePanel;
    public Text messageText;
    public Button messageCloseButton;
    
    [Header("로딩 화면")]
    public GameObject loadingPanel;
    public Slider loadingProgressBar;
    public Text loadingText;
    
    [Header("게임 정보")]
    public Text gameInfoText;
    public Text turnInfoText;
    public Text scoreInfoText;
    
    [Header("접근성")]
    public Toggle colorBlindModeToggle;
    public Slider fontSizeSlider;
    public Text fontSizeLabel;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeUI()
    {
        // 메시지 패널 초기화
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
        
        if (messageCloseButton != null)
        {
            messageCloseButton.onClick.AddListener(HideMessage);
        }
        
        // 로딩 패널 초기화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        // 접근성 설정 로드
        LoadAccessibilitySettings();
    }
    
    // 안내 메시지 표시
    public void ShowMessage(string message, float duration = 3f)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            
            if (duration > 0)
            {
                StartCoroutine(AutoHideMessage(duration));
            }
        }
    }
    
    public void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
    
    IEnumerator AutoHideMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }
    
    // 로딩 화면 표시
    public void ShowLoading(string message = "로딩 중...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            
            if (loadingText != null)
            {
                loadingText.text = message;
            }
            
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 0f;
            }
        }
    }
    
    public void UpdateLoadingProgress(float progress, string message = null)
    {
        if (loadingProgressBar != null)
        {
            loadingProgressBar.value = progress;
        }
        
        if (loadingText != null && !string.IsNullOrEmpty(message))
        {
            loadingText.text = message;
        }
    }
    
    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
    
    // 게임 정보 업데이트
    public void UpdateGameInfo(string info)
    {
        if (gameInfoText != null)
        {
            gameInfoText.text = info;
        }
    }
    
    public void UpdateTurnInfo(string turnInfo)
    {
        if (turnInfoText != null)
        {
            turnInfoText.text = turnInfo;
        }
    }
    
    public void UpdateScoreInfo(string scoreInfo)
    {
        if (scoreInfoText != null)
        {
            scoreInfoText.text = scoreInfo;
        }
    }
    
    // 접근성 설정
    void LoadAccessibilitySettings()
    {
        bool colorBlindMode = PlayerPrefs.GetInt("ColorBlindMode", 0) == 1;
        float fontSize = PlayerPrefs.GetFloat("FontSize", 1.0f);
        
        if (colorBlindModeToggle != null)
        {
            colorBlindModeToggle.isOn = colorBlindMode;
            colorBlindModeToggle.onValueChanged.AddListener(OnColorBlindModeChanged);
        }
        
        if (fontSizeSlider != null)
        {
            fontSizeSlider.value = fontSize;
            fontSizeSlider.onValueChanged.AddListener(OnFontSizeChanged);
        }
        
        ApplyAccessibilitySettings(colorBlindMode, fontSize);
    }
    
    void ApplyAccessibilitySettings(bool colorBlindMode, float fontSize)
    {
        // 색약 모드 적용
        if (colorBlindMode)
        {
            ApplyColorBlindMode();
        }
        
        // 폰트 크기 적용
        ApplyFontSize(fontSize);
    }
    
    void ApplyColorBlindMode()
    {
        // 색약 모드를 위한 색상 변경
        // 빨강/초록 구분이 어려운 색상들을 파랑/노랑으로 변경
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            if (text.color == Color.red)
            {
                text.color = Color.blue;
            }
            else if (text.color == Color.green)
            {
                text.color = Color.yellow;
            }
        }
        
        // 체력바 색상 변경
        Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        foreach (Slider slider in allSliders)
        {
            Image fillImage = slider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (fillImage.color == Color.red)
                {
                    fillImage.color = Color.blue;
                }
                else if (fillImage.color == Color.green)
                {
                    fillImage.color = Color.yellow;
                }
            }
        }
    }
    
    void ApplyFontSize(float sizeMultiplier)
    {
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            text.fontSize = Mathf.RoundToInt(text.fontSize * sizeMultiplier);
        }
        
        if (fontSizeLabel != null)
        {
            fontSizeLabel.text = $"폰트 크기: {sizeMultiplier:F1}x";
        }
    }
    
    public void OnColorBlindModeChanged(bool isOn)
    {
        PlayerPrefs.SetInt("ColorBlindMode", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        ApplyAccessibilitySettings(isOn, fontSizeSlider != null ? fontSizeSlider.value : 1.0f);
    }
    
    public void OnFontSizeChanged(float size)
    {
        PlayerPrefs.SetFloat("FontSize", size);
        PlayerPrefs.Save();
        
        ApplyFontSize(size);
    }
    
    // 툴팁 시스템
    public void ShowTooltip(string tooltipText, Vector3 position)
    {
        // 간단한 툴팁 구현
        ShowMessage(tooltipText, 2f);
    }
    
    // 게임 튜토리얼
    public void ShowTutorial()
    {
        string tutorialText = "게임 방법:\n" +
            "1. 오셀로 보드에서 돌을 놓아 상대방 HP를 깎으세요\n" +
            "2. 스킬 버튼을 사용해 추가 데미지를 주세요\n" +
            "3. 상대방 HP를 0으로 만들면 승리!\n" +
            "4. 5배수 데미지 시 특별 이펙트가 발동됩니다";
        
        ShowMessage(tutorialText, 5f);
    }
    
    // 게임 통계 표시
    public void ShowGameStats(int totalMoves, int totalDamage, float gameTime)
    {
        string statsText = $"게임 통계:\n" +
            $"총 수: {totalMoves}\n" +
            $"총 데미지: {totalDamage}\n" +
            $"게임 시간: {gameTime:F1}초";
        
        ShowMessage(statsText, 4f);
    }
    
    // 키보드 단축키 안내
    public void ShowKeyboardShortcuts()
    {
        string shortcutsText = "키보드 단축키:\n" +
            "1, 2, 3: 스킬 사용\n" +
            "ESC: 메뉴\n" +
            "R: 게임 재시작\n" +
            "T: 튜토리얼";
        
        ShowMessage(shortcutsText, 3f);
    }
    
    // 에러 메시지
    public void ShowError(string errorMessage)
    {
        if (messageText != null)
        {
            messageText.color = Color.red;
        }
        
        ShowMessage($"오류: {errorMessage}", 3f);
        
        // 색상 복원
        StartCoroutine(RestoreMessageColor());
    }
    
    IEnumerator RestoreMessageColor()
    {
        yield return new WaitForSeconds(3f);
        if (messageText != null)
        {
            messageText.color = Color.white;
        }
    }
    
    // 성공 메시지
    public void ShowSuccess(string successMessage)
    {
        if (messageText != null)
        {
            messageText.color = Color.green;
        }
        
        ShowMessage(successMessage, 2f);
        
        // 색상 복원
        StartCoroutine(RestoreMessageColor());
    }
    
    // 경고 메시지
    public void ShowWarning(string warningMessage)
    {
        if (messageText != null)
        {
            messageText.color = Color.yellow;
        }
        
        ShowMessage($"경고: {warningMessage}", 2f);
        
        // 색상 복원
        StartCoroutine(RestoreMessageColor());
    }
} 