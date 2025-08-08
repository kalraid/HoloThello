using UnityEngine;

/// <summary>
/// 이펙트 관리자 인터페이스
/// 이펙트 및 애니메이션을 관리하는 모든 클래스가 구현해야 하는 인터페이스
/// </summary>
public interface IEffectManager
{
    /// <summary>
    /// 데미지 이펙트 표시
    /// </summary>
    /// <param name="position">표시 위치</param>
    /// <param name="damage">데미지량</param>
    /// <param name="isCritical">크리티컬 여부</param>
    void ShowDamageEffect(Vector3 position, int damage, bool isCritical = false);
    
    /// <summary>
    /// 스킬 이펙트 표시
    /// </summary>
    /// <param name="position">표시 위치</param>
    /// <param name="skillName">스킬 이름</param>
    void ShowSkillEffect(Vector3 position, string skillName);
    
    /// <summary>
    /// K.O 이펙트 표시
    /// </summary>
    void ShowKOEffect();
    
    /// <summary>
    /// FINISH 이펙트 표시
    /// </summary>
    void ShowFinishEffect();
    
    /// <summary>
    /// 캐릭터 애니메이션 재생
    /// </summary>
    /// <param name="isPlayer1">플레이어1 여부</param>
    /// <param name="animationName">애니메이션 이름</param>
    void PlayCharacterAnimation(bool isPlayer1, string animationName);
    
    /// <summary>
    /// HP 바 깜빡임 효과
    /// </summary>
    /// <param name="healthBar">HP 바</param>
    void FlashHealthBar(UnityEngine.UI.Slider healthBar);
    
    /// <summary>
    /// 특별 이펙트 표시
    /// </summary>
    /// <param name="message">메시지</param>
    /// <param name="effectColor">이펙트 색상</param>
    void ShowSpecialEffect(string message, Color effectColor);
    
    /// <summary>
    /// 특별 이펙트 표시 (레벨 기반)
    /// </summary>
    /// <param name="level">이펙트 레벨</param>
    /// <param name="isPlayer1">플레이어1 여부</param>
    /// <param name="damage">데미지량</param>
    /// <param name="hp">현재 HP</param>
    /// <param name="maxHp">최대 HP</param>
    void ShowSpecialEffect(int level, bool isPlayer1, int damage, int hp, int maxHp);
    
    /// <summary>
    /// 특별 데미지 이펙트 표시
    /// </summary>
    /// <param name="position">표시 위치</param>
    /// <param name="damage">데미지량</param>
    /// <param name="level">이펙트 레벨</param>
    /// <param name="hp">현재 HP</param>
    /// <param name="maxHp">최대 HP</param>
    void ShowDamageEffectSpecial(Vector3 position, int damage, int level, int hp, int maxHp);
    
    /// <summary>
    /// 카메라 흔들기 효과
    /// </summary>
    /// <param name="level">흔들기 강도</param>
    void CameraShake(int level);
    
    /// <summary>
    /// 이펙트 초기화
    /// </summary>
    void InitializeEffects();
    
    /// <summary>
    /// 모든 이펙트 정리
    /// </summary>
    void ClearAllEffects();
} 