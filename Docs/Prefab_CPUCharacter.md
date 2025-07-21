# CPU/2P 캐릭터 프리팹

## 개념
CPU 또는 2P(두 번째 플레이어)가 조작하는 캐릭터를 나타내는 프리팹입니다. 게임 시작 시 동적으로 생성되어, CPU/2P의 위치, 애니메이션, HP 등을 관리합니다.

## 용도
- CPU/2P 캐릭터의 시각적 표현(스프라이트, 애니메이션)
- HP바, 스킬 이펙트 등 UI/이펙트 연동
- AI/입력에 따른 상태 관리

## 구성 예시
- SpriteRenderer(캐릭터 이미지)
- Animator(애니메이션)
- CPUController.cs 또는 PlayerController.cs(상태/AI 관리)
- HPBar(자식 오브젝트)
- (선택) 스킬 이펙트, 사운드 등

## 활용
- GameManager에서 Instantiate로 동적으로 생성
- 캐릭터 선택 결과에 따라 스프라이트/스탯 변경
- AI/입력에 따라 애니메이션, 이펙트 등 제어 