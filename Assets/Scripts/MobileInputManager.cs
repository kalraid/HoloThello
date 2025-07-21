using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteInEditMode] // 제거
public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }
    
    [Header("터치 설정")]
    public float touchSensitivity = 1.0f;
    public float doubleTapTime = 0.3f;
    public float longPressTime = 1.0f;
    
    [Header("UI 요소")]
    public Canvas mobileCanvas;
    public GameObject mobileControls;
    public Button pauseButton;
    public Button settingsButton;
    public Button helpButton;
    
    private float lastTapTime = 0f;
    private Vector2 lastTapPosition;
    private bool isLongPress = false;
    private float longPressTimer = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMobileInput();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeMobileInput()
    {
        // 모바일 플랫폼에서만 모바일 컨트롤 활성화
        #if UNITY_ANDROID || UNITY_IOS
        if (mobileControls != null)
        {
            mobileControls.SetActive(true);
        }
        #else
        if (mobileControls != null)
        {
            mobileControls.SetActive(false);
        }
        #endif
        
        // 버튼 이벤트 연결
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }
        
        if (helpButton != null)
        {
            helpButton.onClick.AddListener(OnHelpButtonClicked);
        }
    }
    
    void Update()
    {
        #if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
        #endif
    }
    
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch);
                    break;
                    
                case TouchPhase.Moved:
                    OnTouchMoved(touch);
                    break;
                    
                case TouchPhase.Ended:
                    OnTouchEnded(touch);
                    break;
            }
        }
    }
    
    void OnTouchBegan(Touch touch)
    {
        lastTapPosition = touch.position;
        longPressTimer = 0f;
        isLongPress = false;
    }
    
    void OnTouchMoved(Touch touch)
    {
        // 드래그 처리
        Vector2 delta = touch.position - lastTapPosition;
        
        // 오셀로 보드 드래그 처리
        HandleBoardDrag(delta);
        
        lastTapPosition = touch.position;
        
        // 롱프레스 체크
        longPressTimer += Time.deltaTime;
        if (longPressTimer >= longPressTime && !isLongPress)
        {
            isLongPress = true;
            OnLongPress(touch.position);
        }
    }
    
    void OnTouchEnded(Touch touch)
    {
        float timeSinceLastTap = Time.time - lastTapTime;
        
        if (timeSinceLastTap < doubleTapTime)
        {
            // 더블탭 처리
            OnDoubleTap(touch.position);
        }
        else
        {
            // 싱글탭 처리
            OnSingleTap(touch.position);
        }
        
        lastTapTime = Time.time;
        isLongPress = false;
    }
    
    void OnSingleTap(Vector2 position)
    {
        // 오셀로 보드 클릭 처리
        HandleBoardClick(position);
        
        // UI 버튼 클릭 처리
        HandleUIButtonClick(position);
    }
    
    void OnDoubleTap(Vector2 position)
    {
        Debug.Log($"더블탭: {position}");
        
        // 더블탭으로 스킬 사용
        HandleDoubleTapSkill(position);
    }
    
    void OnLongPress(Vector2 position)
    {
        Debug.Log($"롱프레스: {position}");
        
        // 롱프레스로 메뉴 열기
        ShowContextMenu(position);
    }
    
    void HandleBoardClick(Vector2 screenPosition)
    {
        // 스크린 좌표를 월드 좌표로 변환
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        
        // 오셀로 보드 클릭 처리
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager != null)
        {
            // 보드 좌표 계산
            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.y);
            
            // 유효한 보드 범위인지 확인
            if (x >= 0 && x < boardManager.boardSize && y >= 0 && y < boardManager.boardSize)
            {
                // 돌 배치 시도
                boardManager.TryPlacePiece(x, y);
            }
        }
    }
    
    void HandleBoardDrag(Vector2 delta)
    {
        // 보드 드래그 처리 (카메라 이동 등)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // 카메라 이동 제한
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x - delta.x * 0.01f, -5f, 5f);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y - delta.y * 0.01f, -5f, 5f);
            mainCamera.transform.position = cameraPosition;
        }
    }
    
    void HandleUIButtonClick(Vector2 position)
    {
        // UI 레이캐스트로 버튼 클릭 확인
        GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();
        if (raycaster != null && EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = position;
            
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            
            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    break;
                }
            }
        }
    }
    
    void HandleDoubleTapSkill(Vector2 position)
    {
        // 더블탭으로 스킬 사용
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            // 화면 위치에 따라 스킬 선택
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            if (position.x < screenWidth / 3)
            {
                // 좌측: 스킬A
                gameManager.UseSkill(0);
            }
            else if (position.x < screenWidth * 2 / 3)
            {
                // 중앙: 스킬B
                gameManager.UseSkill(1);
            }
            else
            {
                // 우측: 궁극기
                gameManager.UseSkill(2);
            }
        }
    }
    
    void ShowContextMenu(Vector2 position)
    {
        // 컨텍스트 메뉴 표시
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("롱프레스 메뉴", 2f);
        }
    }
    
    // 모바일 전용 버튼 이벤트
    public void OnPauseButtonClicked()
    {
        Debug.Log("일시정지 버튼 클릭");
        
        // 게임 일시정지
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        
        if (UIManager.Instance != null)
        {
            string message = Time.timeScale == 0 ? "게임 일시정지" : "게임 재개";
            UIManager.Instance.ShowMessage(message, 1f);
        }
    }
    
    public void OnSettingsButtonClicked()
    {
        Debug.Log("설정 버튼 클릭");
        
        // 설정 화면으로 이동
        UnityEngine.SceneManagement.SceneManager.LoadScene("SettingsScene");
    }
    
    public void OnHelpButtonClicked()
    {
        Debug.Log("도움말 버튼 클릭");
        
        // 도움말 표시
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTutorial();
        }
    }
    
    // 모바일 전용 제스처 처리
    public void OnSwipeUp()
    {
        Debug.Log("위로 스와이프");
        // 위로 스와이프 처리
    }
    
    public void OnSwipeDown()
    {
        Debug.Log("아래로 스와이프");
        // 아래로 스와이프 처리
    }
    
    public void OnSwipeLeft()
    {
        Debug.Log("왼쪽으로 스와이프");
        // 왼쪽으로 스와이프 처리
    }
    
    public void OnSwipeRight()
    {
        Debug.Log("오른쪽으로 스와이프");
        // 오른쪽으로 스와이프 처리
    }
    
    // 모바일 최적화 설정
    public void ApplyMobileOptimizations()
    {
        // 프레임레이트 제한
        Application.targetFrameRate = 60;
        
        // 배터리 절약 모드
        #if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        #endif
        
        // 메모리 최적화
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;
        
        Debug.Log("모바일 최적화 설정 적용 완료");
    }
} 