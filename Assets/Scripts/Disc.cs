using UnityEngine;

public class Disc : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool hasPiece = false;
    private bool isBlack = true;
    private int x, y;
    private BoardManager boardManager;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Initialize(int x, int y, BoardManager boardManager)
    {
        this.x = x;
        this.y = y;
        this.boardManager = boardManager;
        
        // 초기 상태: 빈 칸
        spriteRenderer.sprite = null;
        hasPiece = false;
    }
    
    public void SetPiece(bool isBlack)
    {
        this.isBlack = isBlack;
        hasPiece = true;
        
        // 캐릭터 이미지 설정
        CharacterData selectedChar = GameData.Instance.selectedCharacter1P;
        if (isBlack)
        {
            selectedChar = GameData.Instance.selectedCharacter1P;
        }
        else
        {
            selectedChar = GameData.Instance.selectedCharacterCPU;
        }
        
        if (selectedChar != null && selectedChar.discSprite != null)
        {
            spriteRenderer.sprite = selectedChar.discSprite;
        }
        else
        {
            // 기본 색상으로 설정
            spriteRenderer.color = isBlack ? Color.black : Color.white;
        }
    }
    
    public void Flip()
    {
        isBlack = !isBlack;
        
        // 캐릭터 이미지 업데이트
        CharacterData selectedChar = GameData.Instance.selectedCharacter1P;
        if (isBlack)
        {
            selectedChar = GameData.Instance.selectedCharacter1P;
        }
        else
        {
            selectedChar = GameData.Instance.selectedCharacterCPU;
        }
        
        if (selectedChar != null && selectedChar.discSprite != null)
        {
            spriteRenderer.sprite = selectedChar.discSprite;
        }
        else
        {
            // 기본 색상으로 설정
            spriteRenderer.color = isBlack ? Color.black : Color.white;
        }
        
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
    
    void OnMouseDown()
    {
        // 게임이 끝났으면 클릭 무시
        if (boardManager.IsGameEnded()) return;
        
        // 게임 모드에 따른 클릭 처리
        GameMode currentMode = GameData.Instance.GetGameMode();
        
        switch (currentMode)
        {
            case GameMode.PlayerVsCPU:
                // 1P vs CPU 모드: 1P만 클릭 가능
                if (boardManager.IsBlackTurn())
                {
                    HandlePlayerClick();
                }
                break;
                
            case GameMode.PlayerVsPlayer:
                // 1P vs 2P 모드: 현재 턴에 맞는 플레이어만 클릭 가능
                HandlePlayerClick();
                break;
                
            case GameMode.CPUVsCPU:
                // CPU vs CPU 모드: 클릭 무시
                break;
        }
    }
    
    void HandlePlayerClick()
    {
        // 현재 위치가 유효한 수인지 확인
        if (boardManager.IsValidMove(x, y, boardManager.IsBlackTurn()))
        {
            // 수를 둠
            boardManager.PlacePiece(x, y, boardManager.IsBlackTurn());
            
            // 클릭 사운드
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }
        else
        {
            // 유효하지 않은 수 클릭 시 피드백
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMessage("유효하지 않은 수입니다!", 1f);
            }
            
            // 에러 사운드
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    }
    
    void OnMouseEnter()
    {
        if (!hasPiece && boardManager != null && !boardManager.IsGameEnded())
        {
            // 유효한 수인지 확인하고 하이라이트
            if (boardManager.IsValidMove(x, y, boardManager.IsBlackTurn()))
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
    
    void OnMouseExit()
    {
        if (!hasPiece)
        {
            spriteRenderer.color = Color.clear;
        }
    }
} 