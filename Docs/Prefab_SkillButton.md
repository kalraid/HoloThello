# Skill Button 프리팹

## 개념
스킬/궁극기 사용을 위한 UI 버튼 프리팹입니다. 각 스킬별로 아이콘, 쿨타임, 상태 표시 등을 포함할 수 있습니다.

## 용도
- 플레이어/CPU의 스킬, 궁극기 버튼 UI
- 쿨타임, 활성/비활성, 아이콘 등 시각적 상태 표시
- 클릭 시 스킬 발동 이벤트 연결

## 구성 예시
- Button(UI 버튼)
- Image(스킬 아이콘)
- Text 또는 TextMeshPro(쿨타임/상태 표시)
- SkillButton.cs(상태/이벤트 제어 스크립트)

## 활용
- Canvas 하위에 배치, 또는 동적으로 Instantiate
- GameManager, UIManager 등에서 버튼 상태/이벤트 제어 