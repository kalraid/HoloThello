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
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void OnClickSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
} 