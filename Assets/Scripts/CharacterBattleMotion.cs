using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterBattleMotion : MonoBehaviour
{
    [Header("캐릭터 참조")]
    public Image playerCharacterImage;
    public Image cpuCharacterImage;
    
    [Header("위치 설정")]
    public Transform playerStartPosition;
    public Transform cpuStartPosition;
    public Transform playerCenterPosition;
    public Transform cpuCenterPosition;
    public Transform playerFarPosition;
    public Transform cpuFarPosition;
    
    [Header("모션 설정")]
    public float moveSpeed = 2f;
    public float shakeIntensity = 5f;
    public float shakeDuration = 0.3f;
    public float flashDuration = 0.1f;
    
    [Header("데미지 모션")]
    public float damageShakeIntensity = 8f;
    public float damageShakeDuration = 0.5f;
    public float damageFlashDuration = 0.15f;
    
    private Vector3 playerOriginalPosition;
    private Vector3 cpuOriginalPosition;
    
    private BoardManager boardManager;
    private GameManager gameManager;
    
    void Start()
    {
        // 참조 찾기
        if (boardManager == null)
            boardManager = FindFirstObjectByType<BoardManager>();
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        // 초기 위치 저장
        if (playerCharacterImage != null)
            playerOriginalPosition = playerCharacterImage.transform.position;
        if (cpuCharacterImage != null)
            cpuOriginalPosition = cpuCharacterImage.transform.position;
    }
    
    void Update()
    {
        // 오셀로 돌 수에 따른 캐릭터 위치 조정
        UpdateCharacterPositions();
    }
    
    /// <summary>
    /// 오셀로 돌 수에 따라 캐릭터 위치를 동적으로 조정
    /// </summary>
    void UpdateCharacterPositions()
    {
        if (boardManager == null) return;
        
        int blackCount = boardManager.GetBlackScore();
        int whiteCount = boardManager.GetWhiteScore();
        
        // 1P(흑) 캐릭터 위치 조정
        if (playerCharacterImage != null)
        {
            Vector3 targetPosition = GetCharacterPosition(blackCount, whiteCount, true);
            MoveCharacterToPosition(playerCharacterImage.transform, targetPosition);
        }
        
        // 2P/CPU(백) 캐릭터 위치 조정
        if (cpuCharacterImage != null)
        {
            Vector3 targetPosition = GetCharacterPosition(whiteCount, blackCount, false);
            MoveCharacterToPosition(cpuCharacterImage.transform, targetPosition);
        }
    }
    
    /// <summary>
    /// 돌 수에 따른 캐릭터 목표 위치 계산
    /// </summary>
    Vector3 GetCharacterPosition(int myCount, int enemyCount, bool isPlayer)
    {
        int difference = myCount - enemyCount;
        
        if (difference >= 10)
        {
            // 압도적 우위: 상대에게 가까이
            return isPlayer ? playerCenterPosition.position : cpuCenterPosition.position;
        }
        else if (difference >= 5)
        {
            // 우위: 중간 위치
            return isPlayer ? playerCenterPosition.position : cpuCenterPosition.position;
        }
        else if (difference <= -10)
        {
            // 압도적 열위: 반대편으로 도망
            return isPlayer ? playerFarPosition.position : cpuFarPosition.position;
        }
        else if (difference <= -5)
        {
            // 열위: 뒤로 물러남
            return isPlayer ? playerFarPosition.position : cpuFarPosition.position;
        }
        else
        {
            // 비등: 기본 위치
            return isPlayer ? playerStartPosition.position : cpuStartPosition.position;
        }
    }
    
    /// <summary>
    /// 캐릭터를 부드럽게 목표 위치로 이동
    /// </summary>
    void MoveCharacterToPosition(Transform character, Vector3 targetPosition)
    {
        if (character == null) return;
        
        float distance = Vector3.Distance(character.position, targetPosition);
        if (distance > 0.1f)
        {
            character.position = Vector3.Lerp(character.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 오셀로 돌이 뒤집힐 때 캐릭터 반응
    /// </summary>
    public void OnDiscFlipped(int x, int y, bool isBlack, int damage)
    {
        if (isBlack)
        {
            // 1P(흑) 돌이 뒤집힘
            StartCoroutine(PlayerReactionSequence(damage));
        }
        else
        {
            // 2P/CPU(백) 돌이 뒤집힘
            StartCoroutine(CPUReactionSequence(damage));
        }
    }
    
    /// <summary>
    /// 1P 캐릭터 반응 시퀀스
    /// </summary>
    IEnumerator PlayerReactionSequence(int damage)
    {
        if (playerCharacterImage == null) yield break;
        
        // 데미지에 따른 모션 강도 조절
        float shakeIntensity = damage >= 5 ? this.damageShakeIntensity : this.shakeIntensity;
        float shakeDuration = damage >= 5 ? this.damageShakeDuration : this.shakeDuration;
        float flashDuration = damage >= 5 ? this.damageFlashDuration : this.flashDuration;
        
        // 깜빡임 효과
        yield return StartCoroutine(FlashCharacter(playerCharacterImage, flashDuration));
        
        // 흔들림 효과
        yield return StartCoroutine(ShakeCharacter(playerCharacterImage, shakeIntensity, shakeDuration));
        
        // 데미지가 클 때 특별 모션
        if (damage >= 10)
        {
            yield return StartCoroutine(SpecialDamageMotion(playerCharacterImage, true));
        }
    }
    
    /// <summary>
    /// 2P/CPU 캐릭터 반응 시퀀스
    /// </summary>
    IEnumerator CPUReactionSequence(int damage)
    {
        if (cpuCharacterImage == null) yield break;
        
        // 데미지에 따른 모션 강도 조절
        float shakeIntensity = damage >= 5 ? this.damageShakeIntensity : this.shakeIntensity;
        float shakeDuration = damage >= 5 ? this.damageShakeDuration : this.shakeDuration;
        float flashDuration = damage >= 5 ? this.damageFlashDuration : this.flashDuration;
        
        // 깜빡임 효과
        yield return StartCoroutine(FlashCharacter(cpuCharacterImage, flashDuration));
        
        // 흔들림 효과
        yield return StartCoroutine(ShakeCharacter(cpuCharacterImage, shakeIntensity, shakeDuration));
        
        // 데미지가 클 때 특별 모션
        if (damage >= 10)
        {
            yield return StartCoroutine(SpecialDamageMotion(cpuCharacterImage, false));
        }
    }
    
    /// <summary>
    /// 캐릭터 깜빡임 효과
    /// </summary>
    IEnumerator FlashCharacter(Image character, float duration)
    {
        Color originalColor = character.color;
        Color flashColor = Color.red;
        
        // 빨간색으로 깜빡임
        character.color = flashColor;
        yield return new WaitForSeconds(duration);
        
        // 원래 색상으로 복원
        character.color = originalColor;
    }
    
    /// <summary>
    /// 캐릭터 흔들림 효과
    /// </summary>
    IEnumerator ShakeCharacter(Image character, float intensity, float duration)
    {
        Vector3 originalPosition = character.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            
            character.transform.position = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 원래 위치로 복원
        character.transform.position = originalPosition;
    }
    
    /// <summary>
    /// 특별 데미지 모션 (콤보 효과)
    /// </summary>
    IEnumerator SpecialDamageMotion(Image character, bool isPlayer)
    {
        Vector3 originalScale = character.transform.localScale;
        Vector3 originalPosition = character.transform.position;
        
        // 확대 효과
        character.transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        
        // 회전 효과
        float rotationTime = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < rotationTime)
        {
            float rotation = Mathf.Lerp(0f, 360f, elapsed / rotationTime);
            character.transform.rotation = Quaternion.Euler(0, 0, rotation);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 원래 상태로 복원
        character.transform.localScale = originalScale;
        character.transform.rotation = Quaternion.identity;
        character.transform.position = originalPosition;
    }
    
    /// <summary>
    /// 체력바 감소 시 캐릭터 모션
    /// </summary>
    public void OnHealthDecreased(bool isPlayer, int damage, int currentHP, int maxHP)
    {
        if (isPlayer)
        {
            StartCoroutine(PlayerHealthMotion(damage, currentHP, maxHP));
        }
        else
        {
            StartCoroutine(CPUHealthMotion(damage, currentHP, maxHP));
        }
    }
    
    /// <summary>
    /// 1P 체력 감소 모션
    /// </summary>
    IEnumerator PlayerHealthMotion(int damage, int currentHP, int maxHP)
    {
        if (playerCharacterImage == null) yield break;
        
        // HP 비율에 따른 모션 강도
        float hpRatio = (float)currentHP / maxHP;
        float motionIntensity = 1f - hpRatio; // HP가 낮을수록 강한 모션
        
        // 데미지 크기에 따른 추가 강도
        float damageMultiplier = Mathf.Min(damage / 1000f, 2f); // 최대 2배
        
        float finalIntensity = shakeIntensity * motionIntensity * damageMultiplier;
        
        // 강한 흔들림
        yield return StartCoroutine(ShakeCharacter(playerCharacterImage, finalIntensity, damageShakeDuration));
        
        // HP가 매우 낮을 때 특별 모션
        if (hpRatio <= 0.2f)
        {
            yield return StartCoroutine(LowHealthMotion(playerCharacterImage, true));
        }
    }
    
    /// <summary>
    /// 2P/CPU 체력 감소 모션
    /// </summary>
    IEnumerator CPUHealthMotion(int damage, int currentHP, int maxHP)
    {
        if (cpuCharacterImage == null) yield break;
        
        // HP 비율에 따른 모션 강도
        float hpRatio = (float)currentHP / maxHP;
        float motionIntensity = 1f - hpRatio; // HP가 낮을수록 강한 모션
        
        // 데미지 크기에 따른 추가 강도
        float damageMultiplier = Mathf.Min(damage / 1000f, 2f); // 최대 2배
        
        float finalIntensity = shakeIntensity * motionIntensity * damageMultiplier;
        
        // 강한 흔들림
        yield return StartCoroutine(ShakeCharacter(cpuCharacterImage, finalIntensity, damageShakeDuration));
        
        // HP가 매우 낮을 때 특별 모션
        if (hpRatio <= 0.2f)
        {
            yield return StartCoroutine(LowHealthMotion(cpuCharacterImage, false));
        }
    }
    
    /// <summary>
    /// 낮은 HP 상태 특별 모션
    /// </summary>
    IEnumerator LowHealthMotion(Image character, bool isPlayer)
    {
        Vector3 originalPosition = character.transform.position;
        Color originalColor = character.color;
        
        // 빨간색으로 변하면서 깜빡임
        for (int i = 0; i < 3; i++)
        {
            character.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            character.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        
        // 위아래로 흔들림
        float shakeTime = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < shakeTime)
        {
            float y = Mathf.Sin(elapsed * 20f) * 10f;
            character.transform.position = originalPosition + new Vector3(0, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 원래 상태로 복원
        character.transform.position = originalPosition;
        character.color = originalColor;
    }
    
    /// <summary>
    /// 연속 데미지 시 콤보 모션
    /// </summary>
    public void OnComboDamage(bool isPlayer, int comboCount)
    {
        if (isPlayer)
        {
            StartCoroutine(ComboMotion(playerCharacterImage, comboCount, true));
        }
        else
        {
            StartCoroutine(ComboMotion(cpuCharacterImage, comboCount, false));
        }
    }
    
    /// <summary>
    /// 콤보 모션
    /// </summary>
    IEnumerator ComboMotion(Image character, int comboCount, bool isPlayer)
    {
        if (character == null) yield break;
        
        Vector3 originalScale = character.transform.localScale;
        Vector3 originalPosition = character.transform.position;
        
        // 콤보 수에 따른 강도
        float comboIntensity = Mathf.Min(comboCount * 0.5f, 3f);
        
        // 확대 및 점프 효과
        character.transform.localScale = originalScale * (1f + comboIntensity * 0.1f);
        character.transform.position = originalPosition + Vector3.up * (comboIntensity * 5f);
        
        yield return new WaitForSeconds(0.2f);
        
        // 원래 상태로 복원
        character.transform.localScale = originalScale;
        character.transform.position = originalPosition;
    }
}
