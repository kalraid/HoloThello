using UnityEngine;
// [ExecuteInEditMode] // 제거
public class Disc : MonoBehaviour
{
    private SpriteRenderer mainRenderer;
    private SpriteRenderer miniRenderer;
    private bool hasPiece = false;
    private bool isBlack = true;
    // x, y 좌표는 public으로 변경하여 GameManager에서 접근 가능하게 함
    public int x, y;
    private BoardManager boardManager;
    
    void Awake()
    {
        // mainRenderer, miniRenderer 자동 할당
        mainRenderer = GetComponent<SpriteRenderer>();
        Transform mini = transform.Find("MiniImage");
        if (mini != null) miniRenderer = mini.GetComponent<SpriteRenderer>();
    }
    
    public void Initialize(int x, int y, BoardManager boardManager)
    {
        this.x = x;
        this.y = y;
        this.boardManager = boardManager;
        
        // 초기 상태: 빈 칸
        mainRenderer.sprite = null;
        hasPiece = false;
    }
    
    // mainRenderer(흑/백), miniRenderer(캐릭터 미니이미지) 동시 할당 함수
    public void SetDisc(bool isBlack, Sprite miniSprite)
    {
        this.isBlack = isBlack;
        hasPiece = true;
        if (mainRenderer != null)
        {
            mainRenderer.sprite = null;
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
        return hasPiece;
    }
    
    public bool IsBlack()
    {
        return isBlack;
    }
    
    // OnMouseDown, HandlePlayerClick, OnMouseEnter, OnMouseExit 메서드 전체 삭제
    // (입력 처리는 GameManager에서 중앙 관리)
} 