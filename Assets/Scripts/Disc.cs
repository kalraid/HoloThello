using UnityEngine;

public class Disc : MonoBehaviour
{
    public int color; // 1:흑, 2:백

    public void SetColor(int color, Sprite black, Sprite white)
    {
        this.color = color;
        GetComponent<SpriteRenderer>().sprite = (color == 1) ? black : white;
    }

    // 하이라이트 표시 (예: 색상 밝게)
    public void SetHighlight(bool highlight)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (highlight)
            sr.color = new Color(1f, 1f, 0.5f, 1f); // 노란빛 하이라이트
        else
            sr.color = Color.white;
    }

    // 돌 뒤집기 애니메이션 (간단한 Scale/Color 효과)
    public void PlayFlipEffect()
    {
        StartCoroutine(FlipAnim());
    }

    private System.Collections.IEnumerator FlipAnim()
    {
        float t = 0f;
        Vector3 origScale = transform.localScale;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 1.3f, t/0.2f);
            transform.localScale = origScale * s;
            yield return null;
        }
        t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1.3f, 1f, t/0.2f);
            transform.localScale = origScale * s;
            yield return null;
        }
        transform.localScale = origScale;
    }
} 