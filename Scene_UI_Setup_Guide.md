# Scene별 UI 설정 가이드

## 📋 개요
Unity 프로젝트의 각 Scene별 UI 구조와 MCP 자동 연결 내역을 정리한 문서입니다.

---

## 🏠 MainScene (메인 메뉴)

### UI 구조
```
MainScene/
├── Canvas/
│   ├── Background (배경)
│   ├── GameTitle (OSELLO)
│   ├── SubTitle (전략 보드 게임)
│   ├── VersionText (v1.0.0)
│   ├── StartButton (게임 시작)
│   ├── SettingsButton (설정)
│   └── QuitButton (종료)
├── EventSystem
├── GameManager
├── EffectManager
├── BoardManager
├── TestManager
└── Main Camera
```

### 버튼 설정
- **StartButton**: 게임 시작 → CharacterSelectScene으로 이동
- **SettingsButton**: 설정 → SettingsScene으로 이동
- **QuitButton**: 게임 종료

### MCP 키
- `GameManager`: 게임 매니저
- `Canvas`: UI 캔버스
- `StartButton`: 시작 버튼
- `SettingsButton`: 설정 버튼
- `QuitButton`: 종료 버튼

---

## 👥 CharacterSelectScene (캐릭터 선택)

### UI 구조
```
CharacterSelectScene/
├── Canvas/
│   ├── Background (배경)
│   ├── Title (캐릭터 선택)
│   ├── SelectLabel (캐릭터를 선택하세요)
│   ├── CharacterGrid (캐릭터 그리드)
│   │   ├── Character1 (캐릭터 1)
│   │   ├── Character2 (캐릭터 2)
│   │   └── ... (캐릭터 3-10)
│   ├── BackgroundSelectArea (배경 선택 영역)
│   │   ├── BackgroundBar_0
│   │   ├── BackgroundBar_1
│   │   ├── BackgroundBar_2
│   │   └── SelectedBackgroundImage
│   ├── BackButton (뒤로)
│   └── Button_확인 (확인)
├── EventSystem
├── CharacterSelectManager
├── CharacterDataManager
├── GameData
└── Main Camera
```

### 버튼 설정
- **Character1-10**: 캐릭터 선택 버튼
- **BackButton**: 뒤로가기 → MainScene으로 이동
- **Button_확인**: 확인 → GameScene으로 이동
- **BackgroundBar_0/1/2**: 배경 선택

### MCP 키
- `CharacterSelectManager`: 캐릭터 선택 매니저
- `CharacterDataManager`: 캐릭터 데이터 매니저
- `CharacterGrid`: 캐릭터 그리드
- `BackgroundSelectArea`: 배경 선택 영역
- `BackButton`: 뒤로 버튼
- `Button_확인`: 확인 버튼

---

## ⚙️ SettingsScene (설정)

### UI 구조
```
SettingsScene/
├── Canvas/
│   ├── Title (설정)
│   ├── VolumeArea/
│   │   ├── BGMLabel (배경음악)
│   │   ├── BGMSlider (볼륨 슬라이더)
│   │   ├── SFXLabel (효과음)
│   │   └── SFXSlider (볼륨 슬라이더)
│   ├── ScreenSizeArea/
│   │   ├── ScreenSizeLabel (화면 크기)
│   │   └── ScreenSizeDropdown (해상도 선택)
│   ├── DifficultyArea/
│   │   ├── DifficultyLabel (난이도)
│   │   └── DifficultyDropdown (난이도 선택)
│   ├── Button_돌아가기 (돌아가기)
│   ├── Button_적용 (적용)
│   └── Button_취소 (취소)
├── EventSystem
└── SettingsManager
```

### 버튼 설정
- **Button_돌아가기**: 뒤로가기 → MainScene으로 이동
- **Button_적용**: 설정 적용
- **Button_취소**: 설정 취소 → MainScene으로 이동
- **BGMSlider**: 배경음악 볼륨 조절
- **SFXSlider**: 효과음 볼륨 조절
- **ScreenSizeDropdown**: 화면 크기 선택
- **DifficultyDropdown**: 난이도 선택

### MCP 키
- `SettingsManager`: 설정 매니저
- `VolumeArea`: 볼륨 설정 영역
- `ScreenSizeArea`: 화면 크기 설정 영역
- `DifficultyArea`: 난이도 설정 영역
- `Button_돌아가기`: 돌아가기 버튼
- `Button_적용`: 적용 버튼
- `Button_취소`: 취소 버튼

---

## 🎮 GameScene (게임 플레이)

