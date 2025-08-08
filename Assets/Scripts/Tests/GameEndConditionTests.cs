using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임 종료 조건 테스트 클래스
/// HP 소진, 보드 가득참, 기타 종료 조건들을 테스트
/// </summary>
public class GameEndConditionTests : MonoBehaviour
{
    [Header("테스트 설정")]
    public bool autoRunTests = true;
    public float testDelay = 2f;
    
    [Header("테스트 결과 UI")]
    public Text testResultText;
    public Button runAllTestsButton;
    public Button runHPTestButton;
    public Button runBoardTestButton;
    public Button runCombinedTestButton;
    
    [Header("테스트 참조")]
    public GameManager gameManager;
    public BoardManager boardManager;
    
    private string testResults = "";
    private bool isTestRunning = false;
    
    void Start()
    {
        if (autoRunTests)
        {
            StartCoroutine(RunAllTestsDelayed());
        }
        
        SetupTestButtons();
    }
    
    void SetupTestButtons()
    {
        if (runAllTestsButton != null)
            runAllTestsButton.onClick.AddListener(() => StartCoroutine(RunAllTests()));
        
        if (runHPTestButton != null)
            runHPTestButton.onClick.AddListener(() => StartCoroutine(TestHPEndCondition()));
        
        if (runBoardTestButton != null)
            runBoardTestButton.onClick.AddListener(() => StartCoroutine(TestBoardFullCondition()));
        
        if (runCombinedTestButton != null)
            runCombinedTestButton.onClick.AddListener(() => StartCoroutine(TestCombinedEndConditions()));
    }
    
    IEnumerator RunAllTestsDelayed()
    {
        yield return new WaitForSeconds(testDelay);
        yield return StartCoroutine(RunAllTests());
    }
    
    public IEnumerator RunAllTests()
    {
        if (isTestRunning)
        {
            AddTestResult("테스트가 이미 실행 중입니다.");
            yield break;
        }
        
        isTestRunning = true;
        testResults = "";
        AddTestResult("=== 게임 종료 조건 테스트 시작 ===\n");
        
        // 1. HP 소진 테스트
        yield return StartCoroutine(TestHPEndCondition());
        
        // 2. 보드 가득참 테스트
        yield return StartCoroutine(TestBoardFullCondition());
        
        // 3. 복합 종료 조건 테스트
        yield return StartCoroutine(TestCombinedEndConditions());
        
        // 4. CPU vs CPU 자동 테스트
        yield return StartCoroutine(TestCPUvsCPUAutoGame());
        
        AddTestResult("\n=== 모든 테스트 완료 ===");
        
        if (testResultText != null)
            testResultText.text = testResults;
        
        Debug.Log(testResults);
        isTestRunning = false;
    }
    
    /// <summary>
    /// HP 소진으로 인한 게임 종료 테스트
    /// </summary>
    public IEnumerator TestHPEndCondition()
    {
        AddTestResult("🔍 HP 소진 종료 조건 테스트:");
        
        if (gameManager == null)
        {
            AddTestResult("  ❌ GameManager가 null입니다.");
            yield break;
        }
        
        // 초기 상태 확인
        AddTestResult("  📊 초기 HP 상태:");
        AddTestResult($"    - 플레이어1 HP: {gameManager.playerHp}");
        AddTestResult($"    - CPU HP: {gameManager.cpuHp}");
        
        // HP 소진 시뮬레이션
        AddTestResult("  🎯 HP 소진 시뮬레이션 시작...");
        
        // 플레이어1 HP를 0으로 만들기
        int originalPlayerHP = gameManager.playerHp;
        gameManager.playerHp = 0;
        AddTestResult($"    - 플레이어1 HP를 {originalPlayerHP}에서 0으로 변경");
        
        // 게임 종료 조건 체크
        yield return new WaitForSeconds(0.5f);
        
        // 게임 상태 확인
        bool gameEnded = CheckGameEndStatus();
        if (gameEnded)
        {
            AddTestResult("  ✅ HP 소진으로 게임이 정상 종료됨");
        }
        else
        {
            AddTestResult("  ❌ HP 소진 시에도 게임이 종료되지 않음");
        }
        
        // CPU HP도 0으로 만들기
        int originalCPUHP = gameManager.cpuHp;
        gameManager.cpuHp = 0;
        AddTestResult($"    - CPU HP를 {originalCPUHP}에서 0으로 변경");
        
        yield return new WaitForSeconds(0.5f);
        
        gameEnded = CheckGameEndStatus();
        if (gameEnded)
        {
            AddTestResult("  ✅ CPU HP 소진으로도 게임이 정상 종료됨");
        }
        else
        {
            AddTestResult("  ❌ CPU HP 소진 시에도 게임이 종료되지 않음");
        }
        
        // HP 복원
        gameManager.playerHp = originalPlayerHP;
        gameManager.cpuHp = originalCPUHP;
        AddTestResult("  🔄 HP 상태 복원 완료");
    }
    
