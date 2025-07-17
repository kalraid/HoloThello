# 홀로 격투 오셀로 프로젝트

## 소개
- 턴제 방식 HP 게임
- 오셀로의 최종목표는 가장 많은 돌을 뒤집어 두는거지만
  홀로 격투 오셀로는 상대방 돌을 뒤집으면 데미지를 전달하여 상대방의 HP를 깍는게 목표
- 더 둘곳이 없을때 많은 돌만큼 피니시

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
