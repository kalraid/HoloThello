# Unity 에디터 설정 가이드 - HoloThello 프로젝트

## 개요
이 가이드는 Unity 에디터에서 UI 컴포넌트들을 연결하여 HoloThello 프로젝트를 완성하는 방법을 설명합니다.

## 📅 **2024-07-21 작업 완료 현황**

### ✅ **완료된 작업**

#### 1. **Editor 최적화 및 에러 수정**
- [x] **Editor 스크립트 최적화**: Debug.Log 과다 사용 문제 해결
- [x] **CompleteUnitySetup.cs 모듈화**: 978줄 대용량 파일 분리
- [x] **EditorConstants.cs 생성**: 하드코딩된 경로들 중앙화
- [x] **EditorMenuManager.cs 생성**: 분산된 메뉴 아이템 통합
- [x] **EditorCommonUtility.cs 생성**: 공통 유틸리티 함수 모듈화
- [x] **EditorOptimizationUtility.cs 생성**: 최적화 도구 중앙화

#### 2. **MCP 서버 연동 및 자동화**
- [x] **MCP 서버 연결 확인**: WebSocket 서버 정상 작동
- [x] **씬별 UI 자동화**: MainScene, CharacterSelectScene, SettingsScene
- [x] **중복 UI 요소 정리**: 각 씬의 중복된 GameObjects 삭제
- [x] **버튼 이벤트 자동 연결**: MCP를 통한 버튼 이벤트 설정

#### 3. **씬별 버튼 연결 완료**

##### **MainScene**
- [x] **MainMenuManager 스크립트 생성**: 메뉴 버튼 관리
- [x] **버튼 연결**:
  - StartButton → CharacterSelectScene
  - SettingsButton → SettingsScene
  - QuitButton → 게임 종료
- [x] **UI 텍스트 연결**: GameTitle, SubTitle, VersionText
- [x] **키보드 입력 처리**: Enter, S, Escape 키 지원

##### **CharacterSelectScene**
- [x] **CharacterSelectManager 버튼 연결**:
  - TypeAButton → OnClickTypeA()
  - TypeBButton → OnClickTypeB()
  - PlayerVsCPUButton → OnClickPlayerVsCPU()
  - PlayerVsPlayerButton → OnClickPlayerVsPlayer()
  - CPUVsCPUButton → OnClickCPUVsCPU()
  - StartButton → OnClickConfirm()
  - BackButton → OnClickBack() (MainScene으로 이동)
- [x] **OnClickBack 메서드 추가**: SceneController 사용

##### **SettingsScene**
- [x] **SettingsManager 버튼 연결**:
  - MasterVolumeSlider → OnMasterVolumeChanged()
  - BGMVolumeSlider → OnBGMVolumeChanged()
  - SFXVolumeSlider → OnSFXVolumeChanged()
  - FullscreenToggle → OnFullscreenChanged()
  - ResolutionDropdown → OnResolutionChanged()
  - TypeAButton → OnTypeAButtonClicked()
  - TypeBButton → OnTypeBButtonClicked()
  - BackButton → OnBackButtonClicked()
- [x] **OnBackButtonClicked 수정**: SceneController 사용

#### 4. **SceneController 생성**
- [x] **씬 전환 로직 중앙화**: 모든 씬 전환을 SceneController로 통합
- [x] **싱글톤 패턴 구현**: Instance 프로퍼티로 전역 접근
- [x] **씬별 전환 메서드**: LoadMainScene, LoadCharacterSelectScene, LoadSettingsScene, LoadGameScene
- [x] **게임 종료 기능**: QuitGame 메서드

#### 5. **문서화 및 정리**
- [x] **Scene_UI_Setup_Guide.md 생성**: 씬별 UI 구조 및 MCP 키 문서화
- [x] **중복 .md 파일 정리**: 불필요한 테스트 관련 문서 삭제
- [x] **Final_Optimization_Summary.md 업데이트**: 최적화 결과 통합

### 🔄 **남은 작업**

#### **GameScene UI 연결**
- [ ] **GameManager 스크립트 설정**: HP Bar, 스킬 버튼, 결과 UI 연결
- [ ] **EffectManager 스크립트 설정**: 프리팹 연결
- [ ] **BoardManager 스크립트 설정**: 보드 셀 연결
- [ ] **TestManager 스크립트 설정**: 테스트 UI 연결

#### **세부 설정**
- [ ] **스킬 버튼 CooldownText 설정**: 각 스킬 버튼의 쿨타임 텍스트
- [ ] **HP Bar 세그먼트 설정**: 1000 단위 구분선
- [ ] **테스트 버튼 설정**: 기능별 테스트 버튼 이벤트 연결

