using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
                disc.Initialize(x, y, this);
                // 빈 칸: SetDisc(false, null)로 초기화
                disc.SetDisc(false, null);
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
        FlipPieces(x, y, isBlackTurn);

        // 2. 턴 변경
        SwitchTurn();
        
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
        isBlackTurn = !isBlackTurn;
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
        
        // 첫 번째 칸이 상대방 돌이어야 함
        if (!IsValidPosition(x, y) || !board[x, y].HasPiece() || board[x, y].IsBlack() == isBlack)
            return false;
            
        // 연속된 상대방 돌 확인
        while (IsValidPosition(x, y) && board[x, y].HasPiece() && board[x, y].IsBlack() != isBlack)
        {
            x += dx;
            y += dy;
        }
        
        // 마지막에 자신의 돌이 있어야 함
        return IsValidPosition(x, y) && board[x, y].HasPiece() && board[x, y].IsBlack() == isBlack;
    }
    
    void FlipPieces(int startX, int startY, bool isBlack)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
        
        for (int i = 0; i < 8; i++)
        {
            FlipInDirection(startX, startY, dx[i], dy[i], isBlack);
        }
    }
    
    void FlipInDirection(int startX, int startY, int dx, int dy, bool isBlack)
    {
        List<Disc> toFlip = new List<Disc>();
        int x = startX + dx;
        int y = startY + dy;
        
        // 연속된 상대방 돌 수집
        while (IsValidPosition(x, y) && board[x, y].HasPiece() && board[x, y].IsBlack() != isBlack)
        {
            toFlip.Add(board[x, y]);
            x += dx;
            y += dy;
        }
        
        // 마지막에 자신의 돌이 있으면 뒤집기
        if (IsValidPosition(x, y) && board[x, y].HasPiece() && board[x, y].IsBlack() == isBlack)
        {
            foreach (Disc disc in toFlip)
            {
                disc.Flip();
            }
        }
    }
    
    void UpdateValidMoves()
    {
        validMoves.Clear();
        
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (IsValidMove(x, y, isBlackTurn))
                {
                    validMoves.Add(new Vector2Int(x, y));
                }
            }
        }
        
        // 유효한 수가 없으면 턴 스킵
        if (validMoves.Count == 0)
        {
            isBlackTurn = !isBlackTurn;
            UpdateValidMoves();
            
            // 양쪽 모두 수가 없으면 게임 종료
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
                // 랜덤하게 수를 둠
                Vector2Int randomMove = validMoves[Random.Range(0, validMoves.Count)];
                TryPlacePiece(randomMove.x, randomMove.y);
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