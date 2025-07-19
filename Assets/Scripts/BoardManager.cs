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
    
    void Start()
    {
        InitializeBoard();
        SetupInitialPieces();
        UpdateValidMoves();
        UpdateScore();
        UpdateUI();
        
        // 주사위 결과에 따른 선공 설정
        if (GameData.Instance.isFirstTurnDetermined)
        {
            // 주사위 결과에 따라 선공 설정
            if (!GameData.Instance.isPlayer1First)
            {
                // 2P가 선공이면 턴 변경
                isBlackTurn = false;
                UpdateUI();
            }
        }
    }
    
    void InitializeBoard()
    {
        board = new Disc[boardSize, boardSize];
        
        // 보드 생성
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject discObj = Instantiate(discPrefab, boardContainer);
                discObj.transform.localPosition = new Vector3(x, y, 0);
                
                Disc disc = discObj.GetComponent<Disc>();
                disc.Initialize(x, y, this);
                board[x, y] = disc;
            }
        }
    }
    
    void SetupInitialPieces()
    {
        // 초기 4개 돌 배치 (오셀로 표준 규칙)
        int center = boardSize / 2;
        
        // 중앙 4개 위치에 초기 돌 배치
        PlacePiece(center - 1, center - 1, false); // 흰돌
        PlacePiece(center, center, false); // 흰돌
        PlacePiece(center - 1, center, true); // 검은돌
        PlacePiece(center, center - 1, true); // 검은돌
    }
    
    public bool PlacePiece(int x, int y, bool isBlack)
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize)
            return false;
            
        if (board[x, y].HasPiece())
            return false;
            
        // 유효한 위치인지 확인
        if (!IsValidMove(x, y, isBlack))
            return false;
            
        // 돌 배치
        board[x, y].SetPiece(isBlack);
        
        // 상대방 돌 뒤집기
        FlipPieces(x, y, isBlack);
        
        // 턴 변경
        isBlackTurn = !isBlackTurn;
        
        // 점수 업데이트
        UpdateScore();
        
        // 다음 유효한 수 확인
        UpdateValidMoves();
        
        // UI 업데이트
        UpdateUI();
        
        // 게임 종료 체크
        CheckGameEnd();
        
        return true;
    }
    
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
    
    void CheckGameEnd()
    {
        // 보드가 가득 찼거나 양쪽 모두 수가 없으면 게임 종료
        if (blackScore + whiteScore >= boardSize * boardSize || validMoves.Count == 0)
        {
            EndGame();
        }
    }
    
    void EndGame()
    {
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
                PlacePiece(randomMove.x, randomMove.y, isBlackTurn);
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
                PlacePiece(x, y, isBlackTurn);
            }
        }
        else
        {
            // 1P vs CPU 모드에서는 1P만 수를 둘 수 있음
            if (isPlayer1 && isBlackTurn)
            {
                PlacePiece(x, y, isBlackTurn);
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
    
    // 유효한 수 하이라이트 표시
    public void ShowHighlights(List<Vector2Int> positions, Sprite highlightSprite)
    {
        // 기존 하이라이트 제거
        Transform[] existingHighlights = boardContainer.GetComponentsInChildren<Transform>();
        foreach (Transform highlight in existingHighlights)
        {
            if (highlight.name.Contains("Highlight"))
            {
                Destroy(highlight.gameObject);
            }
        }
        
        // 새로운 하이라이트 추가
        foreach (Vector2Int pos in positions)
        {
            if (IsValidPosition(pos.x, pos.y))
            {
                GameObject highlight = new GameObject("Highlight");
                highlight.transform.SetParent(boardContainer);
                highlight.transform.localPosition = new Vector3(pos.x, pos.y, -0.1f);
                
                SpriteRenderer sr = highlight.AddComponent<SpriteRenderer>();
                sr.sprite = highlightSprite;
                sr.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
    
    // 돌 배치 (GameManager에서 호출)
    public void PlaceDisc(int x, int y, bool isBlack)
    {
        PlacePiece(x, y, isBlack);
    }
    
    // 돌 뒤집기 (GameManager에서 호출)
    public void FlipDisc(int x, int y)
    {
        if (IsValidPosition(x, y) && board[x, y].HasPiece())
        {
            board[x, y].Flip();
        }
    }
} 