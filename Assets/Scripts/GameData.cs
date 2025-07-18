using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    // 선택 정보
    public int playerCharacterIdx = 0;
    public int cpuCharacterIdx = 1;
    public int backgroundIdx = 0;
    public int difficulty = 0; // 0:보통, 1:어려움 등

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
} 