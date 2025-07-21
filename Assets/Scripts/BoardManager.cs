using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // 코루틴을 사용하기 위해 추가

public class BoardManager : MonoBehaviour
{
    [Header("보드 설정")]
    public int boardSize = 8;
    public GameObject discPrefab;
    public Transform boardContainer;
    
    [Header("UI 연결")]
    public Text turnText;
    public Text resultText;
    public Text blackScoreText;
    public Text whiteScoreText;
    public Text autoPlayText; // CPU vs CPU 모드에서 자동 진행 표시
    
    public Disc[,] board;
    private bool isBlackTurn = true; // true = 검은돌(1P), false = 흰돌(CPU)
    private bool gameEnded = false;
    
    // 게임 상태
    private int blackScore = 0;
    private int whiteScore = 0;
    private List<Vector2Int> validMoves = new List<Vector2Int>();
    
    #region Unity Lifecycle Methods
    
    void Start()
    {
        // 필수 컴포넌트 확인
        if (!ValidateComponents())
        {
            enabled = false; // 필수 컴포넌트 없으면 동작 중지
            return;
        }

        InitializeBoard();
        SetupInitialPieces();
        
        // 선공 설정
        if (GameData.Instance != null && GameData.Instance.isFirstTurnDetermined && !GameData.Instance.isPlayer1First)
        {
            isBlackTurn = false;
        }

        // 게임 시작
        StartGame();
    }

    #endregion

    #region Initialization Methods

    bool ValidateComponents()
    {
        bool isValid = true;
        if (discPrefab == null) { Debug.LogError("discPrefab이 할당되지 않았습니다."); isValid = false; }
        if (boardContainer == null) { Debug.LogError("boardContainer가 할당되지 않았습니다."); isValid = false; }
        return isValid;
    }
    
