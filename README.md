## [Running game using **Face recognition**](https://github.com/yohanii/graphics_termproject)

**21.05.21 ~ 21.06.11**

**2인 개발 / Unity 2020.3.9f1, OpenCV 4.5.2**

### Summary

---

**모바일 게임 ‘Temple Run’을 모티브로 유니티 엔진을 사용해 개발한 게임 프로젝트**입니다. OpenCV을 이용해 웹캠을 통한 얼굴인식으로 시점 이동 및 캐릭터 방향 전환을 할 수 있게 했습니다. 우주선에서 장애물을 피하며, 쫓아오는 좀비를 피해 달아나는 컨셉으로 제작하였습니다. 

![Untitled (5)](https://user-images.githubusercontent.com/33834623/174596789-7f21824d-1284-4ce1-8d07-5a689bdbf24e.png)

### 게임 구성

---

![Untitled (6)](https://user-images.githubusercontent.com/33834623/174596805-3c5f5800-5272-4310-8aac-46e07e778ccf.png)

맵 랜던 생성 / 파괴

![Untitled (7)](https://user-images.githubusercontent.com/33834623/174596819-ceaf3470-5863-4fbe-835d-c993bf61588f.png)


장애물 랜덤 생성

![Untitled (8)](https://user-images.githubusercontent.com/33834623/174596832-e775536a-cdc6-473d-b164-4708575732c1.png)

캐릭터(무료 에셋)

![Untitled (9)](https://user-images.githubusercontent.com/33834623/174596846-401e9adc-6c07-4ff1-a6ed-776752a629f6.png)


라이트

![Untitled (10)](https://user-images.githubusercontent.com/33834623/174596925-33352723-2309-4443-915e-3631483fdc6a.png)

살아남은 시간에 따라 증가하는 점수

![Untitled (11)](https://user-images.githubusercontent.com/33834623/174596938-54e860a3-851c-494b-8f3d-b5690f57d16e.png)

장애물 충돌 효과


![Untitled (12)](https://user-images.githubusercontent.com/33834623/174596949-210a00b7-082f-49e9-ac4c-c1b171033391.png)

죽음 시 메뉴


![Untitled (13)](https://user-images.githubusercontent.com/33834623/174596953-8f2438c3-6040-4ce5-837a-479ea2a0ac83.png)

카메라 회전

### 개발 과정

---

1. 충돌 관리

캐릭터가 벽을 통과하거나 바닥에 빠지는 현상이 발생했습니다. 장애물과 자연스러운 충돌이 일어나지 않는 문제가 생기는 등 충돌 시 발생하는 문제를 해결해야 했습니다. 그래서 플레이어와 장애물과 벽에 rigid body를 추가시켜 물리 제어를 받도록 해서 해결했습니다.

1. 적 움직임

초기에는 쫓아오는 적의 움직임이 단조로웠습니다. 플레이어의 위치에 상관없이 길의 중앙으로만 움직이고 캐릭터 회전시 잠깐 사라진 뒤 뒤에서 다시 추적하도록 구현했으나 긴장감을 전혀 유발하지 못해 개선할 필요가 있었습니다. 그래서 실시간으로 플레이어의 위치를 따라오도록 플레이어의 좌표 쪽으로 적의 방향과 이동 방향을 바꿔주어 개선했습니다. 매 프레임마다 적의 방향과 이동 방향을 바꿔주다보니 렉이 발생했고, 일정 시간 이상 지나면 바꿔주도록 바꿔 해결했습니다.
