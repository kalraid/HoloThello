using UnityEngine;

/// <summary>
/// Editor 스크립트에서 사용하는 상수들을 중앙화
/// </summary>
public static class EditorConstants
{
    // === 씬 경로 ===
    public static class Scenes
    {
        public const string MAIN_SCENE = "Assets/Scenes/MainScene.unity";
        public const string CHARACTER_SELECT_SCENE = "Assets/Scenes/CharacterSelectScene.unity";
        public const string SETTINGS_SCENE = "Assets/Scenes/SettingsScene.unity";
        public const string GAME_SCENE = "Assets/Scenes/GameScene.unity";
        
        public static readonly string[] ALL_SCENES = {
            MAIN_SCENE,
            CHARACTER_SELECT_SCENE,
            SETTINGS_SCENE,
            GAME_SCENE
        };
    }
    
    // === 폴더 경로 ===
    public static class Folders
    {
        public const string PREFABS = "Assets/Prefabs";
        public const string SPRITES_CATS = "Assets/Sprites/Cats";
        public const string AUDIO = "Assets/Audio";
        public const string EDITOR = "Assets/Editor";
    }
    
    // === 파일 경로 ===
    public static class Files
    {
        public const string DISC_PREFAB = "Assets/Prefabs/Disc.prefab";
        public const string INPUT_ACTIONS = "Assets/InputSystem_Actions.inputactions";
    }
    
    // === UI 설정값 ===
    public static class UI
    {
        public const float CANVAS_SCALE_WIDTH = 1920f;
        public const float CANVAS_SCALE_HEIGHT = 1080f;
        public const float CANVAS_MATCH = 0.5f;
        
        public const float BUTTON_WIDTH = 150f;
        public const float BUTTON_HEIGHT = 50f;
        public const float BUTTON_FONT_SIZE = 20f;
        
        public const float TITLE_FONT_SIZE = 30f;
        public const float LABEL_FONT_SIZE = 16f;
        public const float SMALL_FONT_SIZE = 14f;
    }
    
    // === 색상 ===
    public static class Colors
    {
        public static readonly Color BACKGROUND_DARK = new Color(0.1f, 0.1f, 0.1f, 1f);
        public static readonly Color BACKGROUND_LIGHT = new Color(0.2f, 0.2f, 0.2f, 1f);
        public static readonly Color TEXT_WHITE = Color.white;
        public static readonly Color TEXT_YELLOW = Color.yellow;
        public static readonly Color PLAYER_BLUE = Color.blue;
        public static readonly Color CPU_RED = Color.red;
        public static readonly Color BOARD_GREEN = Color.green;
        public static readonly Color BOARD_DARK_GREEN = Color.darkGreen;
    }
    
    // === 게임 설정값 ===
    public static class Game
    {
        public const int BOARD_SIZE = 8;
        public const int MAX_HP = 100;
        public const int SKILL_COUNT = 3;
        public const int CHARACTER_COUNT = 10;
    }
    
    // === 텍스처 설정 ===
    public static class Texture
    {
        public const int CAT_IMAGE_SIZE = 128;
        public const int TEXTURE_SIZE = 128;
    }
    
    // === 로깅 설정 ===
    public static class Logging
    {
        public const bool ENABLE_VERBOSE_LOGGING = false;
        public const int MAX_DEBUG_LOG_COUNT = 10;
    }
    
    // === 메뉴 경로 ===
    public static class Menus
    {
        public const string TOOLS_BASE = "Tools/";
        public const string PREFAB_MENU = TOOLS_BASE + "Prefab/";
        public const string SETUP_MENU = TOOLS_BASE + "Setup/";
        public const string BUTTON_UTILITY_MENU = TOOLS_BASE + "Button Utility/";
        public const string EDITOR_OPTIMIZATION_MENU = TOOLS_BASE + "Editor Optimization/";
        public const string TEST_MENU = TOOLS_BASE + "Test/";
    }
    
    // === 에러 메시지 ===
    public static class ErrorMessages
    {
        public const string CANVAS_NOT_FOUND = "Canvas를 찾을 수 없습니다!";
        public const string MANAGER_NOT_FOUND = "매니저를 찾을 수 없습니다!";
        public const string SCENE_NOT_FOUND = "씬 파일을 찾을 수 없습니다!";
        public const string AUDIO_FOLDER_NOT_FOUND = "Audio 폴더를 찾을 수 없습니다!";
        public const string SCRIPT_NOT_FOUND = "스크립트를 찾을 수 없습니다!";
    }
    
    // === 성공 메시지 ===
    public static class SuccessMessages
    {
        public const string SETUP_COMPLETE = "설정이 완료되었습니다!";
        public const string PREFAB_CREATED = "프리팹이 생성되었습니다!";
        public const string IMAGES_GENERATED = "이미지가 생성되었습니다!";
        public const string BUTTONS_CONNECTED = "버튼이 연결되었습니다!";
    }
} 