---

## 1. GameManager 스크립트 설정

### 1.1 HP 관련 UI 컴포넌트 연결

**Inspector 위치**: `GameManager` 스크립트의 Inspector 창

#### HP Bar 컴포넌트
- **Player HP Bar**: `playerHpBar` 필드
  - Hierarchy에서 `Canvas > GameUI > PlayerHPBar` 오브젝트를 드래그 앤 드롭
- **CPU HP Bar**: `cpuHpBar` 필드
  - Hierarchy에서 `Canvas > GameUI > CPUHPBar` 오브젝트를 드래그 앤 드롭

#### HP Text 컴포넌트
- **Player HP Text**: `playerHpText` 필드
  - Hierarchy에서 `Canvas > GameUI > PlayerHPText` 오브젝트를 드래그 앤 드롭
- **CPU HP Text**: `cpuHpText` 필드
  - Hierarchy에서 `Canvas > GameUI > CPUHPText` 오브젝트를 드래그 앤 드롭

### 1.2 스킬 버튼 컴포넌트 연결

#### 플레이어 스킬 버튼
- **Player Skill Buttons**: `playerSkillButtons` 배열
  - Size를 3으로 설정
  - Element 0: `Canvas > GameUI > SkillPanel > PlayerSkill1` 오브젝트
  - Element 1: `Canvas > GameUI > SkillPanel > PlayerSkill2` 오브젝트
  - Element 2: `Canvas > GameUI > SkillPanel > PlayerSkill3` 오브젝트

#### CPU 스킬 버튼
- **CPU Skill Buttons**: `cpuSkillButtons` 배열
  - Size를 3으로 설정
  - Element 0: `Canvas > GameUI > SkillPanel > CPUSkill1` 오브젝트
  - Element 1: `Canvas > GameUI > SkillPanel > CPUSkill2` 오브젝트
  - Element 2: `Canvas > GameUI > SkillPanel > CPUSkill3` 오브젝트

### 1.3 게임 결과 UI 컴포넌트 연결

#### 결과 패널
- **Result Panel**: `resultPanel` 필드
  - Hierarchy에서 `Canvas > GameUI > ResultPanel` 오브젝트를 드래그 앤 드롭

#### 결과 텍스트
- **Result Title Text**: `resultTitleText` 필드
  - Hierarchy에서 `Canvas > GameUI > ResultPanel > TitleText` 오브젝트를 드래그 앤 드롭
- **Result Detail Text**: `resultDetailText` 필드
  - Hierarchy에서 `Canvas > GameUI > ResultPanel > DetailText` 오브젝트를 드래그 앤 드롭

#### 결과 버튼
- **Result Continue Button**: `resultContinueButton` 필드
  - Hierarchy에서 `Canvas > GameUI > ResultPanel > ContinueButton` 오브젝트를 드래그 앤 드롭
- **Result Restart Button**: `resultRestartButton` 필드
  - Hierarchy에서 `Canvas > GameUI > ResultPanel > RestartButton` 오브젝트를 드래그 앤 드롭

## 2. EffectManager 스크립트 설정

### 2.1 프리팹 연결

**Inspector 위치**: `EffectManager` 스크립트의 Inspector 창

#### 데미지 텍스트 프리팹
- **Damage Text Prefab**: `damageTextPrefab` 필드
  - Project 창에서 `Assets > Prefabs > DamageText` 프리팹을 드래그 앤 드롭

#### 스킬 버튼 프리팹
- **Skill Button Prefab**: `skillButtonPrefab` 필드
  - Project 창에서 `Assets > Prefabs > SkillButton` 프리팹을 드래그 앤 드롭

## 3. BoardManager 스크립트 설정

### 3.1 게임 보드 관련 컴포넌트

**Inspector 위치**: `BoardManager` 스크립트의 Inspector 창

#### 디스크 프리팹
- **Disc Prefab**: `discPrefab` 필드
  - Project 창에서 `Assets > Prefabs > Disc` 프리팹을 드래그 앤 드롭

#### 보드 셀
- **Board Cells**: `boardCells` 배열
  - Size를 64로 설정 (8x8 보드)
  - 각 셀을 순서대로 연결 (왼쪽 상단부터 오른쪽 하단까지)

## 4. TestManager 스크립트 설정

### 4.1 테스트 UI 컴포넌트

**Inspector 위치**: `TestManager` 스크립트의 Inspector 창

#### 기본 테스트 UI
- **Test Result Text**: `testResultText` 필드
  - Hierarchy에서 `Canvas > TestUI > TestResultText` 오브젝트를 드래그 앤 드롭
