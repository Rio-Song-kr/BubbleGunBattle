# 게임명 : 버블건 배틀

## 게임 개요

- **게임 장르** : 멀티플레이어 아케이드
- **목표** : 버블건을 사용해 맵에 랜덤으로 생성된 장난감을 버블로 포획하고, 이를 목표 지점으로 굴려 점수를 획득, 가장 높은 점수를 얻은 플레이어가 승리
- **타깃 플랫폼** : PC
- **세부 사항** : Assets/Documents 폴더 참고

## 컨벤션

### 코드 컨벤션

```cs

// 예시
public int PublicField;
public static int MyStaticField;

private int _packagePrivate;
private int _myPrivate;
private static int _myPrivate;

protected int _myProtected;

```

### Git 컨벤션

| Type     | 내용                                                    |
| -------- | ------------------------------------------------------- |
| Feat     | 새로운 기능 추가, 기존의 기능을 요구 사항에 맞추어 수정 |
| Fix      | 기능에 대한 버그 수정                                   |
| Set      | 프로젝트, 기타 환경 설정 등                             |
| Chore    | 그 외 기타 수정                                         |
| Docs     | 문서(주석 수정)                                         |
| Refactor | 기능의 변화가 아닌 코드 리팩터링                        |
| Test     | 테스트 코드 추가/수정                                   |
