using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// CPU vs CPU 자동 테스트 시스템
/// 완전한 게임을 자동으로 실행하고 다양한 종료 조건을 테스트
/// </summary>
public class CPUvsCPUTest : MonoBehaviour
{
    [Header("테스트 설정")]
    public bool autoStartTest = false;
    public int maxTestGames = 5;
    public float turnDelay = 0.1f;
    public int maxTurnsPerGame = 200;
    
    [Header("테스트 결과 UI")]
    public Text testResultText;
    public Button startTestButton;
    public Button stopTestButton;
    public Button singleGameTestButton;
    public Button multipleGameTestButton;
    
    [Header("테스트 참조")]
    public GameManager gameManager;
    public BoardManager boardManager;
    
    [Header("통계")]
    public Text statsText;
    
    private string testResults = "";
    private bool isTestRunning = false;
    private bool shouldStopTest = false;
    
    // 테스트 통계
    private int totalGamesPlayed = 0;
    private int hpEndGames = 0;
    private int boardFullGames = 0;
    private int maxTurnsReached = 0;
    private float averageGameLength = 0f;
    private List<float> gameLengths = new List<float>();
    
    void Start()
    {
        if (autoStartTest)
        {
            StartCoroutine(RunMultipleGameTest());
        }
        
        SetupTestButtons();
    }
    
    void SetupTestButtons()
    {
        if (startTestButton != null)
            startTestButton.onClick.AddListener(() => StartCoroutine(RunMultipleGameTest()));
        
        if (stopTestButton != null)
            stopTestButton.onClick.AddListener(StopTest);
        
        if (singleGameTestButton != null)
            singleGameTestButton.onClick.AddListener(() => StartCoroutine(RunSingleGameTest()));
        
        if (multipleGameTestButton != null)
            multipleGameTestButton.onClick.AddListener(() => StartCoroutine(RunMultipleGameTest()));
    }
    
    /// <summary>
    /// 테스트 중지
    /// </summary>
    public void StopTest()
    {
        shouldStopTest = true;
        AddTestResult("🛑 테스트 중지 요청됨");
    }
    
    /// <summary>
    /// 단일 게임 테스트 실행
    /// </summary>
    public IEnumerator RunSingleGameTest()
    {
        if (isTestRunning)
        {
            AddTestResult("테스트가 이미 실행 중입니다.");
            yield break;
        }
        
        isTestRunning = true;
        shouldStopTest = false;
        testResults = "";
        
        AddTestResult("=== 단일 CPU vs CPU 게임 테스트 시작 ===\n");
        
        yield return StartCoroutine(PlaySingleGame());
        
        AddTestResult("\n=== 단일 게임 테스트 완료 ===");
        
        if (testResultText != null)
            testResultText.text = testResults;
        
        isTestRunning = false;
    }
    
    /// <summary>
    /// 다중 게임 테스트 실행
    /// </summary>
    public IEnumerator RunMultipleGameTest()
    {
        if (isTestRunning)
        {
            AddTestResult("테스트가 이미 실행 중입니다.");
            yield break;
        }
        
        isTestRunning = true;
        shouldStopTest = false;
        testResults = "";
        
        AddTestResult("=== 다중 CPU vs CPU 게임 테스트 시작 ===\n");
        
        // 통계 초기화
        ResetStatistics();
        
        for (int gameIndex = 1; gameIndex <= maxTestGames; gameIndex++)
        {
            if (shouldStopTest)
            {
                AddTestResult("테스트가 중지되었습니다.");
                break;
            }
            
            AddTestResult($"🎮 게임 {gameIndex}/{maxTestGames} 시작...");
            
            float gameStartTime = Time.time;
            yield return StartCoroutine(PlaySingleGame());
            float gameEndTime = Time.time;
            
            float gameLength = gameEndTime - gameStartTime;
            gameLengths.Add(gameLength);
            
            AddTestResult($"⏱️ 게임 {gameIndex} 완료 - 소요시간: {gameLength:F2}초");
            
            // 잠시 대기
            yield return new WaitForSeconds(0.5f);
        }
        
        // 최종 통계 출력
        yield return StartCoroutine(DisplayFinalStatistics());
        
        AddTestResult("\n=== 다중 게임 테스트 완료 ===");
        
        if (testResultText != null)
            testResultText.text = testResults;
        
        isTestRunning = false;
    }
    
