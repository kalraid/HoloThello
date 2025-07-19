using UnityEngine;

public enum CharacterType
{
    TypeA, // Hololive 캐릭터들
    TypeB  // 고양이 캐릭터들
}

[System.Serializable]
public class SkillData
{
    public string skillName;
    public string skillDescription;
    public int cooldown;
    public int damage;
    public bool isUltimate;
}

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public CharacterType characterType;
    public Sprite characterSprite;
    public Sprite discSprite;
    public SkillData skillA;
    public SkillData skillB;
    public SkillData ultimateA;
    public int characterIndex;
    public string description;
} 