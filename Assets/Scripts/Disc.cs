using UnityEngine;

public class Disc : MonoBehaviour
{
    public int color; // 1:흑, 2:백

    public void SetColor(int color, Sprite black, Sprite white)
    {
        this.color = color;
        GetComponent<SpriteRenderer>().sprite = (color == 1) ? black : white;
    }
} 