    /// <summary>
    /// 단일 게임 실행
    /// </summary>
    private IEnumerator PlaySingleGame()
    {
        if (gameManager == null || boardManager == null)
        {
            AddTestResult("  ❌ GameManager 또는 BoardManager가 null입니다.");
            yield break;
        }
        
        // 게임 초기화
        InitializeGame();
        
        int turnCount = 0;
        float gameStartTime = Time.time;
        
        while (turnCount < maxTurnsPerGame)
        {
            if (shouldStopTest)
            {
                AddTestResult("    - 테스트 중지됨");
                break;
            }
            
            turnCount++;
            
            // 게임 종료 조건 체크
            GameEndCondition endCondition = CheckGameEndCondition();
            if (endCondition != GameEndCondition.None)
            {
                ProcessGameEnd(endCondition, turnCount);
                break;
            }
            
            // CPU 턴 진행
            yield return StartCoroutine(ProcessCPUTurn(turnCount));
            
            // 턴 대기
            yield return new WaitForSeconds(turnDelay);
        }
        
        if (turnCount >= maxTurnsPerGame)
        {
            AddTestResult($"    - 최대 턴 수({maxTurnsPerGame})에 도달하여 게임 종료");
            maxTurnsReached++;
        }
        
        float gameEndTime = Time.time;
        float gameLength = gameEndTime - gameStartTime;
        
        AddTestResult($"    📊 게임 완료 - 턴: {turnCount}, 시간: {gameLength:F2}초");
    }
    
    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void InitializeGame()
    {
        // HP 초기화
        gameManager.playerHp = 10000;
        gameManager.cpuHp = 10000;
        
        // 게임 모드 설정
        if (GameData.Instance != null)
        {
            GameData.Instance.gameMode = GameMode.CPUVsCPU;
        }
        
        // 보드 초기화 (BoardManager에서 처리)
        if (boardManager != null)
        {
            // 보드 초기화 로직이 BoardManager에 있다면 호출
        }
    }
    
    /// <summary>
    /// CPU 턴 처리
    /// </summary>
    private IEnumerator ProcessCPUTurn(int turnNumber)
    {
        // 현재 보드 상태 확인
        int blackScore = boardManager.GetBlackScore();
        int whiteScore = boardManager.GetWhiteScore();
        bool isBlackTurn = boardManager.IsBlackTurn();
        
        // 유효한 이동 가능한 위치 찾기
        List<Vector2Int> validMoves = boardManager.GetValidMoves(isBlackTurn);
        
        if (validMoves.Count > 0)
        {
            // 랜덤하게 이동 선택
            Vector2Int selectedMove = validMoves[Random.Range(0, validMoves.Count)];
            
            // 이동 실행
            bool moveSuccess = boardManager.TryPlacePiece(selectedMove.x, selectedMove.y);
            
            if (moveSuccess)
            {
                // 데미지 계산 및 적용
                int damage = CalculateDamage(selectedMove, isBlackTurn);
                ApplyDamage(damage, isBlackTurn);
                
                AddTestResult($"      턴 {turnNumber}: {(isBlackTurn ? "검은돌" : "흰돌")} ({selectedMove.x},{selectedMove.y}) - 데미지: {damage}");
            }
        }
        else
        {
            AddTestResult($"      턴 {turnNumber}: {(isBlackTurn ? "검은돌" : "흰돌")} - 이동 불가능, 턴 스킵");
        }
        
        yield return null;
    }
    
    /// <summary>
    /// 데미지 계산
    /// </summary>
    private int CalculateDamage(Vector2Int move, bool isBlackTurn)
    {
        // 기본 데미지
        int baseDamage = 10;
        
        // 보드 위치에 따른 보너스 데미지
        int positionBonus = 0;
        if (move.x == 0 || move.x == 7 || move.y == 0 || move.y == 7)
        {
            positionBonus = 5; // 모서리 보너스
        }
        
        // 연속 턴 보너스 (간단한 구현)
        int consecutiveBonus = Random.Range(0, 10);
        
        return baseDamage + positionBonus + consecutiveBonus;
    }
    
