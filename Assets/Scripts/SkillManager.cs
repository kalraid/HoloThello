using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    
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
    
    // 궁극기: 상대 턴 넘기기
    public void Ultimate_SkipOpponentTurn(GameManager gameManager)
    {
        Debug.Log("궁극기: 상대 턴 넘기기 발동!");
        // 현재 턴을 한 번 더 진행(상대 턴을 건너뜀)
        // 예: 턴을 바꾸지 않고 다시 AI/플레이어가 둠
    }
    
    // 궁극기: 돌 제거 및 배치
    public void Ultimate_RemoveAndPlace(GameManager gameManager, BoardManager boardManager)
    {
        Debug.Log("궁극기: 돌 제거 및 배치 발동!");
        // 특정 위치의 돌을 제거하고 새로운 돌을 배치하는 로직
        if (boardManager != null)
        {
            // 보드에서 돌 제거 및 배치 로직 구현
        }
    }
    
    // 일반 스킬: 데미지 증가
    public void Skill_DamageBoost(GameManager gameManager, int damageMultiplier = 2)
    {
        Debug.Log($"스킬: 데미지 증가 (배율: {damageMultiplier})");
        // 다음 공격의 데미지를 증가시키는 로직
    }
    
    // 일반 스킬: HP 회복
    public void Skill_Heal(GameManager gameManager, int healAmount = 1000)
    {
        Debug.Log($"스킬: HP 회복 (양: {healAmount})");
        // 플레이어 HP를 회복시키는 로직
        if (gameManager != null)
        {
            gameManager.ApplyDamageToPlayer1(-healAmount); // 음수 데미지로 회복
        }
    }
} 