- **Run All Tests Button**: `runAllTestsButton` 필드
  - Hierarchy에서 `Canvas > TestUI > RunAllTestsButton` 오브젝트를 드래그 앤 드롭
- **Clear Test Button**: `clearTestButton` 필드
  - Hierarchy에서 `Canvas > TestUI > ClearTestButton` 오브젝트를 드래그 앤 드롭

#### 기능별 테스트 버튼
- **Test Disc Button**: `testDiscButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestDiscButton` 오브젝트를 드래그 앤 드롭
- **Test HP Button**: `testHPButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestHPButton` 오브젝트를 드래그 앤 드롭
- **Test Skill Button**: `testSkillButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestSkillButton` 오브젝트를 드래그 앤 드롭
- **Test Damage Button**: `testDamageButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestDamageButton` 오브젝트를 드래그 앤 드롭
- **Test Game End Button**: `testGameEndButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestGameEndButton` 오브젝트를 드래그 앤 드롭
- **Test HP Bar Button**: `testHPBarButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestHPBarButton` 오브젝트를 드래그 앤 드롭
- **Test Prefab Button**: `testPrefabButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestPrefabButton` 오브젝트를 드래그 앤 드롭
- **Test Performance Button**: `testPerformanceButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestPerformanceButton` 오브젝트를 드래그 앤 드롭
- **Test Character Button**: `testCharacterButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestCharacterButton` 오브젝트를 드래그 앤 드롭
- **Test Board Button**: `testBoardButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestBoardButton` 오브젝트를 드래그 앤 드롭
- **Test Audio Button**: `testAudioButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestAudioButton` 오브젝트를 드래그 앤 드롭
- **Test Mobile Button**: `testMobileButton` 필드
  - Hierarchy에서 `Canvas > TestUI > TestButtons > TestMobileButton` 오브젝트를 드래그 앤 드롭

## 5. UI 계층 구조 설정

### 5.1 Canvas 설정
1. **Canvas** 오브젝트 선택
2. **Canvas Scaler** 컴포넌트에서:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: X=1920, Y=1080
   - Screen Match Mode: Match Width Or Height
   - Match: 0.5

### 5.2 UI 패널 구성
```
Canvas
├── GameUI
│   ├── PlayerHPBar
│   ├── CPUHPBar
│   ├── PlayerHPText
│   ├── CPUHPText
│   ├── SkillPanel
│   │   ├── PlayerSkill1
│   │   ├── PlayerSkill2
│   │   ├── PlayerSkill3
│   │   ├── CPUSkill1
│   │   ├── CPUSkill2
│   │   └── CPUSkill3
│   └── ResultPanel
│       ├── TitleText
│       ├── DetailText
│       ├── ContinueButton
│       └── RestartButton
└── TestUI
    ├── TestResultText
    ├── RunAllTestsButton
    ├── ClearTestButton
    └── TestButtons
        ├── TestDiscButton
        ├── TestHPButton
        ├── TestSkillButton
        ├── TestDamageButton
        ├── TestGameEndButton
        ├── TestHPBarButton
        ├── TestPrefabButton
        ├── TestPerformanceButton
        ├── TestCharacterButton
        ├── TestBoardButton
        ├── TestAudioButton
        └── TestMobileButton
```

## 6. 스킬 버튼 하위 오브젝트 설정

### 6.1 CooldownText 설정
각 스킬 버튼에 다음 하위 오브젝트가 있어야 합니다:

1. **PlayerSkill1 > CooldownText**
   - Text 컴포넌트 추가
   - 초기 상태: 비활성화 (SetActive(false))
   - 폰트 크기: 16
   - 색상: 흰색

2. **PlayerSkill2 > CooldownText**
   - 동일한 설정

3. **PlayerSkill3 > CooldownText**
   - 동일한 설정

4. **CPUSkill1 > CooldownText**
   - 동일한 설정

5. **CPUSkill2 > CooldownText**
   - 동일한 설정

6. **CPUSkill3 > CooldownText**
   - 동일한 설정

## 7. HP Bar 세그먼트 설정

### 7.1 세그먼트 라인 생성
각 HP Bar에 세그먼트 라인을 추가:

1. **PlayerHPBar > Segments** 오브젝트 생성
2. **CPUHPBar > Segments** 오브젝트 생성

각 Segments 오브젝트는 자동으로 세그먼트 라인을 생성합니다.

## 8. 테스트 버튼 설정

### 8.1 기능별 테스트 버튼 생성
각 테스트 버튼을 생성하고 설정:

