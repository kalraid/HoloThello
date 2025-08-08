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
        
        // SpriteRenderer가 없으면 추가
        if (mainRenderer == null)
        {
            mainRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log($"{gameObject.name}에 SpriteRenderer를 추가했습니다.");
        }
        
        // 자식 오브젝트 이름 수정: Minilmage -> MiniImage
        Transform mini = transform.Find("MiniImage");
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
        
        // mainRenderer가 null이면 강제로 초기화
        if (mainRenderer == null)
        {
            mainRenderer = GetComponent<SpriteRenderer>();
            if (mainRenderer == null)
            {
                mainRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log($"{gameObject.name}에 SpriteRenderer를 추가했습니다. (SetDisc)");
            }
        }
        
        if (mainRenderer != null)
        {
            // 색상을 더 명확하게 구분
            if (isBlack)
            {
                mainRenderer.color = Color.black; // 1P = 검정색
            }
            else
            {
                mainRenderer.color = Color.white; // 2P = 흰색
            }
            // 렌더링 순서 설정
            mainRenderer.sortingOrder = 3;
            // 스프라이트가 할당되어 있지 않으면 기본 원형 스프라이트 사용
            if (mainRenderer.sprite == null)
            {
                // 기본 원형 스프라이트 생성 또는 할당
                mainRenderer.sprite = CreateDefaultDiscSprite();
            }
        }
        if (miniRenderer != null)
        {
            miniRenderer.sprite = miniSprite;
            miniRenderer.color = Color.white;
            miniRenderer.sortingOrder = 4; // 미니 이미지는 더 위에 표시
        }
    }
    
    // 빈 칸으로 설정 (초록색)
    public void SetEmpty()
    {
        hasPiece = false;
        
        // mainRenderer가 null이면 강제로 초기화
        if (mainRenderer == null)
        {
            mainRenderer = GetComponent<SpriteRenderer>();
            if (mainRenderer == null)
            {
                mainRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log($"{gameObject.name}에 SpriteRenderer를 추가했습니다. (SetEmpty)");
            }
        }
        
        if (mainRenderer != null)
        {
            mainRenderer.color = Color.green; // 빈 칸 = 초록색
            mainRenderer.sortingOrder = 2;
        }
        if (miniRenderer != null)
        {
            miniRenderer.sprite = null;
        }
    }
    
    // 기본 원형 스프라이트 생성 (필요시)
    private Sprite CreateDefaultDiscSprite()
    {
        // 간단한 원형 텍스처 생성
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
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
        // mainRenderer가 null이면 강제로 초기화
        if (mainRenderer == null)
        {
            mainRenderer = GetComponent<SpriteRenderer>();
            if (mainRenderer == null)
            {
                mainRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log($"{gameObject.name}에 SpriteRenderer를 추가했습니다. (HasPiece)");
            }
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