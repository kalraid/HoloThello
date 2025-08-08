using UnityEngine;
using UnityEditor;

public class BoardTestUtility : MonoBehaviour
{
    [MenuItem("Tools/Board Test/Initialize Board Test")]
    public static void InitializeBoardTest()
    {
        Debug.Log("=== 보드 초기화 테스트 시작 ===");
        
        // BoardManager 찾기
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager를 찾을 수 없습니다!");
            return;
        }
        
        Debug.Log($"BoardManager 발견: {boardManager.name}");
        
        // BoardArea 찾기
        GameObject boardArea = GameObject.Find("BoardArea");
        if (boardArea == null)
        {
            Debug.LogError("BoardArea를 찾을 수 없습니다!");
            return;
        }
        
        Debug.Log($"BoardArea 발견: {boardArea.name}");
        
        // Cell_x_x 오브젝트들 확인
        int cellCount = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                string cellName = $"Cell_{x}_{y}";
                Transform cell = boardArea.transform.Find(cellName);
                if (cell != null)
                {
                    cellCount++;
                    Debug.Log($"Cell 발견: {cellName} at {cell.position}");
                }
                else
                {
                    Debug.LogWarning($"Cell 없음: {cellName}");
                }
            }
        }
        
        Debug.Log($"총 {cellCount}개의 Cell 발견");
        
        // 보드 초기화 강제 실행
        if (boardManager.board == null)
        {
            Debug.Log("보드를 수동으로 초기화합니다...");
            boardManager.InitializeBoard();
            boardManager.SetupInitialPieces();
        }
        
        // 초기 돌 배치 테스트
        TestInitialPiecePlacement(boardManager);
    }
    
    [MenuItem("Tools/Board Test/Place Piece Test")]
    public static void PlacePieceTest()
    {
        Debug.Log("=== 돌 놓기 테스트 시작 ===");
        
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager를 찾을 수 없습니다!");
            return;
        }
        
        // 보드가 초기화되지 않았으면 초기화
        if (boardManager.board == null)
        {
            Debug.Log("보드를 수동으로 초기화합니다...");
            boardManager.InitializeBoard();
            boardManager.SetupInitialPieces();
        }
        
        // 유효한 수 찾기
        var validMoves = boardManager.GetValidMoves();
        if (validMoves.Count > 0)
        {
            Vector2Int move = validMoves[0];
            Debug.Log($"첫 번째 유효한 수: ({move.x}, {move.y})");
            
            // 돌 놓기 시도
            bool success = boardManager.TryPlacePiece(move.x, move.y);
            if (success)
            {
                Debug.Log($"돌 놓기 성공: ({move.x}, {move.y})");
            }
            else
            {
                Debug.LogError($"돌 놓기 실패: ({move.x}, {move.y})");
            }
        }
        else
        {
            Debug.LogWarning("유효한 수가 없습니다!");
        }
    }
    
    [MenuItem("Tools/Board Test/Check Board State")]
    public static void CheckBoardState()
    {
        Debug.Log("=== 보드 상태 확인 ===");
        
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager를 찾을 수 없습니다!");
            return;
        }
        
        // 보드가 초기화되지 않았으면 초기화
        if (boardManager.board == null)
        {
            Debug.Log("보드를 수동으로 초기화합니다...");
            boardManager.InitializeBoard();
            boardManager.SetupInitialPieces();
        }
        
        // 보드 상태 출력
        for (int y = 7; y >= 0; y--)
        {
            string row = "";
            for (int x = 0; x < 8; x++)
            {
                if (boardManager.board[x, y] != null)
                {
                    if (boardManager.board[x, y].HasPiece())
                    {
                        if (boardManager.board[x, y].IsBlack())
                        {
                            row += "● "; // 검은 돌
                        }
                        else
                        {
                            row += "○ "; // 흰 돌
                        }
                    }
                    else
                    {
                        row += "□ "; // 빈 칸
                    }
                }
                else
                {
                    row += "? "; // null
                }
            }
            Debug.Log($"Row {y}: {row}");
        }
        
        // 점수 확인
        Debug.Log($"검은 돌: {boardManager.GetBlackScore()}, 흰 돌: {boardManager.GetWhiteScore()}");
    }
    
    private static void TestInitialPiecePlacement(BoardManager boardManager)
    {
        Debug.Log("=== 초기 돌 배치 테스트 ===");
        
        // 중앙 4개 위치 확인
        int center = 4; // 8x8 보드의 중앙
        Vector2Int[] initialPositions = {
            new Vector2Int(center - 1, center - 1), // (3,3)
            new Vector2Int(center, center),         // (4,4)
            new Vector2Int(center - 1, center),     // (3,4)
            new Vector2Int(center, center - 1)      // (4,3)
        };
        
        foreach (var pos in initialPositions)
        {
            if (boardManager.board[pos.x, pos.y] != null)
            {
                if (boardManager.board[pos.x, pos.y].HasPiece())
                {
                    string color = boardManager.board[pos.x, pos.y].IsBlack() ? "검은" : "흰";
                    Debug.Log($"초기 돌 확인: ({pos.x}, {pos.y}) - {color} 돌");
                }
                else
                {
                    Debug.LogWarning($"초기 돌 없음: ({pos.x}, {pos.y})");
                }
            }
            else
            {
                Debug.LogError($"보드 null: ({pos.x}, {pos.y})");
            }
        }
    }
    
    [MenuItem("Tools/Board Test/Run Full Test")]
    public static void RunFullTest()
    {
        Debug.Log("=== 전체 보드 테스트 시작 ===");
        
        InitializeBoardTest();
        CheckBoardState();
        PlacePieceTest();
        
        Debug.Log("=== 전체 테스트 완료 ===");
    }
} 