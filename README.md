# basic-wpf-2024
IoT 개발자 과정 WPF 학습리포지토리


## 1일차
- WPF 기본학습
    - winforms 확장한 WPF
        - 이전의 Winforms는 이미지 비트맵방식(2D)
        - WPF 이미지 벡터방식
        - XAML 화면 디자인 - 안드로이드 개발시 Java XML로 화면디자인과 PyQt로 디자인과 동일
        
    - XAML
        - <Window> 여는 태그, </Window> 닫는 태그
        - 합치면 <Window /> - Window 태그 안에 다른객체가 없다는 뜻 
        - 여는 태그와 닫는 태그사이에 다른 태그(객체)를 넣어서 디자인

    - WPF 기본 사용법
        - Winforms와는 다르게 코딩으로 디자인

    - 레이아웃
        1. Canvas:미술에서의 캔버스와 거의 유사
        2. StackPanel: 컨트롤을 쌓아나가는 형태
        3. DockPanel: 컨트롤을 방향에 따라 도킹시키는 레이아웃
        4. Grid: WPF에서 가장 많이 사용하는 레이아웃
        5. Margin: 여백과 앵커링 기능(중요!)


## 2일차
- WPF 기본학습
    - 데이터바이딩: 데이터소스(DB, 엑셀, txt, 클라우드 등에 보관되어있는 데이터원본)의 데이터를 쉽게 가져다쓰기 위한 데이터 핸들링방법
        - xaml: {Binding Path= 속성, ElementName=객체, Mode=(OneWay|TwoWay), StringFormat={}{0:#.#}}
        - dataContext: 데이터를 담아서 전달하는 이름
        - 전통적인 윈폼 코드비하인드에서 데이터를 처리하는 것을 지양 -> 최종적으로 디자인과 개발 부분을 분리하는 것을 목적으로 함

## 3일차
- WPF 기본학습
    - 데이터바인딩 마무리
    - 디자인 리소스
- WPF MVVM




    - 디자인 코딩방법
    - 디자인, C#코드 완전분리 개발: MVVM 디자인패턴
