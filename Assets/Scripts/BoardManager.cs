using UnityEngine;
using System.Collections.Generic; // Added for List

public class BoardManager : MonoBehaviour
{
    public GameObject discPrefab;
    public Sprite blackSprite;
    public Sprite whiteSprite;
    public int[,] board = new int[8,8]; // 0:빈칸, 1:흑, 2:백
    public Disc[,] discs = new Disc[8,8];

    // 하이라이트용 임시 디스크 배열
    private Disc[,] highlightDiscs = new Disc[8,8];

    public void InitBoard()
    {
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            board[x, y] = 0;
            if (discs[x, y] != null)
                GameObject.Destroy(discs[x, y].gameObject);
            discs[x, y] = null;
        }

        board[3,3] = 2; // 백
        board[3,4] = 1; // 흑
        board[4,3] = 1; // 흑
        board[4,4] = 2; // 백

        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            if (board[x, y] != 0)
                PlaceDisc(x, y, board[x, y]);
        }
    }

    public void PlaceDisc(int x, int y, int color)
    {
        GameObject obj = GameObject.Instantiate(discPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        Disc disc = obj.GetComponent<Disc>();
        disc.SetColor(color, blackSprite, whiteSprite);
        discs[x, y] = disc;
        board[x, y] = color;
    }

    // 캐릭터별 돌 스프라이트 적용
    public void PlaceDiscWithSprite(int x, int y, int color, Sprite discSprite)
    {
        GameObject obj = GameObject.Instantiate(discPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        Disc disc = obj.GetComponent<Disc>();
        disc.SetColor(color, blackSprite, whiteSprite);
        obj.GetComponent<SpriteRenderer>().sprite = discSprite;
        discs[x, y] = disc;
        board[x, y] = color;
    }

    public void FlipDisc(int x, int y, int color)
    {
        if (discs[x, y] != null)
            discs[x, y].SetColor(color, blackSprite, whiteSprite);
        board[x, y] = color;
    }

    // 오셀로 규칙 관련 보조 메서드 (GameManager에서 호출)
    // (실제 뒤집기, 유효성 검사는 GameManager에서 처리)
    // 필요시 여기에 추가적인 보조 메서드 구현 가능
    
    // 예시: 해당 위치가 보드 범위 내인지 확인
    public bool IsInBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    // 빈 칸에 하이라이트 디스크 생성
    public void ShowHighlights(List<Vector2Int> positions, Sprite highlightSprite)
    {
        ClearHighlights();
        foreach (var pos in positions)
        {
            if (highlightDiscs[pos.x, pos.y] == null)
            {
                GameObject obj = GameObject.Instantiate(discPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
                Disc disc = obj.GetComponent<Disc>();
                disc.SetHighlight(true);
                obj.GetComponent<SpriteRenderer>().sprite = highlightSprite;
                highlightDiscs[pos.x, pos.y] = disc;
            }
        }
    }

    // 모든 하이라이트 디스크 제거
    public void ClearHighlights()
    {
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            if (highlightDiscs[x, y] != null)
            {
                GameObject.Destroy(highlightDiscs[x, y].gameObject);
                highlightDiscs[x, y] = null;
            }
        }
    }
} 