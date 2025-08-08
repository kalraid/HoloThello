using UnityEngine;
using System.Collections.Generic;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance { get; private set; }
    
    [Header("Hololive 타입 캐릭터들")]
    public CharacterData[] hololiveCharacters;
    
    [Header("고양이 타입 캐릭터들")]
    public CharacterData[] catCharacters;
    
    private Dictionary<CharacterType, CharacterData[]> characterDataByType;
    
    // 테스트용 characters 필드 추가
    public CharacterData[] characters;
    
    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCharacterData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeCharacterData()
    {
        characterDataByType = new Dictionary<CharacterType, CharacterData[]>();
        
        // Hololive 타입 캐릭터들 초기화
        if (hololiveCharacters != null && hololiveCharacters.Length > 0)
        {
            characterDataByType[CharacterType.TypeA] = hololiveCharacters;
        }
        else
        {
            // 기본 Hololive 캐릭터들 생성
            characterDataByType[CharacterType.TypeA] = CreateDefaultHololiveCharacters();
        }
        
        // 고양이 타입 캐릭터들 초기화
        if (catCharacters != null && catCharacters.Length > 0)
        {
            characterDataByType[CharacterType.TypeB] = catCharacters;
        }
        else
        {
            // 기본 고양이 캐릭터들 생성
            characterDataByType[CharacterType.TypeB] = CreateDefaultCatCharacters();
        }
    }
    
    public CharacterData[] GetCharactersByType(CharacterType type)
    {
        if (characterDataByType.ContainsKey(type))
        {
            return characterDataByType[type];
        }
        
        Debug.LogWarning($"CharacterType {type}에 대한 캐릭터 데이터가 없습니다.");
        return new CharacterData[0];
    }
    
    public CharacterData GetCharacterData(int index)
    {
        if (characterDataByType == null)
            InitializeCharacterData();
        if (index < 0 || index >= 20) return null;
        if (index < 10)
            return characterDataByType[CharacterType.TypeA][index];
        else
            return characterDataByType[CharacterType.TypeB][index - 10];
    }
    
    private CharacterData[] CreateDefaultHololiveCharacters()
    {
        CharacterData[] characters = new CharacterData[10];
        
        for (int i = 0; i < 10; i++)
        {
            characters[i] = new CharacterData();
            characters[i].characterName = $"Hololive_{i + 1}";
            characters[i].characterType = CharacterType.TypeA;
            characters[i].characterIndex = i;
            characters[i].description = $"Hololive 캐릭터 {i + 1}";
            // 스프라이트는 나중에 설정
        }
        
        return characters;
    }
    
    private CharacterData[] CreateDefaultCatCharacters()
    {
        CharacterData[] characters = new CharacterData[10];
        
        for (int i = 0; i < 10; i++)
        {
            characters[i] = new CharacterData();
            characters[i].characterName = $"Cat_{i + 1}";
            characters[i].characterType = CharacterType.TypeB;
            characters[i].characterIndex = i;
            characters[i].description = $"고양이 캐릭터 {i + 1}";
            // 스프라이트는 나중에 설정
        }
        
        return characters;
    }
} 