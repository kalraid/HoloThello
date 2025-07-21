using UnityEngine;
using System.Collections;
// [ExecuteInEditMode] // 제거
public class Disc : MonoBehaviour
{
    private SpriteRenderer mainRenderer;
    private SpriteRenderer miniRenderer;
    private bool hasPiece = false;
    private bool isBlack = true;
    // x, y 좌표는 public으로 변경하여 GameManager에서 접근 가능하게 함
    public int x, y;
    public int X;
    public int Y;
    private BoardManager boardManager;
    
    void Awake()
    {
        // mainRenderer, miniRenderer 자동 할당
        mainRenderer = GetComponent<SpriteRenderer>();
        // 자식 오브젝트 이름 오타 수정: MiniImage -> Minilmage
        Transform mini = transform.Find("Minilmage");
        if (mini != null) miniRenderer = mini.GetComponent<SpriteRenderer>();
        if (mainRenderer == null)
            Debug.LogError($"{gameObject.name}의 mainRenderer가 null입니다! (Awake)");
    }
    
    public void Initialize(int x, int y, BoardManager boardManager)
    {
        this.x = x;
        this.y = y;
        this.boardManager = boardManager;
        
        // 초기 상태: 빈 칸
        hasPiece = false;
    }
    
    // mainRenderer(흑/백), miniRenderer(캐릭터 미니이미지) 동시 할당 함수
    public void SetDisc(bool isBlack, Sprite miniSprite)
    {
        this.isBlack = isBlack;
        hasPiece = true;
        if (mainRenderer != null)
        {
            mainRenderer.color = isBlack ? Color.black : Color.white;
        }
        if (miniRenderer != null)
        {
            miniRenderer.sprite = miniSprite;
            miniRenderer.color = Color.white;
        }
    }
    
    public void SetPiece(bool isBlack)
    {
        // 캐릭터 미니이미지 결정
        CharacterData selectedChar = isBlack ? GameData.Instance.selectedCharacter1P : GameData.Instance.selectedCharacterCPU;
        Sprite miniSprite = (selectedChar != null) ? selectedChar.discSprite : null;
        SetDisc(isBlack, miniSprite);
    }
    
    public void Flip()
    {
        isBlack = !isBlack;
        // 캐릭터 미니이미지 결정
        CharacterData selectedChar = isBlack ? GameData.Instance.selectedCharacter1P : GameData.Instance.selectedCharacterCPU;
        Sprite miniSprite = (selectedChar != null) ? selectedChar.discSprite : null;
        SetDisc(isBlack, miniSprite);
        
        // 뒤집기 애니메이션 (간단한 스케일 효과)
        StartCoroutine(FlipAnimation());
    }
    
    private System.Collections.IEnumerator FlipAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 0.5f;
        
        // 축소
        float duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // 확대
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    public bool HasPiece()
    {
        if (mainRenderer == null)
        {
            Debug.LogError($"{gameObject.name}의 mainRenderer가 null입니다! (HasPiece)");
            return false;
        }
        return hasPiece;
    }
    
    public bool IsBlack()
    {
        return isBlack;
    }
    
    // OnMouseDown, HandlePlayerClick, OnMouseEnter, OnMouseExit 메서드 전체 삭제
    // (입력 처리는 GameManager에서 중앙 관리)

    public void AnimatePlace()
    {
        StartCoroutine(AnimatePlaceCoroutine());
    }

    private IEnumerator AnimatePlaceCoroutine()
    {
        float duration = 0.15f;
        float maxScale = 1.2f;
        float t = 0f;
        Vector3 start = Vector3.one;
        Vector3 end = Vector3.one * maxScale;
        // 커졌다가
        while (t < duration / 2f)
        {
            float p = t / (duration / 2f);
            transform.localScale = Vector3.Lerp(start, end, p);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = end;
        // 다시 작아짐
        t = 0f;
        while (t < duration / 2f)
        {
            float p = t / (duration / 2f);
            transform.localScale = Vector3.Lerp(end, start, p);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = start;
    }

    public void AnimateFlip()
    {
        StartCoroutine(AnimateFlipCoroutine());
    }

    private IEnumerator AnimateFlipCoroutine()
    {
        float duration = 0.12f;
        float maxScale = 1.2f;
        float t = 0f;
        Vector3 start = Vector3.one;
        Vector3 end = Vector3.one * maxScale;
        Color origColor = mainRenderer.color;
        Color bright = origColor * 1.3f;
        // 밝아지고 커짐
        while (t < duration / 2f)
        {
            float p = t / (duration / 2f);
            transform.localScale = Vector3.Lerp(start, end, p);
            mainRenderer.color = Color.Lerp(origColor, bright, p);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = end;
        mainRenderer.color = bright;
        // 다시 작아지고 원래 색으로
        t = 0f;
        while (t < duration / 2f)
        {
            float p = t / (duration / 2f);
            transform.localScale = Vector3.Lerp(end, start, p);
            mainRenderer.color = Color.Lerp(bright, origColor, p);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = start;
        mainRenderer.color = origColor;
    }
} 