1. **TestDiscButton**
   - Text: "Disc 테스트"
   - OnClick: TestManager.RunSingleTest("Disc")

2. **TestHPButton**
   - Text: "HP 테스트"
   - OnClick: TestManager.RunSingleTest("HP")

3. **TestSkillButton**
   - Text: "스킬 테스트"
   - OnClick: TestManager.RunSingleTest("Skill")

4. **TestDamageButton**
   - Text: "데미지 테스트"
   - OnClick: TestManager.RunSingleTest("Damage")

5. **TestGameEndButton**
   - Text: "게임종료 테스트"
   - OnClick: TestManager.RunSingleTest("GameEnd")

6. **TestHPBarButton**
   - Text: "HP바 테스트"
   - OnClick: TestManager.RunSingleTest("HPBar")

7. **TestPrefabButton**
   - Text: "프리팹 테스트"
   - OnClick: TestManager.RunSingleTest("Prefab")

8. **TestPerformanceButton**
   - Text: "성능 테스트"
   - OnClick: TestManager.RunSingleTest("Performance")

9. **TestCharacterButton**
   - Text: "캐릭터 테스트"
   - OnClick: TestManager.RunSingleTest("Character")

10. **TestBoardButton**
    - Text: "보드 테스트"
    - OnClick: TestManager.RunSingleTest("Board")

11. **TestAudioButton**
    - Text: "오디오 테스트"
    - OnClick: TestManager.RunSingleTest("Audio")

12. **TestMobileButton**
    - Text: "모바일 테스트"
    - OnClick: TestManager.RunSingleTest("Mobile")

## 9. 검증 단계

### 9.1 컴포넌트 연결 확인
1. 모든 필드가 null이 아닌지 확인
2. 배열의 크기가 올바른지 확인
3. 프리팹 참조가 올바른지 확인

### 9.2 테스트 실행
1. 게임 실행
2. TestManager의 자동 테스트 확인
3. 각 기능별 테스트 버튼 클릭하여 개별 테스트 실행
4. 각 기능이 정상 작동하는지 확인

## 10. 문제 해결

### 10.1 일반적인 오류
- **NullReferenceException**: 컴포넌트가 연결되지 않음
- **MissingReferenceException**: 프리팹 참조 누락
- **Array Index Out of Range**: 배열 크기 설정 오류

### 10.2 해결 방법
1. Inspector에서 모든 필드 확인
2. Hierarchy에서 오브젝트 이름 확인
3. Project 창에서 프리팹 경로 확인

## 11. 완료 체크리스트

- [x] **MainScene 버튼 연결 완료**
- [x] **CharacterSelectScene 버튼 연결 완료**
- [x] **SettingsScene 버튼 연결 완료**
- [ ] GameManager의 모든 UI 컴포넌트 연결
- [ ] EffectManager의 프리팹 연결
- [ ] BoardManager의 보드 셀 연결
- [ ] TestManager의 테스트 UI 연결
- [ ] 기능별 테스트 버튼 설정
- [ ] 스킬 버튼의 CooldownText 설정
- [ ] HP Bar 세그먼트 설정
- [ ] 게임 실행 및 테스트 확인
- [ ] 모든 기능 정상 작동 확인

## 12. 테스트 실행 방법

### 12.1 전체 테스트 실행
1. **Run All Tests** 버튼 클릭
2. 모든 기능을 한 번에 테스트
3. 결과를 TestResultText에서 확인

### 12.2 개별 기능 테스트
1. 원하는 기능의 테스트 버튼 클릭
2. 해당 기능만 테스트
3. 결과를 TestResultText에서 확인

### 12.3 테스트 결과 해석
- ✅: 정상 작동
- ❌: 문제 발견
- 📊: 성능 지표

## 13. MCP 자동화 현황

### 13.1 완료된 자동화
- [x] **MainScene UI 자동화**: 버튼 생성 및 이벤트 연결
- [x] **CharacterSelectScene UI 자동화**: 캐릭터 선택 UI 및 버튼 연결
- [x] **SettingsScene UI 자동화**: 설정 UI 및 슬라이더 연결
- [x] **중복 요소 정리**: 각 씬의 중복된 GameObjects 삭제

### 13.2 남은 자동화
- [ ] **GameScene UI 자동화**: 게임 UI 및 매니저 연결
- [ ] **프리팹 연결 자동화**: EffectManager, BoardManager 프리팹 설정
- [ ] **테스트 UI 자동화**: TestManager UI 컴포넌트 연결

이 가이드를 따라 설정하면 HoloThello 프로젝트가 100% 완성됩니다! 