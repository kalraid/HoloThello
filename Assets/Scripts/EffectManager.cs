using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    
    [Header("UI 이펙트")]
    public GameObject damageTextPrefab;
    public Transform effectCanvas;
    
    [Header("스킬 이펙트")]
    public GameObject skillEffectPrefab;
    public Transform skillEffectContainer;
    
    [Header("게임 종료 이펙트")]
    public GameObject koEffectPrefab;
    public GameObject finishEffectPrefab;
    public Transform gameEndEffectContainer;
    
    [Header("캐릭터 애니메이션")]
    public Animator player1Animator;
    public Animator cpuAnimator;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 데미지 텍스트 이펙트
    public void ShowDamageEffect(Vector3 position, int damage, bool isCritical = false)
    {
        if (damageTextPrefab != null && effectCanvas != null)
        {
            GameObject damageText;
            
            // ObjectPool 사용
            if (ObjectPool.Instance != null)
            {
                damageText = ObjectPool.Instance.SpawnFromPool("DamageText", position, Quaternion.identity);
                if (damageText == null)
                {
                    // 풀에 없으면 직접 생성
                    damageText = Instantiate(damageTextPrefab, effectCanvas);
                }
            }
            else
            {
                damageText = Instantiate(damageTextPrefab, effectCanvas);
            }
            
            damageText.transform.position = position;
            
            Text textComponent = damageText.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = damage.ToString();
                textComponent.color = isCritical ? Color.red : Color.white;
                textComponent.fontSize = isCritical ? 24 : 18;
            }
            
            StartCoroutine(AnimateDamageText(damageText));
        }
    }
    
    IEnumerator AnimateDamageText(GameObject damageText)
    {
        Vector3 startPos = damageText.transform.position;
        Vector3 endPos = startPos + Vector3.up * 50f;
        
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            damageText.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // 페이드 아웃
            Text text = damageText.GetComponent<Text>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - t;
                text.color = color;
            }
            
            yield return null;
        }
        
        // ObjectPool로 반환
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool("DamageText", damageText);
        }
        else
        {
            Destroy(damageText);
        }
    }
    
    // 스킬 사용 이펙트
    public void ShowSkillEffect(Vector3 position, string skillName)
    {
        if (skillEffectPrefab != null && skillEffectContainer != null)
        {
            GameObject skillEffect;
            
            // ObjectPool 사용
            if (ObjectPool.Instance != null)
            {
                skillEffect = ObjectPool.Instance.SpawnFromPool("SkillEffect", position, Quaternion.identity);
                if (skillEffect == null)
                {
                    // 풀에 없으면 직접 생성
                    skillEffect = Instantiate(skillEffectPrefab, skillEffectContainer);
                }
            }
            else
            {
                skillEffect = Instantiate(skillEffectPrefab, skillEffectContainer);
            }
            
            skillEffect.transform.position = position;
            
            // 스킬 이름 텍스트 설정
            Text skillText = skillEffect.GetComponentInChildren<Text>();
            if (skillText != null)
            {
                skillText.text = skillName;
            }
            
            StartCoroutine(AnimateSkillEffect(skillEffect));
        }
    }
    
    IEnumerator AnimateSkillEffect(GameObject skillEffect)
    {
        // 스케일 애니메이션
        Vector3 originalScale = skillEffect.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (t < 0.5f)
            {
                // 확대
                skillEffect.transform.localScale = Vector3.Lerp(originalScale, targetScale, t * 2f);
            }
            else
            {
                // 축소
                skillEffect.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, (t - 0.5f) * 2f);
            }
            
            yield return null;
        }
        
        // ObjectPool로 반환
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool("SkillEffect", skillEffect);
        }
        else
        {
            Destroy(skillEffect);
        }
    }
    
    // K.O. 이펙트
    public void ShowKOEffect()
    {
        if (koEffectPrefab != null && gameEndEffectContainer != null)
        {
            GameObject koEffect = Instantiate(koEffectPrefab, gameEndEffectContainer);
            StartCoroutine(AnimateKOEffect(koEffect));
        }
    }
    
    IEnumerator AnimateKOEffect(GameObject koEffect)
    {
        // 초기 설정
        koEffect.transform.localScale = Vector3.zero;
        
        // 등장 애니메이션
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            koEffect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        
        // 통통 튀기 애니메이션
        Vector3 originalPos = koEffect.transform.localPosition;
        float bounceTimer = 0f;
        
        while (bounceTimer < 3f) // 3초간 통통 튀기
        {
            bounceTimer += Time.deltaTime;
            float bounce = Mathf.Abs(Mathf.Sin(bounceTimer * Mathf.PI / 1.5f)) * 30f;
            koEffect.transform.localPosition = originalPos + Vector3.up * bounce;
            yield return null;
        }
        
        Destroy(koEffect);
    }
    
    // FINISH 이펙트
    public void ShowFinishEffect()
    {
        if (finishEffectPrefab != null && gameEndEffectContainer != null)
        {
            GameObject finishEffect = Instantiate(finishEffectPrefab, gameEndEffectContainer);
            StartCoroutine(AnimateFinishEffect(finishEffect));
        }
    }
    
    IEnumerator AnimateFinishEffect(GameObject finishEffect)
    {
        // 초기 설정
        finishEffect.transform.localScale = Vector3.zero;
        
        // 등장 애니메이션
        float duration = 0.8f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            finishEffect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        
        // 2초간 유지
        yield return new WaitForSeconds(2f);
        
        // 페이드 아웃
        elapsed = 0f;
        duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            CanvasGroup canvasGroup = finishEffect.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - t;
            }
            
            yield return null;
        }
        
        Destroy(finishEffect);
    }
    
    // 캐릭터 애니메이션
    public void PlayCharacterAnimation(bool isPlayer1, string animationName)
    {
        Animator targetAnimator = isPlayer1 ? player1Animator : cpuAnimator;
        
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(animationName);
        }
    }
    
    // 체력바 반짝임 효과
    public void FlashHealthBar(Slider healthBar)
    {
        StartCoroutine(FlashHealthBarCoroutine(healthBar));
    }
    
    IEnumerator FlashHealthBarCoroutine(Slider healthBar)
    {
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        Color originalColor = fillImage.color;
        
        for (int i = 0; i < 3; i++)
        {
            fillImage.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            fillImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    // 특별 이펙트 (5배수 데미지 등)
    public void ShowSpecialEffect(string message, Color effectColor)
    {
        if (effectCanvas != null)
        {
            GameObject specialEffect = new GameObject("SpecialEffect");
            specialEffect.transform.SetParent(effectCanvas, false);
            
            Text effectText = specialEffect.AddComponent<Text>();
            effectText.text = message;
            effectText.color = effectColor;
            effectText.fontSize = 32;
            effectText.fontStyle = FontStyle.Bold;
            effectText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform rectTransform = specialEffect.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.2f, 0.4f);
            rectTransform.anchorMax = new Vector2(0.8f, 0.6f);
            rectTransform.sizeDelta = Vector2.zero;
            
            StartCoroutine(AnimateSpecialEffect(specialEffect));
        }
    }
    
    IEnumerator AnimateSpecialEffect(GameObject specialEffect)
    {
        // 초기 설정
        specialEffect.transform.localScale = Vector3.zero;
        
        // 등장 애니메이션
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            specialEffect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        
        // 2초간 유지
        yield return new WaitForSeconds(2f);
        
        // 페이드 아웃
        elapsed = 0f;
        duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            Text text = specialEffect.GetComponent<Text>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - t;
                text.color = color;
            }
            
            yield return null;
        }
        
        Destroy(specialEffect);
    }
} 