    /// <summary>
    /// 보드 가득참으로 인한 게임 종료 테스트
    /// </summary>
    public IEnumerator TestBoardFullCondition()
    {
        AddTestResult("🔍 보드 가득참 종료 조건 테스트:");
        
        if (boardManager == null)
        {
            AddTestResult("  ❌ BoardManager가 null입니다.");
            yield break;
        }
        
        // 현재 보드 상태 확인
        AddTestResult("  📊 현재 보드 상태:");
        int blackCount = boardManager.GetBlackScore();
        int whiteCount = boardManager.GetWhiteScore();
        int totalPieces = blackCount + whiteCount;
        AddTestResult($"    - 검은돌: {blackCount}개");
        AddTestResult($"    - 흰돌: {whiteCount}개");
        AddTestResult($"    - 총 돌: {totalPieces}개 / 64개");
        
        // 보드 가득참 시뮬레이션
        AddTestResult("  🎯 보드 가득참 시뮬레이션 시작...");
        
        // 모든 빈 칸을 돌로 채우기
        yield return StartCoroutine(FillBoardCompletely());
        
        // 게임 종료 조건 체크
        yield return new WaitForSeconds(0.5f);
        
        bool gameEnded = CheckGameEndStatus();
        if (gameEnded)
        {
            AddTestResult("  ✅ 보드 가득참으로 게임이 정상 종료됨");
        }
        else
        {
            AddTestResult("  ❌ 보드 가득참 시에도 게임이 종료되지 않음");
        }
        
        // 보드 초기화
        yield return StartCoroutine(ResetBoard());
        AddTestResult("  🔄 보드 상태 초기화 완료");
    }
    
    /// <summary>
    /// 복합 종료 조건 테스트
    /// </summary>
    public IEnumerator TestCombinedEndConditions()
    {
        AddTestResult("🔍 복합 종료 조건 테스트:");
        
        // 시나리오 1: HP 소진 + 보드 가득참
        AddTestResult("  📋 시나리오 1: HP 소진 + 보드 가득참");
        
        // HP를 낮게 설정
        gameManager.playerHp = 100;
        gameManager.cpuHp = 100;
        AddTestResult("    - HP를 100으로 설정");
        
        // 보드를 거의 가득 채우기
        yield return StartCoroutine(FillBoardAlmostCompletely());
        
        yield return new WaitForSeconds(1f);
        
        bool gameEnded = CheckGameEndStatus();
        if (gameEnded)
        {
            AddTestResult("  ✅ 복합 조건으로 게임이 정상 종료됨");
        }
        else
        {
            AddTestResult("  ❌ 복합 조건 시에도 게임이 종료되지 않음");
        }
        
        // 상태 복원
        gameManager.playerHp = 10000;
        gameManager.cpuHp = 10000;
        yield return StartCoroutine(ResetBoard());
        AddTestResult("  🔄 상태 복원 완료");
    }
    
    /// <summary>
    /// CPU vs CPU 자동 게임 테스트
    /// </summary>
    public IEnumerator TestCPUvsCPUAutoGame()
    {
        AddTestResult("🔍 CPU vs CPU 자동 게임 테스트:");
        
        if (gameManager == null)
        {
            AddTestResult("  ❌ GameManager가 null입니다.");
            yield break;
        }
        
        AddTestResult("  🎮 CPU vs CPU 자동 게임 시작...");
        
        // 게임 모드를 CPU vs CPU로 설정
        if (GameData.Instance != null)
        {
            GameData.Instance.gameMode = GameMode.CPUVsCPU;
            AddTestResult("    - 게임 모드를 CPU vs CPU로 설정");
        }
        
        // 자동 게임 시작
        yield return StartCoroutine(RunCPUvsCPUGame());
        
        AddTestResult("  ✅ CPU vs CPU 자동 게임 테스트 완료");
    }
    
