using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public Text turnText;
    public Text resultText;
    public int currentPlayer = 1; // 1:흑, 2:백

    void Start()
    {
        boardManager.InitBoard();
        currentPlayer = 1;
        UpdateTurnText();
        resultText.text = "";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && resultText.text == "")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            if (IsValidMove(x, y, currentPlayer))
            {
                PlaceAndFlip(x, y, currentPlayer);
                currentPlayer = 3 - currentPlayer; // 1<->2
                UpdateTurnText();

                if (!HasValidMove(currentPlayer))
                {
                    currentPlayer = 3 - currentPlayer;
                    if (!HasValidMove(currentPlayer))
                        EndGame();
                }
            }
        }
    }

    void UpdateTurnText()
    {
        turnText.text = (currentPlayer == 1) ? "흑 차례" : "백 차례";
    }

    void EndGame()
    {
        int black = 0, white = 0;
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            if (boardManager.board[x, y] == 1) black++;
            if (boardManager.board[x, y] == 2) white++;
        }
        if (black > white) resultText.text = $"흑 승! ({black}:{white})";
        else if (white > black) resultText.text = $"백 승! ({black}:{white})";
        else resultText.text = $"무승부! ({black}:{white})";
    }

    bool IsValidMove(int x, int y, int color)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) return false;
        if (boardManager.board[x, y] != 0) return false;
        return GetFlippableDiscs(x, y, color).Count > 0;
    }

    bool HasValidMove(int color)
    {
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
            if (IsValidMove(x, y, color))
                return true;
        return false;
    }

    void PlaceAndFlip(int x, int y, int color)
    {
        boardManager.PlaceDisc(x, y, color);
        var toFlip = GetFlippableDiscs(x, y, color);
        foreach (var pos in toFlip)
            boardManager.FlipDisc(pos.x, pos.y, color);
    }

    List<Vector2Int> GetFlippableDiscs(int x, int y, int color)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int[] dx = {-1,0,1,-1,1,-1,0,1};
        int[] dy = {-1,-1,-1,0,0,1,1,1};
        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir], ny = y + dy[dir];
            List<Vector2Int> temp = new List<Vector2Int>();
            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && boardManager.board[nx, ny] == 3 - color)
            {
                temp.Add(new Vector2Int(nx, ny));
                nx += dx[dir]; ny += dy[dir];
            }
            if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && boardManager.board[nx, ny] == color && temp.Count > 0)
                result.AddRange(temp);
        }
        return result;
    }
} 