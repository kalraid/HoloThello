using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class AutoSceneUISetup : MonoBehaviour
{
    [MenuItem("Tools/Auto UI Setup")]
    public static void SetupUI()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Canvas 생성
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // EventSystem 생성
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // 씬별로 UI 배치
        switch (sceneName)
        {
            case "MainScene":
                CreateText(canvas.transform, "메인 화면", new Vector2(0, 200));
                CreateButton(canvas.transform, "시작하기", new Vector2(0, 50));
                CreateButton(canvas.transform, "설정하기", new Vector2(0, -50));
                CreateImage(canvas.transform, "캐릭터/배경", new Vector2(0, -200));
                break;
            case "CharacterSelectScene":
                CreateText(canvas.transform, "캐릭터 선택", new Vector2(0, 200));
                CreateButton(canvas.transform, "1P 캐릭터 선택", new Vector2(-100, 0));
                CreateButton(canvas.transform, "CPU 캐릭터 선택", new Vector2(100, 0));
                break;
            case "SettingsScene":
                CreateText(canvas.transform, "설정", new Vector2(0, 200));
                CreateSlider(canvas.transform, "볼륨", new Vector2(0, 50));
                CreateDropdown(canvas.transform, "난이도", new Vector2(0, -50));
                break;
            case "GameScene":
                CreateText(canvas.transform, "게임 화면", new Vector2(0, 250));
                CreatePanel(canvas.transform, "상단 검정 바", new Vector2(0, 180), new Vector2(800, 100));
                CreatePanel(canvas.transform, "오셀로 보드 영역", new Vector2(0, 0), new Vector2(400, 400));
                CreatePanel(canvas.transform, "격투 애니메이션 영역", new Vector2(0, 100), new Vector2(400, 100));
                CreatePanel(canvas.transform, "체력바 영역", new Vector2(0, 220), new Vector2(800, 40));
                break;
            default:
                EditorUtility.DisplayDialog("Auto UI Setup", "지원하지 않는 씬입니다: " + sceneName, "확인");
                break;
        }

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Auto UI Setup", "UI 자동 배치가 완료되었습니다!", "확인");
    }

    static void CreateText(Transform parent, string text, Vector2 pos)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        rect.sizeDelta = new Vector2(400, 80);
    }

    static void CreateButton(Transform parent, string text, Vector2 pos)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform), typeof(Button), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(200, 60);

        var img = go.GetComponent<Image>();
        img.color = Color.white;

        GameObject txt = new GameObject("Text", typeof(RectTransform));
        txt.transform.SetParent(go.transform, false);
        var txtRect = txt.GetComponent<RectTransform>();
        txtRect.anchoredPosition = Vector2.zero;
        txtRect.sizeDelta = new Vector2(180, 50);

        var tmp = txt.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    static void CreateImage(Transform parent, string name, Vector2 pos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(200, 200);
    }

    static void CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        var img = go.GetComponent<Image>();
        img.color = new Color(0, 0, 0, 0.3f);
    }

    static void CreateSlider(Transform parent, string name, Vector2 pos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(300, 40);
    }

    static void CreateDropdown(Transform parent, string name, Vector2 pos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TMP_Dropdown));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(300, 40);
    }
} 