### UI 구조
```
GameScene/
├── Canvas/
│   ├── TopBar (상단 영역)
│   ├── FightingArea (전투 영역)
│   │   ├── PlayerHPBar (플레이어 HP)
│   │   │   ├── PlayerHPLabel
│   │   │   └── Fill Area
│   │   ├── CPUHPBar (CPU HP)
│   │   │   └── Fill Area
│   │   └── CharacterArea
│   │       ├── PlayerCharacter
│   │       ├── CPUCharacter
│   │       └── BackgroundImage
│   └── OthelloArea (스킬 영역)
│       ├── PlayerSkillButton_0 (스킬 1)
│       ├── PlayerSkillButton_1 (스킬 2)
│       ├── PlayerSkillButton_2 (스킬 3)
│       ├── CPUSkillButton_0 (CPU 스킬 1)
│       ├── CPUSkillButton_1 (CPU 스킬 2)
│       └── CPUSkillButton_2 (CPU 스킬 3)
├── BoardArea (게임 보드)
│   ├── BoardBackground
│   └── Cell_X_Y (8x8 그리드)
├── TurnText (턴 표시)
├── ResultText (결과 표시)
├── EventSystem
├── GameManager
├── BoardManager
├── AudioManager
├── GameData
└── Camera
```

### 버튼 설정
- **PlayerSkillButton_0/1/2**: 플레이어 스킬 버튼
- **CPUSkillButton_0/1/2**: CPU 스킬 버튼
- **Cell_X_Y**: 게임 보드 셀 (8x8 그리드)

### MCP 키
- `GameManager`: 게임 매니저
- `BoardManager`: 보드 매니저
- `AudioManager`: 오디오 매니저
- `FightingArea`: 전투 영역
- `PlayerHPBar`: 플레이어 HP 바
- `CPUHPBar`: CPU HP 바
- `OthelloArea`: 스킬 영역
- `PlayerSkillButton_0/1/2`: 플레이어 스킬 버튼
- `CPUSkillButton_0/1/2`: CPU 스킬 버튼
- `TurnText`: 턴 표시 텍스트
- `ResultText`: 결과 표시 텍스트

---

## 🔧 MCP 자동 연결 내역

### 1단계: 기본 매니저 연결
- ✅ GameManager, BoardManager, AudioManager, SettingsManager, CharacterSelectManager, CharacterDataManager 생성 및 설정

### 2단계: UI 요소 자동 생성
- ✅ Canvas, EventSystem 설정
- ✅ 배경, 제목, 버튼들 생성
- ✅ HP 바, 스킬 버튼, 드롭다운 메뉴 생성

### 3단계: 프리팹 연결
- ✅ DamageText, SkillButton, Disc 프리팹 연결
- ✅ 매니저 컴포넌트에 프리팹 참조 설정

### 4단계: 버튼 이벤트 연결
- ✅ 각 씬별 버튼에 적절한 텍스트 설정
- ✅ 버튼 위치 및 크기 최적화

---

## 🎯 Select Object 키 정리

### 공통 키
- `Canvas`: UI 캔버스
- `EventSystem`: 이벤트 시스템
- `Main Camera`: 메인 카메라

### MainScene 키
- `GameManager`
- `StartButton`
- `SettingsButton`
- `QuitButton`
- `GameTitle`
- `SubTitle`
- `VersionText`

### CharacterSelectScene 키
- `CharacterSelectManager`
- `CharacterDataManager`
- `CharacterGrid`
- `BackgroundSelectArea`
- `SelectLabel`
- `BackButton`
- `Button_확인`

### SettingsScene 키
- `SettingsManager`
- `VolumeArea`
- `ScreenSizeArea`
- `DifficultyArea`
- `Button_돌아가기`
- `Button_적용`
- `Button_취소`

### GameScene 키
- `GameManager`
- `BoardManager`
- `AudioManager`
- `FightingArea`
- `PlayerHPBar`
- `CPUHPBar`
- `OthelloArea`
- `PlayerSkillButton_0/1/2`
- `CPUSkillButton_0/1/2`
- `TurnText`
- `ResultText`

---

## 📝 참고사항

1. **씬 전환**: 각 씬의 버튼들이 올바른 씬으로 이동하도록 설정됨
2. **UI 일관성**: 모든 씬에서 동일한 스타일과 색상 사용
3. **반응형 UI**: CanvasScaler를 통한 다양한 해상도 지원
4. **MCP 자동화**: 모든 UI 요소가 MCP를 통해 자동 생성 및 연결됨

이 문서를 참고하여 각 씬의 UI 구조를 이해하고 MCP 자동 연결을 활용하세요! 