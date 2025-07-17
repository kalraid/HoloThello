# Unity 2D 오셀로(리버시) 프로젝트

## 소개
- Unity 2D 엔진으로 제작한 오셀로(리버시) 게임입니다.
- 8x8 보드, 돌 뒤집기, 턴 관리, 승패 판정 등 기본 규칙 구현

## 폴더 구조
```
Assets/
 ├─ Scripts/      # 게임 로직 C# 스크립트
 ├─ Prefabs/      # 돌(디스크) 프리팹
 ├─ Sprites/      # 보드/돌 이미지
 └─ Scenes/       # 샘플 씬 및 안내
```

## 실행(부팅) 방법
1. **Unity Hub에서 프로젝트 폴더(이 디렉토리)를 열기**
2. **SampleScene.unity** 또는 원하는 씬을 열기
3. **필수 리소스 준비**
   - Assets/Sprites/Board.png (8x8 보드)
   - Assets/Sprites/Black.png (흑돌)
   - Assets/Sprites/White.png (백돌)
   - Assets/Prefabs/Disc.prefab (SpriteRenderer+Disc.cs)
4. **Hierarchy에서 BoardManager, GameManager 오브젝트 생성 및 스크립트 연결**
5. **Canvas에 Text 오브젝트 2개(turnText, resultText) 추가 및 GameManager에 연결**
6. **Main Camera 위치 (x=3.5, y=3.5, z=-10)로 조정**
7. ▶️ 버튼으로 실행

## 개발/배포 참고
- .gitignore로 Library, Temp, obj, Build 등 불필요/대용량 파일은 git에 포함되지 않음
- 자세한 TODO 및 개선 계획은 TODO.txt 참고

## 라이선스
- 오픈소스(라이선스 파일 참고)

## 문의/기여
- Pull Request, Issue 환영! 