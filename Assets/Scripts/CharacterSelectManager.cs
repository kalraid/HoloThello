using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectManager : MonoBehaviour
{
    public Image[] characterBarImages; // 캐릭터 바(작은 사진) UI
    public Image fullCharacterImage;   // 선택된 캐릭터 풀사진
    public Sprite[] characterSprites;  // 캐릭터 스프라이트(작은/큰 동일 순서)
    public Image[] backgroundBarImages; // 배경 선택 바 UI
    public Sprite[] backgroundSprites;  // 배경 스프라이트
    public Image selectedBackgroundImage; // 선택된 배경 미리보기
    public GameObject cpuBlockOverlay; // CPU가 선택 불가한 캐릭터에 덮을 오브젝트(회색 처리)
    public Text selectLabel; // '1P 선택', 'CPU 선택', '배경 선택' 등 안내

    private int selectStep = 0; // 0:1P, 1:CPU, 2:배경
    private int selected1P = -1;
    private int selectedCPU = -1;
    private int selectedBackground = 0;

    void Start()
    {
        selectStep = 0;
        selected1P = -1;
        selectedCPU = -1;
        selectedBackground = 0;
        UpdateUI();
    }

    void Update()
    {
        // 키보드 좌우 이동/선택 처리 (예시: 좌우 화살표, Z키)
        if (selectStep < 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveSelection(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveSelection(1);
            if (Input.GetKeyDown(KeyCode.Z))
                ConfirmSelection();
        }
        else if (selectStep == 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveBackground(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveBackground(1);
            if (Input.GetKeyDown(KeyCode.Z))
                ConfirmBackground();
        }
    }

    public void OnClickCharacter(int idx)
    {
        if (selectStep == 0 && idx != selectedCPU)
        {
            selected1P = idx;
            UpdateUI();
        }
        else if (selectStep == 1 && idx != selected1P)
        {
            selectedCPU = idx;
            UpdateUI();
        }
    }

    public void OnClickBackground(int idx)
    {
        if (selectStep == 2)
        {
            selectedBackground = idx;
            UpdateUI();
        }
    }

    public void OnClickConfirm()
    {
        if (selectStep < 2)
            ConfirmSelection();
        else
            ConfirmBackground();
    }

    void MoveSelection(int dir)
    {
        int max = characterSprites.Length;
        if (selectStep == 0)
        {
            if (selected1P == -1) selected1P = 0;
            else selected1P = (selected1P + dir + max) % max;
            if (selected1P == selectedCPU) selected1P = (selected1P + dir + max) % max;
        }
        else if (selectStep == 1)
        {
            if (selectedCPU == -1) selectedCPU = 0;
            else selectedCPU = (selectedCPU + dir + max) % max;
            if (selectedCPU == selected1P) selectedCPU = (selectedCPU + dir + max) % max;
        }
        UpdateUI();
    }

    void MoveBackground(int dir)
    {
        int max = backgroundSprites.Length;
        selectedBackground = (selectedBackground + dir + max) % max;
        UpdateUI();
    }

    void ConfirmSelection()
    {
        if (selectStep == 0 && selected1P != -1)
        {
            selectStep = 1;
            // 1P가 고른 캐릭터는 CPU가 선택 불가
            if (selectedCPU == selected1P) selectedCPU = -1;
        }
        else if (selectStep == 1 && selectedCPU != -1)
        {
            selectStep = 2;
        }
        UpdateUI();
    }

    void ConfirmBackground()
    {
        // 선택 완료 → 게임 씬으로 이동, 선택 정보 저장
        if (GameData.Instance)
        {
            GameData.Instance.playerCharacterIdx = selected1P;
            GameData.Instance.cpuCharacterIdx = selectedCPU;
            GameData.Instance.backgroundIdx = selectedBackground;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void UpdateUI()
    {
        // 캐릭터 바/풀사진/회색 처리/선택 박스 등 UI 갱신
        for (int i = 0; i < characterBarImages.Length; i++)
        {
            characterBarImages[i].sprite = characterSprites[i];
            // 1P/CPU가 선택한 캐릭터는 테두리/색상 등으로 표시
            characterBarImages[i].color = Color.white;
            if (selectStep == 0 && i == selected1P) characterBarImages[i].color = Color.yellow;
            if (selectStep == 1 && i == selectedCPU) characterBarImages[i].color = Color.cyan;
            // CPU가 선택 불가한 캐릭터(1P가 고른 것)는 회색 처리
            if (selectStep == 1 && i == selected1P) characterBarImages[i].color = Color.gray;
        }
        // 풀사진 갱신
        if (selectStep == 0 && selected1P != -1)
            fullCharacterImage.sprite = characterSprites[selected1P];
        else if (selectStep == 1 && selectedCPU != -1)
            fullCharacterImage.sprite = characterSprites[selectedCPU];
        // 안내 텍스트
        if (selectStep == 0) selectLabel.text = "1P 캐릭터를 선택하세요";
        else if (selectStep == 1) selectLabel.text = "CPU 캐릭터를 선택하세요";
        else selectLabel.text = "배경을 선택하세요";
        // 배경 바/미리보기
        for (int i = 0; i < backgroundBarImages.Length; i++)
        {
            backgroundBarImages[i].sprite = backgroundSprites[i];
            backgroundBarImages[i].color = (i == selectedBackground) ? Color.yellow : Color.white;
        }
        selectedBackgroundImage.sprite = backgroundSprites[selectedBackground];
    }
} 