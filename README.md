# 🎮 HoloThello - 홀로 격투 오셀로

## 📖 소개
HoloThello는 오셀로 게임에 격투 요소와 캐릭터 선택을 결합한 혁신적인 캐주얼 게임입니다. 20개의 귀여운 캐릭터(10개 Hololive 스타일, 10개 고양이)와 함께 오셀로의 전략적 사고와 격투 게임의 긴장감을 동시에 즐길 수 있습니다.

### 🎯 게임 특징
- **턴제 방식 HP 게임**: 오셀로로 상대방 HP를 깎아내리는 전략적 대전
- **20개 캐릭터 시스템**: A타입(Hololive 스타일) 10개, B타입(고양이) 10개
- **3개 스킬 시스템**: 스킬A, 스킬B, 궁극기A로 전략적 플레이
- **실시간 체력 시스템**: 10000P 체력으로 긴장감 있는 대전
- **3가지 게임 모드**: 1P vs CPU, 1P vs 2P, CPU vs CPU
- **주사위 시스템**: 6면체 주사위로 선공 결정

### 📊 **프로젝트 현황**
- **진행률**: 99% 완료
- **남은 작업**: Unity 에디터에서 UI 컴포넌트 연결 (1%)
- **최종 완성**: Unity_Editor_Setup_Guide.md 참조

## 🚀 빌드 및 배포

### 📋 시스템 요구사항
- **운영체제**: Windows 10/11 (64-bit)
- **Unity 버전**: 2022.3 LTS 이상
- **메모리**: 최소 4GB RAM
- **저장공간**: 500MB 이상

### 🔧 개발 환경 설정
```bash
# 1. Unity Hub 설치
# https://unity.com/download

# 2. 프로젝트 클론
git clone https://github.com/your-repo/HoloThello.git
cd HoloThello

# 3. Unity Hub에서 프로젝트 열기
# Unity 2022.3 LTS 이상 선택
```

### 🛠️ 빌드 방법

#### **개발용 빌드**
```bash
# Unity Editor에서
1. File > Build Settings
2. Platform: PC, Mac & Linux Standalone 선택
3. Target Platform: Windows 선택
4. Build 버튼 클릭
```

#### **배포용 빌드**
```bash
# Unity Editor에서
1. File > Build Settings
2. Player Settings > Other Settings
3. Company Name: "HoloThello"
4. Product Name: "HoloThello"
5. Version: "1.0.0"
6. Build 버튼 클릭
```

### 📦 배포 패키지 구성
```
HoloThello/
├── HoloThello.exe          # 메인 실행 파일
├── HoloThello_Data/        # 게임 데이터 폴더
│   ├── Managed/            # .NET 어셈블리
│   ├── StreamingAssets/    # 스트리밍 에셋
│   └── il2cpp_data/       # IL2CPP 데이터
├── MonoBleedingEdge/       # Mono 런타임
└── README.md              # 설치 가이드
```

## 🎮 실행(부팅) 방법

### **Unity Editor에서 실행**
1. **Unity Hub에서 프로젝트 열기**
2. **Assets/Scenes/MainScene.unity 열기**
3. **Tools > Complete Unity Setup - All Scenes 실행**
4. **Unity_Editor_Setup_Guide.md 참조하여 UI 컴포넌트 연결**
5. **▶️ Play 버튼으로 실행**

### **빌드된 게임 실행**
```bash
# Windows
./HoloThello.exe

# 또는 더블클릭으로 실행
```

### **개발 모드 실행**
```bash
# Unity Editor에서
1. Window > Package Manager
2. Unity MCP Server 설치 (필요시)
3. Tools > Generate Cat Images (고양이 이미지 생성)
4. Unity_Editor_Setup_Guide.md 따라 UI 컴포넌트 연결
5. Play 모드로 테스트
```

## 🧪 테스트

### **자동화된 테스트**
```bash
# Unity Editor에서
1. Window > General > Test Runner
2. EditMode > Run All Tests
3. PlayMode > Run All Tests
```

### **수동 테스트 체크리스트**
- [ ] 메인 화면 로딩 및 버튼 동작
- [ ] 캐릭터 선택 (20개 캐릭터)
- [ ] 게임 모드 선택 (1P vs CPU, 1P vs 2P, CPU vs CPU)
- [ ] 주사위 굴리기 및 선공 결정
- [ ] 설정 화면 (볼륨, 화면 크기)
- [ ] 게임 화면 (오셀로, 스킬, 체력바)
- [ ] 씬 전환 및 데이터 저장

### **성능 테스트**
```bash
# Unity Profiler 사용
1. Window > Analysis > Profiler
2. Play 모드에서 성능 측정
3. FPS, 메모리 사용량 확인
```

