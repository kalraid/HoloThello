using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 궁극기: 상대 턴 넘기기
    public void Ultimate_SkipOpponentTurn(GameManager gameManager)
    {
        if (gameManager != null)
        {
            gameManager.SkipOpponentTurn();
        }
    }

    // 궁극기: 돌 하나 삭제 후 그 자리에 놓기
    public void Ultimate_RemoveAndPlace(GameManager gameManager, BoardManager boardManager)
    {
        if (gameManager != null && boardManager != null)
        {
            // 랜덤으로 돌이 놓인 위치를 하나 찾는다
            for (int attempt = 0; attempt < 100; attempt++)
            {
                int x = Random.Range(0, boardManager.boardSize);
                int y = Random.Range(0, boardManager.boardSize);
                if (boardManager.board[x, y].HasPiece())
                {
                    boardManager.board[x, y].SetDisc(false, null); // 돌 삭제
                    boardManager.TryPlacePiece(x, y); // 그 자리에 새 돌 놓기(현재 턴)
                    break;
                }
            }
        }
    }
} 