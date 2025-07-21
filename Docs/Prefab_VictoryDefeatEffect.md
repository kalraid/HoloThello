# Victory/Defeat 이펙트 프리팹

## 개념
게임에서 승리/패배 시 연출되는 시각적 이펙트(파티클, 애니메이션, 텍스트 등)를 위한 프리팹입니다.

## 용도
- 게임 종료(승리/패배) 시 시각적 효과 연출
- 파티클, 애니메이션, 사운드, 텍스트 등 다양한 연출 가능
- 결과 메시지, 축하/패배 효과 등 포함

## 구성 예시
- ParticleSystem(파티클 효과)
- Animator(애니메이션)
- Text 또는 TextMeshPro(결과 메시지)
- AudioSource(사운드)
- VictoryDefeatEffect.cs(이펙트 제어 스크립트)

## 활용
- GameManager, UIManager 등에서 Instantiate로 동적으로 생성
- 게임 종료 시 자동 재생/파괴 