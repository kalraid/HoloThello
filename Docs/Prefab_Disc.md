# Disc(돌) 프리팹

## 개념
오셀로 보드에 놓이는 돌(흑/백)을 나타내는 프리팹입니다. 게임 플레이 중 플레이어 또는 CPU가 돌을 놓을 때 동적으로 생성됩니다.

## 용도
- 오셀로 보드의 각 셀에 배치되는 돌 오브젝트
- 흑/백 색상(기본), 애니메이션, 이펙트 등 시각적 효과 포함 가능
- 클릭, 하이라이트, 애니메이션 등 상호작용 지원

## 구성 예시
- SpriteRenderer(흑/백 이미지 / 선택한 케릭터 미니이미지(_mini))
- Disc.cs(돌의 상태/색상/애니메이션/이벤트 제어 스크립트)
- (선택) Animator(뒤집기/파괴 애니메이션)
- (선택) ParticleSystem, AudioSource(이펙트/사운드)

---

## 상세 구현 가이드

### 1. 케릭터 미니이미지(_mini) 오버레이
- Disc 오브젝트의 자식으로 "MiniImage" GameObject를 추가합니다.
- MiniImage에 SpriteRenderer를 추가하고, SortingOrder를 돌보다 위로 설정합니다.
- Disc.cs에서 선택한 캐릭터의 _mini 이미지를 MiniImage에 할당합니다.
- 돌이 흑/백일 때 각각의 내부에 미니이미지가 잘 보이도록 마스크/알파 조정(흑/백 대비 유지).

### 2. 뒤집기(플립) 연출
- **Animator/Animation 방식**
  - Disc 프리팹에 Animator를 추가하고, "Flip" 애니메이션(1초, 10프레임 분할)을 만듭니다.
  - Y축 0→180도 회전, 중간(0.5초, 5프레임)에서 Sprite/색상/미니이미지 교체.
  - Disc.cs에서 FlipDisc() 함수로 애니메이션 트리거.
- **코루틴 방식**
  - Disc.cs에서 코루틴으로 1초 동안 10프레임(0.1초 간격)씩 Y축 회전.
  - 0.5초(5프레임)에서 Sprite/색상/미니이미지 교체.
- FlipDisc() 함수는 BoardManager에서 돌을 뒤집을 때 호출.

### 3. 5배수마다 FlipEffect 이펙트 처리
- 뒤집힌 돌 개수가 5, 10, 15, ... 60개 등 5배수일 때 특별한 이펙트/사운드 연출.
- FlipEffect 프리팹(파티클, 사운드 등)을 여러 종류로 준비(5, 10, 15, ...).
- Disc.cs에서 FlipDisc() 실행 시 현재 뒤집힌 돌 개수를 인자로 받아, 5배수일 때 해당 FlipEffect 프리팹을 Instantiate.
- 예시: `if (flippedCount % 5 == 0) Instantiate(flipEffect5x, transform.position, Quaternion.identity);`

### 4. 돌 파괴 연출
- 스킬 등으로 돌이 파괴될 때 파괴 애니메이션(Animator/ParticleSystem) 실행.
- 파괴 후 Destroy(gameObject)로 오브젝트 제거.
- 필요시 파괴 이펙트 프리팹 별도 준비.

### 5. Disc.cs 주요 함수 예시
```csharp
public class Disc : MonoBehaviour
{
    public SpriteRenderer mainRenderer; // 흑/백
    public SpriteRenderer miniRenderer; // 케릭터 미니이미지
    public Animator animator; // (선택)
    public ParticleSystem[] flipEffects; // 5배수 이펙트들

    public void SetDisc(bool isBlack, Sprite miniSprite)
    {
        // 흑/백 이미지 설정
        // miniRenderer.sprite = miniSprite;
    }

    public void FlipDisc(int flippedCount)
    {
        // Animator 방식
        animator.SetTrigger("Flip");
        // 코루틴 방식
        // StartCoroutine(FlipCoroutine());

        // 5배수 이펙트
        if (flippedCount % 5 == 0)
        {
            int idx = Mathf.Clamp(flippedCount / 5 - 1, 0, flipEffects.Length - 1);
            Instantiate(flipEffects[idx], transform.position, Quaternion.identity);
        }
    }

    public void DestroyDisc()
    {
        // 파괴 이펙트, 애니메이션
        // Destroy(gameObject, 0.5f);
    }
}
```

---

## 활용
- BoardManager에서 Instantiate로 동적으로 생성
- 게임판 상태에 따라 색상/애니메이션/미니이미지/이펙트 변경
- 뒤집기, 파괴, 5배수 이펙트 등 다양한 연출 지원

