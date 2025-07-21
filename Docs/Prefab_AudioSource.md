# Audio Source 프리팹

## 개념
효과음, 배경음 등 동적으로 재생되는 오디오를 위한 프리팹입니다. 사운드 이펙트, 스킬/이벤트/버튼 클릭 등 다양한 상황에서 사용됩니다.

## 용도
- 효과음, 배경음, 스킬/이펙트 사운드 등 동적 오디오 재생
- AudioManager, EffectManager 등에서 Instantiate로 생성
- 3D/2D 사운드, 볼륨, 반복 등 다양한 설정 가능

## 구성 예시
- AudioSource(오디오 재생 컴포넌트)
- AudioSourceController.cs(사운드 제어 스크립트, 필요시)

## 활용
- AudioManager에서 Instantiate로 동적으로 생성
- 사운드 재생 후 자동 파괴 또는 오브젝트 풀링 