# Character Select Button 프리팹

## 개념
캐릭터 선택 화면에서 각 캐릭터를 선택할 수 있는 UI 버튼 프리팹입니다. 캐릭터 아이콘, 이름, 선택 상태 등을 표시할 수 있습니다.

## 용도
- 캐릭터 선택 씬의 캐릭터 선택 버튼
- 캐릭터 아이콘, 이름, 선택/비활성화 상태 표시
- 클릭 시 캐릭터 선택 이벤트 연결

## 구성 예시
- Button(UI 버튼)
- Image(캐릭터 아이콘)
- Text 또는 TextMeshPro(캐릭터 이름)
- CharacterSelectButton.cs(상태/이벤트 제어 스크립트)

## 활용
- CharacterSelectManager에서 동적으로 Instantiate
- 선택/비활성화/하이라이트 등 상태 제어 