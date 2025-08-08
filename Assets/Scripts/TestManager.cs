using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    [Header("테스트 UI")]
    public Text testResultText;
    public Button runAllTestsButton;
    public Button clearTestButton;
    
    [Header("기능별 테스트 버튼")]
    public Button testDiscButton;
    public Button testHPButton;
    public Button testSkillButton;
    public Button testDamageButton;
    public Button testGameEndButton;
    public Button testHPBarButton;
    public Button testPrefabButton;
    public Button testPerformanceButton;
    public Button testCharacterButton;
    public Button testBoardButton;
    public Button testAudioButton;
    public Button testMobileButton;
    
    private string testResults = "";
    
    void Start()
    {
        // 전체 테스트 버튼
        if (runAllTestsButton != null)
            runAllTestsButton.onClick.AddListener(RunAllTests);
        
        if (clearTestButton != null)
            clearTestButton.onClick.AddListener(ClearTestResults);
        
        // 기능별 테스트 버튼
        if (testDiscButton != null)
            testDiscButton.onClick.AddListener(() => RunSingleTest("Disc"));
        if (testHPButton != null)
            testHPButton.onClick.AddListener(() => RunSingleTest("HP"));
        if (testSkillButton != null)
            testSkillButton.onClick.AddListener(() => RunSingleTest("Skill"));
        if (testDamageButton != null)
            testDamageButton.onClick.AddListener(() => RunSingleTest("Damage"));
        if (testGameEndButton != null)
            testGameEndButton.onClick.AddListener(() => RunSingleTest("GameEnd"));
        if (testHPBarButton != null)
            testHPBarButton.onClick.AddListener(() => RunSingleTest("HPBar"));
        if (testPrefabButton != null)
            testPrefabButton.onClick.AddListener(() => RunSingleTest("Prefab"));
        if (testPerformanceButton != null)
            testPerformanceButton.onClick.AddListener(() => RunSingleTest("Performance"));
        if (testCharacterButton != null)
            testCharacterButton.onClick.AddListener(() => RunSingleTest("Character"));
        if (testBoardButton != null)
            testBoardButton.onClick.AddListener(() => RunSingleTest("Board"));
        if (testAudioButton != null)
            testAudioButton.onClick.AddListener(() => RunSingleTest("Audio"));
        if (testMobileButton != null)
            testMobileButton.onClick.AddListener(() => RunSingleTest("Mobile"));
    }
    
    public void RunSingleTest(string testType)
    {
        testResults = "";
        AddTestResult($"=== {testType} 기능 테스트 시작 ===\n");
        
        switch (testType)
        {
            case "Disc":
                TestDiscVisibility();
                break;
            case "HP":
                TestHPDisplay();
                break;
            case "Skill":
                TestSkillUI();
                break;
            case "Damage":
                TestDamageEffects();
                break;
            case "GameEnd":
                TestGameEndResult();
                break;
            case "HPBar":
                TestHPBarSegments();
                break;
            case "Prefab":
                TestPrefabSystem();
                break;
            case "Performance":
                TestPerformance();
                break;
            case "Character":
                TestCharacterSystem();
                break;
            case "Board":
                TestBoardSystem();
                break;
            case "Audio":
                TestAudioSystem();
                break;
            case "Mobile":
                TestMobileSystem();
                break;
        }
        
        AddTestResult($"\n=== {testType} 테스트 완료 ===");
        
        if (testResultText != null)
            testResultText.text = testResults;
        
        Debug.Log(testResults);
    }
    
    public void RunAllTests()
    {
        testResults = "";
        AddTestResult("=== HoloThello 프로젝트 전체 테스트 시작 ===\n");
        
        // 1순위 테스트
        TestDiscVisibility();
        TestHPDisplay();
        TestSkillUI();
        
        // 2순위 테스트
        TestDamageEffects();
        TestGameEndResult();
        TestHPBarSegments();
        
        // 3순위 테스트
        TestPrefabSystem();
        TestPerformance();
        
        // 추가 기능 테스트
        TestCharacterSystem();
        TestBoardSystem();
        TestAudioSystem();
        TestMobileSystem();
        
        AddTestResult("\n=== 전체 테스트 완료 ===");
        
        if (testResultText != null)
            testResultText.text = testResults;
        
        Debug.Log(testResults);
    }
    
    void TestDiscVisibility()
    {
        AddTestResult("🔍 Disc 오브젝트 가시성 테스트:");
        
        // Disc 프리팹 존재 확인
        GameObject discPrefab = Resources.Load<GameObject>("Prefabs/Disc");
        if (discPrefab != null)
        {
            AddTestResult("  ✅ Disc 프리팹 존재");
        }
        else
        {
            AddTestResult("  ❌ Disc 프리팹 없음");
        }
        
        // Disc.cs 스크립트 확인
        Disc[] discs = FindObjectsOfType<Disc>();
        if (discs.Length > 0)
        {
            AddTestResult($"  ✅ {discs.Length}개의 Disc 오브젝트 발견");
            
            // MiniImage 컴포넌트 확인
            foreach (Disc disc in discs)
            {
                Transform miniImage = disc.transform.Find("MiniImage");
                if (miniImage != null)
                {
                    AddTestResult($"  ✅ Disc '{disc.name}'의 MiniImage 존재");
                }
                else
                {
                    AddTestResult($"  ❌ Disc '{disc.name}'의 MiniImage 없음");
                }
            }
        }
        else
        {
            AddTestResult("  ❌ Disc 오브젝트 없음");
        }
    }
    
    void TestHPDisplay()
    {
        AddTestResult("🔍 HP 수치 표시 테스트:");
        
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            AddTestResult("  ✅ GameManager 발견");
            
            // HP 텍스트 컴포넌트 확인
            if (gameManager.playerHpText != null)
                AddTestResult("  ✅ 1P HP 텍스트 컴포넌트 존재");
            else
                AddTestResult("  ❌ 1P HP 텍스트 컴포넌트 없음");
                
            if (gameManager.cpuHpText != null)
                AddTestResult("  ✅ CPU HP 텍스트 컴포넌트 존재");
            else
                AddTestResult("  ❌ CPU HP 텍스트 컴포넌트 없음");
                
            // HP 바 컴포넌트 확인
            if (gameManager.playerHpBar != null)
                AddTestResult("  ✅ 1P HP 바 컴포넌트 존재");
            else
                AddTestResult("  ❌ 1P HP 바 컴포넌트 없음");
                
            if (gameManager.cpuHpBar != null)
                AddTestResult("  ✅ CPU HP 바 컴포넌트 존재");
            else
                AddTestResult("  ❌ CPU HP 바 컴포넌트 없음");
        }
        else
        {
            AddTestResult("  ❌ GameManager 없음");
        }
    }
    
    void TestSkillUI()
    {
        AddTestResult("🔍 스킬 UI 테스트:");
        
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            if (gameManager.playerSkillButtons != null && gameManager.playerSkillButtons.Length > 0)
            {
                AddTestResult($"  ✅ {gameManager.playerSkillButtons.Length}개의 플레이어 스킬 버튼 존재");
                
                // 각 스킬 버튼의 CooldownText 확인
                for (int i = 0; i < gameManager.playerSkillButtons.Length; i++)
                {
                    if (gameManager.playerSkillButtons[i] != null)
                    {
                        Transform cooldownText = gameManager.playerSkillButtons[i].transform.Find("CooldownText");
                        if (cooldownText != null)
                        {
                            AddTestResult($"  ✅ 플레이어 스킬 {i+1}의 CooldownText 존재");
                        }
                        else
                        {
                            AddTestResult($"  ❌ 플레이어 스킬 {i+1}의 CooldownText 없음");
                        }
                    }
                }
            }
            else
            {
                AddTestResult("  ❌ 플레이어 스킬 버튼 없음");
            }
            
            if (gameManager.cpuSkillButtons != null && gameManager.cpuSkillButtons.Length > 0)
            {
                AddTestResult($"  ✅ {gameManager.cpuSkillButtons.Length}개의 CPU 스킬 버튼 존재");
            }
            else
            {
                AddTestResult("  ❌ CPU 스킬 버튼 없음");
            }
        }
    }
    
    void TestDamageEffects()
    {
        AddTestResult("🔍 데미지 이펙트 테스트:");
        
        EffectManager effectManager = FindFirstObjectByType<EffectManager>();
        if (effectManager != null)
        {
            AddTestResult("  ✅ EffectManager 발견");
            
            if (effectManager.damageTextPrefab != null)
                AddTestResult("  ✅ 데미지 텍스트 프리팹 존재");
            else
                AddTestResult("  ❌ 데미지 텍스트 프리팹 없음");
                
            if (effectManager.skillButtonPrefab != null)
                AddTestResult("  ✅ 스킬 버튼 프리팹 존재");
            else
                AddTestResult("  ❌ 스킬 버튼 프리팹 없음");
        }
        else
        {
            AddTestResult("  ❌ EffectManager 없음");
        }
    }
    
    void TestGameEndResult()
    {
        AddTestResult("🔍 게임 종료 결과 테스트:");
        
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            if (gameManager.resultPanel != null)
                AddTestResult("  ✅ 결과 패널 존재");
            else
                AddTestResult("  ❌ 결과 패널 없음");
                
            if (gameManager.resultTitleText != null)
                AddTestResult("  ✅ 결과 제목 텍스트 존재");
            else
                AddTestResult("  ❌ 결과 제목 텍스트 없음");
                
            if (gameManager.resultDetailText != null)
                AddTestResult("  ✅ 결과 상세 텍스트 존재");
            else
                AddTestResult("  ❌ 결과 상세 텍스트 없음");
                
            if (gameManager.resultContinueButton != null)
                AddTestResult("  ✅ 계속하기 버튼 존재");
            else
                AddTestResult("  ❌ 계속하기 버튼 없음");
                
            if (gameManager.resultRestartButton != null)
                AddTestResult("  ✅ 재시작 버튼 존재");
            else
                AddTestResult("  ❌ 재시작 버튼 없음");
        }
    }
    
    void TestHPBarSegments()
    {
        AddTestResult("🔍 HP바 구분선 테스트:");
        
        Slider[] hpBars = FindObjectsOfType<Slider>();
        if (hpBars.Length > 0)
        {
            AddTestResult($"  ✅ {hpBars.Length}개의 HP바 발견");
            
            foreach (Slider hpBar in hpBars)
            {
                Transform segments = hpBar.transform.Find("Segments");
                if (segments != null)
                {
                    AddTestResult($"  ✅ HP바 '{hpBar.name}'에 구분선 존재");
                    
                    // 구분선 개수 확인
                    int segmentCount = segments.childCount;
                    AddTestResult($"  📊 구분선 개수: {segmentCount}개");
                }
                else
                {
                    AddTestResult($"  ❌ HP바 '{hpBar.name}'에 구분선 없음");
                }
            }
        }
        else
        {
            AddTestResult("  ❌ HP바 없음");
        }
    }
    
    void TestPrefabSystem()
    {
        AddTestResult("🔍 프리팹 시스템 테스트:");
        
        // 프리팹 파일 존재 확인
        string[] prefabFiles = {
            "Assets/Prefabs/DamageText.prefab",
            "Assets/Prefabs/SkillButton.prefab",
            "Assets/Prefabs/Disc.prefab"
        };
        
        foreach (string prefabPath in prefabFiles)
        {
            if (System.IO.File.Exists(prefabPath))
            {
                AddTestResult($"  ✅ {prefabPath} 존재");
            }
            else
            {
                AddTestResult($"  ❌ {prefabPath} 없음");
            }
        }
        
        // ObjectPool 시스템 확인
        ObjectPool objectPool = FindFirstObjectByType<ObjectPool>();
        if (objectPool != null)
        {
            AddTestResult("  ✅ ObjectPool 시스템 존재");
        }
        else
        {
            AddTestResult("  ❌ ObjectPool 시스템 없음");
        }
    }
    
    void TestPerformance()
    {
        AddTestResult("🔍 성능 최적화 테스트:");
        
        ObjectPool objectPool = FindFirstObjectByType<ObjectPool>();
        if (objectPool != null)
        {
            AddTestResult("  ✅ ObjectPool 시스템 존재");
            
            if (objectPool.poolDictionary != null)
            {
                AddTestResult($"  ✅ {objectPool.poolDictionary.Count}개의 오브젝트 풀 존재");
            }
        }
        else
        {
            AddTestResult("  ❌ ObjectPool 시스템 없음");
        }
        
        // FPS 확인
        float fps = 1.0f / Time.deltaTime;
        AddTestResult($"  📊 현재 FPS: {fps:F1}");
        
        // 메모리 사용량 확인
        long memoryUsage = System.GC.GetTotalMemory(false);
        AddTestResult($"  📊 메모리 사용량: {memoryUsage / 1024 / 1024}MB");
    }
    
    void TestCharacterSystem()
    {
        AddTestResult("🔍 캐릭터 시스템 테스트:");
        
        CharacterDataManager characterManager = FindFirstObjectByType<CharacterDataManager>();
        if (characterManager != null)
        {
            AddTestResult("  ✅ CharacterDataManager 발견");
            
            if (characterManager.characters != null)
            {
                AddTestResult($"  ✅ {characterManager.characters.Length}개의 캐릭터 데이터 존재");
            }
            else
            {
                AddTestResult("  ❌ 캐릭터 데이터 없음");
            }
        }
        else
        {
            AddTestResult("  ❌ CharacterDataManager 없음");
        }
        
        CharacterSelectManager selectManager = FindFirstObjectByType<CharacterSelectManager>();
        if (selectManager != null)
        {
            AddTestResult("  ✅ CharacterSelectManager 발견");
        }
        else
        {
            AddTestResult("  ❌ CharacterSelectManager 없음");
        }
    }
    
    void TestBoardSystem()
    {
        AddTestResult("🔍 오셀로 보드 시스템 테스트:");
        
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager != null)
        {
            AddTestResult("  ✅ BoardManager 발견");
            
            if (boardManager.boardCells != null)
            {
                AddTestResult($"  ✅ {boardManager.boardCells.Length}개의 보드 셀 존재");
            }
            else
            {
                AddTestResult("  ❌ 보드 셀 없음");
            }
            
            if (boardManager.discPrefab != null)
            {
                AddTestResult("  ✅ 디스크 프리팹 존재");
            }
            else
            {
                AddTestResult("  ❌ 디스크 프리팹 없음");
            }
        }
        else
        {
            AddTestResult("  ❌ BoardManager 없음");
        }
    }
    
    void TestAudioSystem()
    {
        AddTestResult("🔍 오디오 시스템 테스트:");
        
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            AddTestResult("  ✅ AudioManager 발견");
        }
        else
        {
            AddTestResult("  ❌ AudioManager 없음");
        }
        
        // AudioSource 컴포넌트 확인
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        if (audioSources.Length > 0)
        {
            AddTestResult($"  ✅ {audioSources.Length}개의 AudioSource 발견");
        }
        else
        {
            AddTestResult("  ❌ AudioSource 없음");
        }
    }
    
    void TestMobileSystem()
    {
        AddTestResult("🔍 모바일 시스템 테스트:");
        
        MobileInputManager mobileInput = FindFirstObjectByType<MobileInputManager>();
        if (mobileInput != null)
        {
            AddTestResult("  ✅ MobileInputManager 발견");
        }
        else
        {
            AddTestResult("  ❌ MobileInputManager 없음");
        }
        
        // 터치 입력 지원 확인
        if (Input.touchSupported)
        {
            AddTestResult("  ✅ 터치 입력 지원");
        }
        else
        {
            AddTestResult("  ❌ 터치 입력 미지원");
        }
        
        // 멀티터치 지원 확인
        if (Input.multiTouchEnabled)
        {
            AddTestResult("  ✅ 멀티터치 지원");
        }
        else
        {
            AddTestResult("  ❌ 멀티터치 미지원");
        }
    }
    
    void AddTestResult(string result)
    {
        testResults += result + "\n";
    }
    
    void ClearTestResults()
    {
        testResults = "";
        if (testResultText != null)
            testResultText.text = "";
    }
    
    // 자동 테스트 실행 (씬 로드 시)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoRunTests()
    {
        // 메인 씬에서만 자동 테스트 실행
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            TestManager testManager = FindFirstObjectByType<TestManager>();
            if (testManager != null)
            {
                testManager.RunAllTests();
            }
        }
    }
} 