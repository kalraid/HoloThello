using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }
    
    [Header("주사위 UI")]
    public GameObject dicePanel;
    public Text dice1PText;
    public Text dice2PText;
    public Text resultText;
    public Button rollButton;
    public Button continueButton;
    
    [Header("주사위 애니메이션")]
    public GameObject dice1PObject;
    public GameObject dice2PObject;
    public float rollDuration = 1.0f;
    public float rollInterval = 0.1f;
    
    [Header("사운드")]
    public AudioClip diceRollSfx;
    public AudioClip diceResultSfx;
    
    private bool isRolling = false;
    private int currentDice1P = 0;
    private int currentDice2P = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDiceManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeDiceManager()
    {
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(StartDiceRoll);
        }
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueToGame);
            continueButton.gameObject.SetActive(false);
        }
        
        if (dicePanel != null)
        {
            dicePanel.SetActive(false);
        }
    }
    
    public void ShowDicePanel()
    {
        if (dicePanel != null)
        {
            dicePanel.SetActive(true);
            UpdateDiceUI();
        }
    }
    
    public void HideDicePanel()
    {
        if (dicePanel != null)
        {
            dicePanel.SetActive(false);
        }
    }
    
    void UpdateDiceUI()
    {
        if (dice1PText != null)
        {
            dice1PText.text = $"1P 주사위: {currentDice1P}";
        }
        
        if (dice2PText != null)
        {
            dice2PText.text = $"2P 주사위: {currentDice2P}";
        }
        
        if (resultText != null)
        {
            if (currentDice1P > 0 && currentDice2P > 0)
            {
                string result = "";
                if (currentDice1P > currentDice2P)
                {
                    result = "1P 선공!";
                }
                else if (currentDice2P > currentDice1P)
                {
                    result = "2P 선공!";
                }
                else
                {
                    result = "무승부! 랜덤 선공";
                }
                resultText.text = result;
            }
            else
            {
                resultText.text = "주사위를 굴려주세요!";
            }
        }
    }
    
    public void StartDiceRoll()
    {
        if (!isRolling)
        {
            StartCoroutine(RollDiceAnimation());
        }
    }
    
    IEnumerator RollDiceAnimation()
    {
        isRolling = true;
        
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }
        
        // 주사위 굴리기 사운드
        if (AudioManager.Instance != null && diceRollSfx != null)
        {
            AudioManager.Instance.PlaySFX(diceRollSfx);
        }
        
        // 주사위 애니메이션
        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            
            // 랜덤 주사위 값 표시
            currentDice1P = Random.Range(1, 7);
            currentDice2P = Random.Range(1, 7);
            
            UpdateDiceUI();
            
            // 주사위 오브젝트 회전 애니메이션
            if (dice1PObject != null)
            {
                dice1PObject.transform.Rotate(0, 0, 360 * Time.deltaTime);
            }
            if (dice2PObject != null)
            {
                dice2PObject.transform.Rotate(0, 0, -360 * Time.deltaTime);
            }
            
            yield return new WaitForSeconds(rollInterval);
        }
        
        // 최종 결과 결정
        GameData.Instance.DetermineFirstTurn();
        currentDice1P = GameData.Instance.diceResult1P;
        currentDice2P = GameData.Instance.diceResult2P;
        
        UpdateDiceUI();
        
        // 결과 사운드
        if (AudioManager.Instance != null && diceResultSfx != null)
        {
            AudioManager.Instance.PlaySFX(diceResultSfx);
        }
        
        // 주사위 오브젝트 정지
        if (dice1PObject != null)
        {
            dice1PObject.transform.rotation = Quaternion.identity;
        }
        if (dice2PObject != null)
        {
            dice2PObject.transform.rotation = Quaternion.identity;
        }
        
        // 계속하기 버튼 활성화
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
        
        if (rollButton != null)
        {
            rollButton.interactable = true;
        }
        
        isRolling = false;
    }
    
    public void ContinueToGame()
    {
        // 게임 씬으로 이동
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    // 주사위 결과 가져오기
    public int GetDiceResult1P()
    {
        return currentDice1P;
    }
    
    public int GetDiceResult2P()
    {
        return currentDice2P;
    }
    
    public bool IsPlayer1First()
    {
        return GameData.Instance.isPlayer1First;
    }
    
    // 주사위 굴리기 완료 여부
    public bool IsDiceRollComplete()
    {
        return GameData.Instance.isFirstTurnDetermined;
    }
} 