    void InitializeBoard()
    {
        board = new Disc[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject discObj = Instantiate(discPrefab, boardContainer);
                discObj.transform.localPosition = new Vector3(x, y, 0);
                Disc disc = discObj.GetComponent<Disc>();
                if (disc == null)
                {
                    Debug.LogError($"[InitializeBoard] Disc 프리팹에 Disc 컴포넌트가 없습니다! ({x},{y})");
                }
                else
                {
                    Debug.Log($"[InitializeBoard] board[{x},{y}]에 Disc 할당 완료");
                }
                disc.Initialize(x, y, this);
                // 빈 칸: SetDisc(false, null)로 초기화
                board[x, y] = disc;
            }
        }
    }
    
    void SetupInitialPieces()
    {
        // 초기 4개 돌 배치 (오셀로 표준 규칙)
        int center = boardSize / 2;
        
        // 중앙 4개 위치에 초기 돌 배치
        // PlacePiece -> SetDiscOnBoard로 변경
        SetDiscOnBoard(center - 1, center - 1, false); // 흰돌
        SetDiscOnBoard(center, center, false); // 흰돌
        SetDiscOnBoard(center - 1, center, true); // 검은돌
        SetDiscOnBoard(center, center - 1, true); // 검은돌
    }

    #endregion

    #region Game Flow

    void StartGame()
    {
        gameEnded = false;
        UpdateTurn();
    }

    // PlacePiece -> TryPlacePiece로 변경하고 역할 분리
    public bool TryPlacePiece(int x, int y)
    {
        if (gameEnded || !IsValidMove(x, y, isBlackTurn))
        {
            return false;
        }

        // 1. 돌 배치 및 뒤집기
        SetDiscOnBoard(x, y, isBlackTurn);
        board[x, y].AnimatePlace(); // 돌 놓기 애니메이션 추가
        StartCoroutine(FlipPiecesWithEffect(x, y, isBlackTurn));

        // 로그: 일반 수 두기
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.LogMove(x, y);

        // 2. 턴 전환은 코루틴에서 애니메이션 끝난 후 처리
        return true;
    }

    void SetDiscOnBoard(int x, int y, bool isBlack)
    {
        if (!IsValidPosition(x, y) || (board[x, y] != null && board[x, y].HasPiece())) return;

        CharacterData selectedChar = isBlack 
            ? (GameData.Instance != null ? GameData.Instance.selectedCharacter1P : null)
            : (GameData.Instance != null ? GameData.Instance.selectedCharacterCPU : null);
        
        Sprite miniSprite = (selectedChar != null) ? selectedChar.discSprite : null;
        board[x, y].SetDisc(isBlack, miniSprite);
    }
    
    void SwitchTurn()
    {
        // 턴 종료 로그
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.LogTurnEnd();
        isBlackTurn = !isBlackTurn;
        // 턴 시작 로그
        if (gm != null) gm.LogTurnStart();
        UpdateTurn();
    }

    void UpdateTurn()
    {
        UpdateValidMoves();
        
        // 현재 턴에 둘 곳이 없으면 상대방에게 턴 넘김
        if (validMoves.Count == 0)
        {
            isBlackTurn = !isBlackTurn;
            UpdateValidMoves();

            // 양쪽 모두 둘 곳이 없으면 게임 종료
            if (validMoves.Count == 0)
            {
                EndGame();
                return;
            }
        }
        
        UpdateAllUI();
        CheckGameEndCondition();

        // GameManager에 유효한 수 목록을 전달하여 커서 상태 업데이트에 사용하도록 할 수 있음
        // (이번 단계에서는 직접적인 호출 대신, GameManager가 GetValidMoves를 사용하도록 유도)
    }

    #endregion

    #region UI Update Methods

    void UpdateAllUI()
    {
        UpdateScore();
        UpdateTurnDisplay();
        UpdateScoreDisplay();
        // 필요하다면 하이라이트 표시 등 추가
    }

    void UpdateScoreDisplay()
    {
        if (blackScoreText != null) blackScoreText.text = blackScore.ToString();
        if (whiteScoreText != null) whiteScoreText.text = whiteScore.ToString();
    }
    
    void UpdateUI()
    {
        UpdateScore();
        UpdateTurnDisplay();
        UpdateValidMoves();
        
        // 게임 모드에 따른 추가 UI 업데이트
        if (GameData.Instance.IsCPUVsCPUMode())
        {
            // CPU vs CPU 모드에서는 자동 진행 표시
            if (autoPlayText != null)
            {
                autoPlayText.text = "CPU vs CPU 자동 진행 중...";
            }
        }
        else
        {
            if (autoPlayText != null)
            {
                autoPlayText.text = "";
            }
        }
    }
    
    // 게임 모드에 따른 턴 표시 업데이트
    void UpdateTurnDisplay()
    {
        if (turnText != null)
        {
            string turnInfo = "";
            switch (GameData.Instance.GetGameMode())
            {
                case GameMode.PlayerVsCPU:
                    turnInfo = isBlackTurn ? "1P 턴" : "CPU 턴";
                    break;
                case GameMode.PlayerVsPlayer:
                    turnInfo = isBlackTurn ? "1P 턴" : "2P 턴";
                    break;
                case GameMode.CPUVsCPU:
                    turnInfo = isBlackTurn ? "CPU1 턴" : "CPU2 턴";
                    break;
            }
            turnText.text = turnInfo;
        }
    }

    #endregion

    #region Game Logic

    public bool IsValidMove(int x, int y, bool isBlack)
    {
        if (!IsValidPosition(x, y)) return false; // 좌표 유효성 먼저 체크
        if (board[x, y] == null)
        {
            Debug.LogError($"[IsValidMove] board[{x},{y}]가 null입니다!");
            return false;
        }
        if (board[x, y].HasPiece())
            return false;
            
        // 8방향 체크
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
        
        for (int i = 0; i < 8; i++)
        {
            if (CanFlipInDirection(x, y, dx[i], dy[i], isBlack))
                return true;
        }
        
        return false;
    }
    
    bool CanFlipInDirection(int startX, int startY, int dx, int dy, bool isBlack)
    {
        int x = startX + dx;
        int y = startY + dy;
        
        if (!IsValidPosition(x, y)) return false;
        
        // 첫 번째 칸이 상대방 돌이어야 함
        if (board[x, y] == null || !board[x, y].HasPiece() || board[x, y].IsBlack() == isBlack)
            return false;
            
        // 연속된 상대방 돌 확인
        x += dx;
        y += dy;
        while (IsValidPosition(x, y))
        {
            if (board[x, y] == null || !board[x, y].HasPiece()) return false;
            if (board[x, y].IsBlack() == isBlack) return true; // 마지막에 자신의 돌을 찾음
            x += dx;
            y += dy;
        }
        
        return false; // 경계를 벗어남
    }
    
    // 기존 FlipPieces/FlipInDirection은 사용하지 않고, 아래 코루틴으로 대체
    private IEnumerator FlipPiecesWithEffect(int startX, int startY, bool isBlack)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
        List<Disc> allToFlip = new List<Disc>();
        List<int> flipCounts = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            List<Disc> toFlip = GetFlippableDiscsInDirection(startX, startY, dx[i], dy[i], isBlack);
            if (toFlip.Count > 0)
            {
                allToFlip.AddRange(toFlip);
                flipCounts.Add(toFlip.Count);
            }
        }
        // 가까운 순서대로(거리순) 정렬
        allToFlip.Sort((a, b) =>
        {
            float da = Vector2Int.Distance(new Vector2Int(startX, startY), new Vector2Int(a.X, a.Y));
            float db = Vector2Int.Distance(new Vector2Int(startX, startY), new Vector2Int(b.X, b.Y));
            return da.CompareTo(db);
        });
        // 하나씩 0.08초 간격으로 뒤집기
        foreach (Disc disc in allToFlip)
        {
            disc.Flip();
            disc.AnimateFlip();
            yield return new WaitForSeconds(0.08f);
        }
        // 뒤집은 개수 및 데미지 계산
        int totalFlipped = allToFlip.Count;
        int damage = totalFlipped;
        int multiplierCount = 0;
        if (totalFlipped >= 5)
        {
            multiplierCount = totalFlipped / 5;
            float multiplier = Mathf.Pow(1.2f, multiplierCount);
            damage = Mathf.RoundToInt(totalFlipped * multiplier);
        }
        // 데미지 적용 및 로그
        if (totalFlipped > 0)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                if (isBlack)
                {
                    gm.ApplyDamageToCPU(damage);
                    Debug.Log($"CPU1이 돌 {totalFlipped}개를 뒤집어 CPU2에게 {damage} 데미지! CPU2 HP: {gm.cpuHp}/10000");
                    if (multiplierCount > 0 && EffectManager.Instance != null)
                        EffectManager.Instance.ShowSpecialEffect(multiplierCount, false, damage, gm.cpuHp, 10000);
                }
                else
                {
                    gm.ApplyDamageToPlayer1(damage);
                    Debug.Log($"CPU2가 돌 {totalFlipped}개를 뒤집어 CPU1에게 {damage} 데미지! CPU1 HP: {gm.playerHp}/10000");
                    if (multiplierCount > 0 && EffectManager.Instance != null)
                        EffectManager.Instance.ShowSpecialEffect(multiplierCount, true, damage, gm.playerHp, 10000);
                }
            }
        }
        else
        {
            Debug.Log($"뒤집힌 돌 없음");
        }
        // 턴 전환
        SwitchTurn();
    }

    // 각 방향별 뒤집을 돌 리스트 반환
    private List<Disc> GetFlippableDiscsInDirection(int startX, int startY, int dx, int dy, bool isBlack)
    {
        List<Disc> toFlip = new List<Disc>();
        int x = startX + dx;
        int y = startY + dy;
        while (IsValidPosition(x, y) && board[x, y] != null && board[x, y].HasPiece() && board[x, y].IsBlack() != isBlack)
        {
            toFlip.Add(board[x, y]);
            x += dx;
            y += dy;
        }
        if (IsValidPosition(x, y) && board[x, y] != null && board[x, y].HasPiece() && board[x, y].IsBlack() == isBlack)
        {
            return toFlip;
        }
        return new List<Disc>(); // 뒤집을 수 없는 경우 빈 리스트 반환
    }
    
    // 헬퍼: 특정 턴에서 validMoves 계산
    void CalculateValidMoves(bool turn)
    {
        validMoves.Clear();
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (IsValidMove(x, y, turn))
                {
                    validMoves.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    void UpdateValidMoves()
    {
        CalculateValidMoves(isBlackTurn);
        // 유효한 수가 없으면 턴 스킵
        if (validMoves.Count == 0)
        {
            isBlackTurn = !isBlackTurn;
            CalculateValidMoves(isBlackTurn);
            if (validMoves.Count == 0)
            {
                EndGame();
            }
        }
    }
    
    void UpdateScore()
    {
        blackScore = 0;
        whiteScore = 0;
        
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (board[x, y].HasPiece())
                {
                    if (board[x, y].IsBlack())
                        blackScore++;
                    else
                        whiteScore++;
                }
            }
        }
    }
    
    void CheckGameEndCondition()
    {
        // 보드가 가득 찼을 때
        if (blackScore + whiteScore >= boardSize * boardSize)
        {
            EndGame();
        }
    }
    
    void EndGame()
    {
        if (gameEnded) return; // 중복 실행 방지
        gameEnded = true;
        
        if (resultText != null)
        {
            if (blackScore > whiteScore)
            {
                resultText.text = "1P 승리!";
            }
            else if (whiteScore > blackScore)
            {
                resultText.text = "CPU 승리!";
            }
            else
            {
                resultText.text = "무승부!";
            }
        }
        
        // GameManager에 결과 전달
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            int flippedCount = Mathf.Abs(blackScore - whiteScore);
            // OnOthelloGameEnd 메서드가 없으므로 직접 처리
            Debug.Log($"오셀로 게임 종료 - 1P: {blackScore}, CPU: {whiteScore}, 차이: {flippedCount}");
        }
    }
    
    bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
    }
    
    public bool IsGameEnded()
    {
        return gameEnded;
    }
    
    public bool IsBlackTurn()
    {
        return isBlackTurn;
    }
    
    public List<Vector2Int> GetValidMoves()
    {
        return validMoves;
    }
    
    public List<Vector2Int> GetValidMoves(bool isBlack)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (IsValidMove(x, y, isBlack))
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }
        return moves;
    }
    
    public int GetBlackScore()
    {
        return blackScore;
    }
    
    public int GetWhiteScore()
    {
        return whiteScore;
    }
    
    // AI를 위한 메서드
    public void MakeAIMove()
    {
        if (GameData.Instance.IsCPUVsCPUMode())
        {
            // CPU가 자동으로 수를 둠
            List<Vector2Int> validMoves = GetValidMoves(isBlackTurn);
            if (validMoves.Count > 0)
            {
                Vector2Int randomMove = validMoves[Random.Range(0, validMoves.Count)];
                TryPlacePiece(randomMove.x, randomMove.y);
                // 로그는 TryPlacePiece에서 자동으로 남음
            }
        }
    }
    
    // 2P 모드에서 수 두기
    public void PlacePieceForPlayer(int x, int y, bool isPlayer1)
    {
        if (GameData.Instance.IsTwoPlayerMode())
        {
            // 2P 모드에서는 현재 턴에 맞는 플레이어만 수를 둘 수 있음
            if ((isBlackTurn && isPlayer1) || (!isBlackTurn && !isPlayer1))
            {
                TryPlacePiece(x, y);
            }
        }
        else
        {
            // 1P vs CPU 모드에서는 1P만 수를 둘 수 있음
            if (isPlayer1 && isBlackTurn)
            {
                TryPlacePiece(x, y);
            }
        }
    }
    
    #endregion
    
    // ShowHighlights 메서드 삭제 (GameManager의 boardSelector로 대체)

    // PlaceDisc는 GameManager.TryPlacePieceAt에서 이미 BoardManager.TryPlacePiece를 호출하므로,
    // 중복되는 public 메서드인 PlaceDisc(int, int, bool)는 제거하거나 private으로 변경 가능.
    // 여기서는 일단 유지하되, 향후 리팩토링 시 제거 고려.
} 