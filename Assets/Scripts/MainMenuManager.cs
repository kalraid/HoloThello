using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Image backgroundImage;
    public Image characterImage;
    public Sprite[] characterSprites;
    public float characterChangeInterval = 2f;
    private int currentCharacter = 0;
    private float timer = 0f;

    void Start()
    {
        if (characterSprites.Length > 0)
            characterImage.sprite = characterSprites[0];
    }

    void Update()
    {
        // 캐릭터 이미지 순환
        if (characterSprites.Length > 1)
        {
            timer += Time.deltaTime;
            if (timer > characterChangeInterval)
            {
                timer = 0f;
                currentCharacter = (currentCharacter + 1) % characterSprites.Length;
                characterImage.sprite = characterSprites[currentCharacter];
            }
        }
    }

    public void OnClickStart()
    {
        Debug.Log("[MainMenuManager] OnClickStart 호출됨 - CharacterSelectScene으로 이동 시도");
        SceneManager.LoadScene("CharacterSelectScene", LoadSceneMode.Single);
    }

    public void OnClickSettings()
    {
        Debug.Log("[MainMenuManager] OnClickSettings 호출됨 - SettingsScene으로 이동 시도");
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void OnClickExit()
    {
        Debug.Log("[MainMenuManager] OnClickExit 호출됨 - 게임 종료 시도");
#if UNITY_EDITOR
        Debug.Log("[MainMenuManager] (에디터 환경에서는 종료되지 않습니다)");
#else
        Application.Quit();
#endif
    }
} 