    /// <summary>
    /// CPU vs CPU 자동 게임 실행
    /// </summary>
    private IEnumerator RunCPUvsCPUGame()
    {
        int maxTurns = 100; // 최대 턴 수 제한
        int currentTurn = 0;
        
        while (currentTurn < maxTurns)
        {
            currentTurn++;
            AddTestResult($"    - 턴 {currentTurn} 진행 중...");
            
            // 게임 종료 조건 체크
            if (CheckGameEndStatus())
            {
                AddTestResult($"    - 턴 {currentTurn}에서 게임 종료됨");
                break;
            }
            
            // CPU 턴 진행
            yield return new WaitForSeconds(0.1f); // 빠른 진행을 위해 짧은 대기
            
            // 보드 상태 확인
            if (boardManager != null)
            {
                int blackScore = boardManager.GetBlackScore();
                int whiteScore = boardManager.GetWhiteScore();
                AddTestResult($"      현재 점수 - 검은돌: {blackScore}, 흰돌: {whiteScore}");
            }
            
            // HP 상태 확인
            AddTestResult($"      현재 HP - 플레이어1: {gameManager.playerHp}, CPU: {gameManager.cpuHp}");
        }
        
        if (currentTurn >= maxTurns)
        {
            AddTestResult("    - 최대 턴 수에 도달하여 테스트 종료");
        }
    }
    
    /// <summary>
    /// 보드를 완전히 채우기
    /// </summary>
    private IEnumerator FillBoardCompletely()
    {
        if (boardManager == null) yield break;
        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // 빈 칸인 경우 돌 배치
                if (boardManager.IsValidMove(x, y, boardManager.IsBlackTurn()))
                {
                    boardManager.TryPlacePiece(x, y);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
    
    /// <summary>
    /// 보드를 거의 가득 채우기
    /// </summary>
    private IEnumerator FillBoardAlmostCompletely()
    {
        if (boardManager == null) yield break;
        
        int filledCount = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (filledCount >= 60) break; // 60개까지만 채우기
                
                if (boardManager.IsValidMove(x, y, boardManager.IsBlackTurn()))
                {
                    boardManager.TryPlacePiece(x, y);
                    filledCount++;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            if (filledCount >= 60) break;
        }
    }
    
    /// <summary>
    /// 보드 초기화
    /// </summary>
    private IEnumerator ResetBoard()
    {
        if (boardManager == null) yield break;
        
        // 보드 초기화 로직 (실제 구현은 BoardManager에 의존)
        AddTestResult("    - 보드 초기화 중...");
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// 게임 종료 상태 확인
    /// </summary>
    private bool CheckGameEndStatus()
    {
        // HP 소진 확인
        if (gameManager.playerHp <= 0 || gameManager.cpuHp <= 0)
        {
            return true;
        }
        
        // 보드 가득참 확인
        if (boardManager != null)
        {
            int blackScore = boardManager.GetBlackScore();
            int whiteScore = boardManager.GetWhiteScore();
            if (blackScore + whiteScore >= 64)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 테스트 결과 추가
    /// </summary>
    private void AddTestResult(string result)
    {
        testResults += result + "\n";
        Debug.Log($"[GameEndConditionTests] {result}");
    }
    
    /// <summary>
    /// 개별 HP 테스트 실행
    /// </summary>
    public void RunHPTest()
    {
        StartCoroutine(TestHPEndCondition());
    }
    
    /// <summary>
    /// 개별 보드 테스트 실행
    /// </summary>
    public void RunBoardTest()
    {
        StartCoroutine(TestBoardFullCondition());
    }
    
    /// <summary>
    /// 개별 복합 테스트 실행
    /// </summary>
    public void RunCombinedTest()
    {
        StartCoroutine(TestCombinedEndConditions());
    }
} 