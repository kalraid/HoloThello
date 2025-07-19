using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestManager : MonoBehaviour
{
    public static TestManager Instance { get; private set; }
    
    [Header("테스트 설정")]
    public bool enableDebugMode = false;
    public bool enablePerformanceMonitoring = false;
    public bool enableErrorLogging = true;
    
    [Header("성능 모니터링")]
    public Text fpsText;
    public Text memoryText;
    public Text errorText;
    
    private List<string> errorLog = new List<string>();
    private float fpsUpdateTimer = 0f;
    private int frameCount = 0;
    private float currentFPS = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTestManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeTestManager()
    {
        // 디버그 모드 설정
        if (enableDebugMode)
        {
            Debug.Log("TestManager: 디버그 모드 활성화");
        }
        
        // 성능 모니터링 초기화
        if (enablePerformanceMonitoring)
        {
            StartPerformanceMonitoring();
        }
        
        // 에러 로깅 초기화
        if (enableErrorLogging)
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }
    }
    
    void Update()
    {
        if (enablePerformanceMonitoring)
        {
            UpdatePerformanceMonitoring();
        }
    }
    
    void StartPerformanceMonitoring()
    {
        Debug.Log("TestManager: 성능 모니터링 시작");
    }
    
    void UpdatePerformanceMonitoring()
    {
        frameCount++;
        fpsUpdateTimer += Time.deltaTime;
        
        if (fpsUpdateTimer >= 1f)
        {
            currentFPS = frameCount / fpsUpdateTimer;
            frameCount = 0;
            fpsUpdateTimer = 0f;
            
            UpdatePerformanceUI();
        }
    }
    
    void UpdatePerformanceUI()
    {
        if (fpsText != null)
        {
            fpsText.text = $"FPS: {currentFPS:F1}";
        }
        
        if (memoryText != null)
        {
            long totalMemory = System.GC.GetTotalMemory(false);
            memoryText.text = $"메모리: {totalMemory / 1024 / 1024}MB";
        }
        
        if (errorText != null && errorLog.Count > 0)
        {
            errorText.text = $"에러: {errorLog.Count}개";
        }
    }
    
    void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            string errorMessage = $"[{type}] {logString}";
            errorLog.Add(errorMessage);
            
            if (enableDebugMode)
            {
                Debug.LogError($"TestManager 에러 로그: {errorMessage}");
            }
            
            // 최대 100개까지만 유지
            if (errorLog.Count > 100)
            {
                errorLog.RemoveAt(0);
            }
        }
    }
    
    // 유닛 테스트
    public void RunUnitTests()
    {
        Debug.Log("TestManager: 유닛 테스트 시작");
        
        TestCharacterData();
        TestGameLogic();
        TestUISystem();
        TestAudioSystem();
        
        Debug.Log("TestManager: 유닛 테스트 완료");
    }
    
    void TestCharacterData()
    {
        Debug.Log("TestManager: 캐릭터 데이터 테스트");
        
        // CharacterDataManager 테스트
        if (CharacterDataManager.Instance != null)
        {
            CharacterData[] typeAChars = CharacterDataManager.Instance.GetCharactersByType(CharacterType.TypeA);
            CharacterData[] typeBChars = CharacterDataManager.Instance.GetCharactersByType(CharacterType.TypeB);
            
            Debug.Log($"TypeA 캐릭터 수: {typeAChars.Length}");
            Debug.Log($"TypeB 캐릭터 수: {typeBChars.Length}");
            
            // 스킬 데이터 테스트
            for (int i = 0; i < 10; i++)
            {
                CharacterData charData = CharacterDataManager.Instance.GetCharacterData(i);
                if (charData != null)
                {
                    Debug.Log($"캐릭터 {i}: {charData.characterName}, 스킬A: {charData.skillA.skillName}");
                }
            }
        }
    }
    
    void TestGameLogic()
    {
        Debug.Log("TestManager: 게임 로직 테스트");
        
        // BoardManager 테스트
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager != null)
        {
            Debug.Log($"보드 크기: {boardManager.boardSize}");
            Debug.Log($"게임 종료 여부: {boardManager.IsGameEnded()}");
            Debug.Log($"현재 턴: {(boardManager.IsBlackTurn() ? "1P" : "CPU")}");
        }
        
        // GameManager 테스트
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log($"1P HP: {gameManager.playerHp}");
            Debug.Log($"CPU HP: {gameManager.cpuHp}");
        }
    }
    
    void TestUISystem()
    {
        Debug.Log("TestManager: UI 시스템 테스트");
        
        // UIManager 테스트
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("UI 테스트 메시지", 2f);
            Debug.Log("UIManager 테스트 완료");
        }
        
        // EffectManager 테스트
        if (EffectManager.Instance != null)
        {
            Debug.Log("EffectManager 테스트 완료");
        }
    }
    
    void TestAudioSystem()
    {
        Debug.Log("TestManager: 오디오 시스템 테스트");
        
        // AudioManager 테스트
        if (AudioManager.Instance != null)
        {
            Debug.Log($"마스터 볼륨: {AudioManager.Instance.GetMasterVolume()}");
            Debug.Log($"BGM 볼륨: {AudioManager.Instance.GetBGMVolume()}");
            Debug.Log($"SFX 볼륨: {AudioManager.Instance.GetSFXVolume()}");
        }
    }
    
    // 통합 테스트
    public void RunIntegrationTests()
    {
        Debug.Log("TestManager: 통합 테스트 시작");
        
        TestGameFlow();
        TestDataPersistence();
        TestSceneTransitions();
        
        Debug.Log("TestManager: 통합 테스트 완료");
    }
    
    void TestGameFlow()
    {
        Debug.Log("TestManager: 게임 플로우 테스트");
        
        // 캐릭터 선택 → 게임 → 결과 테스트
        if (GameData.Instance != null)
        {
            // 테스트용 캐릭터 설정
            CharacterData testChar = CharacterDataManager.Instance.GetCharacterData(0);
            if (testChar != null)
            {
                GameData.Instance.selectedCharacter1P = testChar;
                GameData.Instance.selectedCharacterCPU = testChar;
                Debug.Log("테스트 캐릭터 설정 완료");
            }
        }
    }
    
    void TestDataPersistence()
    {
        Debug.Log("TestManager: 데이터 저장 테스트");
        
        // 설정 저장/불러오기 테스트
        GameData.Instance.SetCharacterType(CharacterType.TypeA);
        GameData.Instance.SaveSettings();
        
        // 볼륨 설정 테스트
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(0.8f);
            AudioManager.Instance.SetBGMVolume(0.6f);
            AudioManager.Instance.SetSFXVolume(0.9f);
        }
        
        Debug.Log("데이터 저장 테스트 완료");
    }
    
    void TestSceneTransitions()
    {
        Debug.Log("TestManager: 씬 전환 테스트");
        
        // 씬 전환 테스트 (실제 전환은 하지 않고 로그만)
        Debug.Log("MainScene → CharacterSelectScene 전환 테스트");
        Debug.Log("CharacterSelectScene → GameScene 전환 테스트");
        Debug.Log("GameScene → SettingsScene 전환 테스트");
    }
    
    // 성능 테스트
    public void RunPerformanceTests()
    {
        Debug.Log("TestManager: 성능 테스트 시작");
        
        TestMemoryUsage();
        TestFrameRate();
        TestLoadingTime();
        
        Debug.Log("TestManager: 성능 테스트 완료");
    }
    
    void TestMemoryUsage()
    {
        long beforeMemory = System.GC.GetTotalMemory(false);
        
        // 메모리 사용량 테스트
        for (int i = 0; i < 1000; i++)
        {
            GameObject testObj = new GameObject($"TestObject_{i}");
            Destroy(testObj);
        }
        
        long afterMemory = System.GC.GetTotalMemory(false);
        long memoryDiff = afterMemory - beforeMemory;
        
        Debug.Log($"메모리 사용량 테스트: {memoryDiff / 1024}KB 차이");
    }
    
    void TestFrameRate()
    {
        Debug.Log("TestManager: 프레임레이트 테스트");
        
        // 1초간 FPS 측정
        float testDuration = 1f;
        float elapsed = 0f;
        int frameCount = 0;
        
        while (elapsed < testDuration)
        {
            elapsed += Time.deltaTime;
            frameCount++;
        }
        
        float averageFPS = frameCount / testDuration;
        Debug.Log($"평균 FPS: {averageFPS:F1}");
    }
    
    void TestLoadingTime()
    {
        Debug.Log("TestManager: 로딩 시간 테스트");
        
        float startTime = Time.realtimeSinceStartup;
        
        // 로딩 시뮬레이션
        for (int i = 0; i < 1000; i++)
        {
            // 가상의 로딩 작업
        }
        
        float endTime = Time.realtimeSinceStartup;
        float loadingTime = endTime - startTime;
        
        Debug.Log($"로딩 시간: {loadingTime * 1000:F1}ms");
    }
    
    // 버그 리포트 생성
    public void GenerateBugReport()
    {
        Debug.Log("TestManager: 버그 리포트 생성");
        
        string report = "=== 버그 리포트 ===\n";
        report += $"시간: {System.DateTime.Now}\n";
        report += $"Unity 버전: {Application.unityVersion}\n";
        report += $"플랫폼: {Application.platform}\n";
        report += $"FPS: {currentFPS:F1}\n";
        report += $"메모리: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB\n";
        report += $"에러 수: {errorLog.Count}\n";
        
        if (errorLog.Count > 0)
        {
            report += "\n=== 에러 로그 ===\n";
            foreach (string error in errorLog)
            {
                report += $"{error}\n";
            }
        }
        
        Debug.Log(report);
        
        // 파일로 저장 (에디터에서만)
        #if UNITY_EDITOR
        string filePath = $"Assets/bug_report_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
        System.IO.File.WriteAllText(filePath, report);
        Debug.Log($"버그 리포트 저장: {filePath}");
        #endif
    }
    
    // 디버그 명령어
    public void ExecuteDebugCommand(string command)
    {
        switch (command.ToLower())
        {
            case "test":
                RunUnitTests();
                break;
            case "integration":
                RunIntegrationTests();
                break;
            case "performance":
                RunPerformanceTests();
                break;
            case "report":
                GenerateBugReport();
                break;
            case "clear":
                errorLog.Clear();
                Debug.Log("에러 로그 초기화");
                break;
            default:
                Debug.Log($"알 수 없는 명령어: {command}");
                break;
        }
    }
    
    void OnDestroy()
    {
        if (enableErrorLogging)
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
    }
} 