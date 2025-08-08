using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // ObjectPool 확장을 위해 추가

// [ExecuteInEditMode] // 제거
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
    
    [Header("오브젝트 풀 설정")]
    public List<PoolableObject> objectPools; // 인스펙터에서 설정할 풀 목록
    
    // 테스트용 skillButtonPrefab 필드 추가
    public GameObject skillButtonPrefab;

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
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }
        InitializeObjectPools();
    }
    
    #region Initialization

    bool ValidateComponents()
    {
        if (damageTextPrefab == null || effectCanvas == null || player1Animator == null || cpuAnimator == null)
        {
            Debug.LogError("EffectManager의 필수 컴포넌트 중 일부가 연결되지 않았습니다!");
            return false;
        }
        return true;
    }

    void InitializeObjectPools()
    {
        if (ObjectPool.Instance != null && objectPools != null)
        {
            foreach (var pool in objectPools)
            {
                ObjectPool.Instance.AddPool(pool.tag, pool.prefab, pool.size);
            }
        }
    }

    #endregion

    #region Effect Spawning Methods

    // 모든 이펙트 생성 메서드에서 ObjectPool을 사용하도록 통합
    public void ShowDamageEffect(Vector3 position, int damage, bool isCritical = false)
    {
        GameObject effectObject = GetPooledObject("DamageText", position);
        if (effectObject == null) return;

        Text textComponent = effectObject.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = damage.ToString();
            
            // 데미지 크기에 따른 색상과 크기 설정
            if (damage >= 100)
            {
                textComponent.color = Color.red;
                textComponent.fontSize = 32;
                textComponent.fontStyle = FontStyle.Bold;
            }
            else if (damage >= 50)
            {
                textComponent.color = Color.yellow;
                textComponent.fontSize = 28;
                textComponent.fontStyle = FontStyle.Bold;
            }
            else if (damage >= 20)
            {
                textComponent.color = Color.orange;
                textComponent.fontSize = 24;
            }
            else
            {
                textComponent.color = Color.white;
                textComponent.fontSize = 18;
            }
            
            // 크리티컬 효과
            if (isCritical)
            {
                textComponent.color = Color.red;
                textComponent.fontSize += 8;
                textComponent.fontStyle = FontStyle.Bold;
            }
        }
        
        // 애니메이션 타입을 크리티컬 여부에 따라 결정
        AnimationType animType = isCritical ? AnimationType.ScaleUpDown : AnimationType.MoveUpFadeOut;
        StartCoroutine(AnimateAndReturn(effectObject, "DamageText", 1.5f, animType));
    }
    
    public void ShowSkillEffect(Vector3 position, string skillName)
    {
        GameObject effectObject = GetPooledObject("SkillEffect", position, skillEffectContainer);
        if (effectObject == null) return;
        
        Text skillText = effectObject.GetComponentInChildren<Text>();
        if (skillText != null)
        {
            skillText.text = skillName;
        }
        
        StartCoroutine(AnimateAndReturn(effectObject, "SkillEffect", 1f, AnimationType.ScaleUpDown));
    }

    public void ShowKOEffect()
    {
        GameObject effectObject = GetPooledObject("KOEffect", Vector3.zero, gameEndEffectContainer);
        if (effectObject == null) return;
        StartCoroutine(AnimateKOEffect(effectObject)); // KO는 독자적인 애니메이션 유지
    }

    public void ShowFinishEffect()
    {
        GameObject effectObject = GetPooledObject("FinishEffect", Vector3.zero, gameEndEffectContainer);
        if (effectObject == null) return;
        StartCoroutine(AnimateFinishEffect(effectObject)); // Finish도 독자적인 애니메이션 유지
    }

    private GameObject GetPooledObject(string tag, Vector3 position, Transform parent = null)
    {
        if (ObjectPool.Instance == null) return null;
        
        GameObject obj = ObjectPool.Instance.SpawnFromPool(tag, position, Quaternion.identity);
        if (obj != null && parent != null)
        {
            obj.transform.SetParent(parent, false);
        }
        return obj;
    }

    #endregion

    #region Animation Coroutines

    // 범용 애니메이션 및 반환 코루틴
    private enum AnimationType { MoveUpFadeOut, ScaleUpDown }

    IEnumerator AnimateAndReturn(GameObject obj, string tag, float duration, AnimationType animType = AnimationType.MoveUpFadeOut)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 startScale = obj.transform.localScale;
        Text text = obj.GetComponent<Text>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            switch(animType)
            {
                case AnimationType.MoveUpFadeOut:
                    obj.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 50f, t);
                    if (text != null) text.color = new Color(text.color.r, text.color.g, text.color.b, 1f - t);
                    break;
                case AnimationType.ScaleUpDown:
                    obj.transform.localScale = Vector3.Lerp(startScale, startScale * 1.5f, Mathf.PingPong(t * 2, 1));
                    break;
            }
            yield return null;
        }

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool(tag, obj);
        }
        else
        {
            Destroy(obj);
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
        else
        {
            Debug.LogWarning( (isPlayer1 ? "Player1" : "CPU") + "의 Animator가 연결되지 않았습니다.");
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

    public void ShowSpecialEffect(int level, bool isPlayer1, int damage, int hp, int maxHp)
    {
        // 1. 파티클/이펙트
        for (int i = 0; i < level; i++)
        {
            if (skillEffectPrefab != null && effectCanvas != null)
            {
                Vector3 pos = isPlayer1 ? player1Animator.transform.position : cpuAnimator.transform.position;
                GameObject fx = Instantiate(skillEffectPrefab, pos, Quaternion.identity, effectCanvas);
                float scale = 1f + 0.3f * i;
                fx.transform.localScale = Vector3.one * scale;
                Destroy(fx, 1.5f);
            }
        }
        // 2. 화면 흔들림
        if (level >= 2)
        {
            CameraShake(level);
        }
        // 3. 사운드 볼륨 증가
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySkillUse(level);
        }
        // 4. HP바 진동/깜빡임
        Slider hpBar = isPlayer1 ? player1Animator.GetComponentInChildren<Slider>() : cpuAnimator.GetComponentInChildren<Slider>();
        if (hpBar != null)
        {
            StartCoroutine(FlashHealthBarSpecial(hpBar, level));
        }
        // 5. 데미지 텍스트 강조
        Vector3 hpBarPos = hpBar != null ? hpBar.transform.position : Vector3.zero;
        ShowDamageEffectSpecial(hpBarPos, damage, level, hp, maxHp);
    }

    private void CameraShake(int level)
    {
        // 간단한 화면 흔들림 (진폭, 시간 증가)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            StartCoroutine(CameraShakeCoroutine(mainCam, 0.1f * level, 0.15f + 0.05f * level));
        }
    }
    private IEnumerator CameraShakeCoroutine(Camera cam, float magnitude, float duration)
    {
        Vector3 origPos = cam.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cam.transform.position = origPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = origPos;
    }
    private IEnumerator FlashHealthBarSpecial(Slider healthBar, int level)
    {
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        Color origColor = fillImage.color;
        for (int i = 0; i < 2 + level; i++)
        {
            fillImage.color = Color.yellow;
            yield return new WaitForSeconds(0.08f);
            fillImage.color = origColor;
            yield return new WaitForSeconds(0.08f);
        }
    }
    public void ShowDamageEffectSpecial(Vector3 position, int damage, int level, int hp, int maxHp)
    {
        GameObject effectObject = GetPooledObject("DamageText", position);
        if (effectObject == null) return;
        Text textComponent = effectObject.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = $"{damage} {(level > 0 ? "CRITICAL!" : "")}\nHP: {hp}/{maxHp}";
            textComponent.color = level >= 2 ? Color.red : (level == 1 ? new Color(1f,0.5f,0f) : Color.white);
            textComponent.fontSize = 18 + 6 * level;
        }
        StartCoroutine(AnimateAndReturn(effectObject, "DamageText", 1.2f + 0.2f * level));
    }

    #endregion
}

[System.Serializable]
public class PoolableObject
{
    public string tag;
    public GameObject prefab;
    public int size;
} 