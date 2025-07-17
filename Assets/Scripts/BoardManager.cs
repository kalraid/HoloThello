using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject discPrefab;
    public Sprite blackSprite;
    public Sprite whiteSprite;
    public int[,] board = new int[8,8]; // 0:빈칸, 1:흑, 2:백
    public Disc[,] discs = new Disc[8,8];

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

    public void FlipDisc(int x, int y, int color)
    {
        if (discs[x, y] != null)
            discs[x, y].SetColor(color, blackSprite, whiteSprite);
        board[x, y] = color;
    }
} 