## 📁 프로젝트 구조
```
HoloThello/
├── Assets/
│   ├── Scripts/           # 게임 로직 C# 스크립트
│   │   ├── GameManager.cs
│   │   ├── CharacterData.cs
│   │   ├── BoardManager.cs
│   │   ├── EffectManager.cs
│   │   ├── TestManager.cs
│   │   └── ...
│   ├── Scenes/           # 게임 씬들
│   │   ├── MainScene.unity
│   │   ├── CharacterSelectScene.unity
│   │   ├── GameScene.unity
│   │   └── SettingsScene.unity
│   ├── Sprites/          # 이미지 에셋
│   │   ├── Cats/         # 고양이 캐릭터 이미지
│   │   ├── Anime/        # Hololive 스타일 이미지
│   │   └── UI/           # UI 이미지
│   ├── Prefabs/          # 프리팹
│   │   ├── Disc.prefab
│   │   ├── DamageText.prefab
│   │   └── SkillButton.prefab
│   └── Editor/           # 에디터 스크립트
├── ProjectSettings/       # Unity 프로젝트 설정
├── Packages/             # 패키지 관리
├── TODO.md              # 작업 계획서 (99% 완료)
├── Unity_Editor_Setup_Guide.md  # Unity 에디터 설정 가이드
└── README.md            # 이 파일
```

## 🔧 개발 도구

### **Unity Editor 도구**
- **Tools > Complete Unity Setup - All Scenes**: 전체 설정 자동화
- **Tools > Generate Cat Images**: 고양이 이미지 생성
- **Tools > Setup CharacterSelectScene**: 캐릭터 선택 화면 설정
- **Tools > Setup GameScene**: 게임 화면 설정

### **디버깅 도구**
```bash
# Unity Console에서
Debug.Log("디버그 메시지");

# 프로파일링
Window > Analysis > Profiler
Window > Analysis > Frame Debugger
```

## 🐛 문제 해결

### **일반적인 문제들**
1. **씬이 로드되지 않음**
   - Build Settings에서 씬 추가 확인
   - File > Build Settings > Add Open Scenes

2. **캐릭터 이미지가 표시되지 않음**
   - Tools > Generate Cat Images 실행
   - Assets/Sprites/Cats/ 폴더 확인

3. **UI 컴포넌트가 연결되지 않음**
   - Unity_Editor_Setup_Guide.md 참조
   - GameManager, EffectManager 등의 Inspector 확인

4. **스킬 버튼이 작동하지 않음**
   - GameManager의 public 변수 연결 확인
   - CharacterDataManager 인스턴스 확인

### **로그 확인**
```bash
# Unity Console에서 에러/경고 확인
# 또는 빌드된 게임의 로그 파일 확인
```

## 🎯 **최근 업데이트 (2024-07-21)**

### **완료된 주요 기능**
- ✅ **Disc 오브젝트 가시성 문제 해결**: MiniImage 오타 수정
- ✅ **HP Bar 수치 표시**: 실시간 HP 수치 및 색상 변경
- ✅ **HP Bar 세그먼트 정렬**: 1000 단위 구분선 정확한 위치 설정
- ✅ **스킬 UI 개선**: 쿨타임 표시 및 버튼 상태 시각화
- ✅ **데미지 이펙트 강화**: 데미지 크기별 색상 및 애니메이션
- ✅ **게임 결과 UI**: 승리/패배 결과 패널 및 재시작 기능
- ✅ **테스트 시스템**: 종합적인 기능 테스트 및 성능 모니터링
- ✅ **프리팹 생성**: DamageText, SkillButton 프리팹 완성

### **남은 작업**
- 🔄 **Unity 에디터 설정**: UI 컴포넌트 연결 (1%)
- 📋 **설정 가이드**: Unity_Editor_Setup_Guide.md 참조

## 📄 라이선스
- **오픈소스**: MIT License
- **Hololive 에셋**: 비상업적 2차 창작 허용
- **Unity**: Unity Personal/Plus/Pro 라이선스 준수

## 🤝 기여하기
1. **Fork** 프로젝트
2. **Feature branch** 생성 (`git checkout -b feature/AmazingFeature`)
3. **Commit** 변경사항 (`git commit -m 'Add some AmazingFeature'`)
4. **Push** 브랜치 (`git push origin feature/AmazingFeature`)
5. **Pull Request** 생성

## 📞 문의
- **이슈 리포트**: GitHub Issues
- **기능 제안**: GitHub Discussions
- **버그 리포트**: 상세한 재현 단계와 함께

---

**마지막 업데이트**: 2024년 7월 21일  
**버전**: 1.0.0  
**Unity 버전**: 2022.3 LTS  
**진행률**: 99% 완료 (Unity 에디터 설정만 남음) 