    /// <summary>
    /// 데미지 적용
    /// </summary>
    private void ApplyDamage(int damage, bool isBlackTurn)
    {
        if (isBlackTurn)
        {
            // 검은돌 턴이면 CPU에게 데미지
            gameManager.cpuHp -= damage;
            if (gameManager.cpuHp < 0) gameManager.cpuHp = 0;
        }
        else
        {
            // 흰돌 턴이면 플레이어에게 데미지
            gameManager.playerHp -= damage;
            if (gameManager.playerHp < 0) gameManager.playerHp = 0;
        }
    }
    
    /// <summary>
    /// 게임 종료 조건 확인
    /// </summary>
    private GameEndCondition CheckGameEndCondition()
    {
        // HP 소진 확인
        if (gameManager.playerHp <= 0)
        {
            return GameEndCondition.Player1HPZero;
        }
        
        if (gameManager.cpuHp <= 0)
        {
            return GameEndCondition.Player2HPZero;
        }
        
        // 보드 가득참 확인
        if (boardManager != null)
        {
            int blackScore = boardManager.GetBlackScore();
            int whiteScore = boardManager.GetWhiteScore();
            if (blackScore + whiteScore >= 64)
            {
                return GameEndCondition.BoardFull;
            }
        }
        
        return GameEndCondition.None;
    }
    
    /// <summary>
    /// 게임 종료 처리
    /// </summary>
    private void ProcessGameEnd(GameEndCondition condition, int turnCount)
    {
        switch (condition)
        {
            case GameEndCondition.Player1HPZero:
                AddTestResult($"    🏁 게임 종료: 플레이어1 HP 소진 (턴 {turnCount})");
                hpEndGames++;
                break;
                
            case GameEndCondition.Player2HPZero:
                AddTestResult($"    🏁 게임 종료: CPU HP 소진 (턴 {turnCount})");
                hpEndGames++;
                break;
                
            case GameEndCondition.BoardFull:
                AddTestResult($"    🏁 게임 종료: 보드 가득참 (턴 {turnCount})");
                boardFullGames++;
                break;
        }
        
        totalGamesPlayed++;
    }
    
    /// <summary>
    /// 최종 통계 표시
    /// </summary>
    private IEnumerator DisplayFinalStatistics()
    {
        AddTestResult("\n📊 === 최종 테스트 통계 ===");
        AddTestResult($"총 게임 수: {totalGamesPlayed}");
        AddTestResult($"HP 소진으로 종료: {hpEndGames}게임");
        AddTestResult($"보드 가득참으로 종료: {boardFullGames}게임");
        AddTestResult($"최대 턴 도달: {maxTurnsReached}게임");
        
        if (gameLengths.Count > 0)
        {
            float totalTime = 0f;
            foreach (float time in gameLengths)
            {
                totalTime += time;
            }
            averageGameLength = totalTime / gameLengths.Count;
            
            AddTestResult($"평균 게임 시간: {averageGameLength:F2}초");
            AddTestResult($"총 테스트 시간: {totalTime:F2}초");
        }
        
        // 통계 UI 업데이트
        if (statsText != null)
        {
            statsText.text = $"게임: {totalGamesPlayed}\nHP 종료: {hpEndGames}\n보드 종료: {boardFullGames}\n평균 시간: {averageGameLength:F1}초";
        }
        
        yield return null;
    }
    
    /// <summary>
    /// 통계 초기화
    /// </summary>
    private void ResetStatistics()
    {
        totalGamesPlayed = 0;
        hpEndGames = 0;
        boardFullGames = 0;
        maxTurnsReached = 0;
        gameLengths.Clear();
    }
    
    /// <summary>
    /// 테스트 결과 추가
    /// </summary>
    private void AddTestResult(string result)
    {
        testResults += result + "\n";
        Debug.Log($"[CPUvsCPUTest] {result}");
    }
}

/// <summary>
/// 게임 종료 조건 열거형
/// </summary>
public enum GameEndCondition
{
    None,
    Player1HPZero,
    Player2HPZero,
    BoardFull
} 