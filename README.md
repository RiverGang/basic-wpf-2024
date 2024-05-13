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
- WPF에서 중요한 개념(윈폼과의 차이점)
    1. 데이터 바인딩 - 바인딩 키워드로 코드와 분리
    2. 옵저버패턴 - 값이 변경된 사실을 사용자에게 공지 (OnPropertyChanged 이벤트)
    3. 디자인리소스 - 각 컨트롤마다 디자인(X), 리소스로 디자인 공유
        - 각 화면당 Resources: 해당 화면에만 적용가능 디자인
        - App.xaml Resources: 애플리케이션 전체 적용가능 디자인
            - 리소스사전(Dictionary):공유할 디자인 내용이 많을 때, 독립적인 파일로 따로 지정

- WPF MVVM
    - MVC(Moder View Controller 패턴)
        - 웹개발(Spring, ASP.NET, dJango, etc...) 현재도 사용됨
        - Model: Data입출력 처리 담당
        - View: 디스플레이 화면 담당 순수 xaml로만 구성
        - Controller: View를 제어, Model 처리 중앙에 관장

    - MVVM(Model View ViewModel)
        - Modael: Data 입출력(DB side), 뷰에 제공할 데이터...
        - View: 화면, 순수 xaml로만 구성
        - ViewModel : 뷰에 대한 메서드, 액션, INotifyPropertyChanged를 구현

    ![MVVM패턴](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/wpf001.png)

    - 권장 구혀낭법
        - Caliburn.Toolkit - 3rd Party 개발 . 2009년부터 시작 2014년도 이후 더이상 개발이나 지원이 없음
        - **Caliburn.Micro** - 3rd Party 개발, MVVM이 아주 간단/강력. 중소형 프로젝트에 적합, 디버깅이 조금 어려움
        - AvaloniaUI:  3rd Party 개발, 크로스플랫폼, 디자인 최고
        - Prism Mcrosoft 개발, 지막지하게 어렵다. 대구묘 프로젝트 활용 

- Caliburn.Micro
    1. 프로젝트 생성 후 MainWindow.xaml 삭제
    2. Models, Views, ViewModels 폴더(네임스페이스) 생성
    3. 종속성 NuGet패키지 Caliburn.Micro 설치
    4. 루트 폴더에 Bootstrapper.cs 클래스 생성
    5. App.xaml에서 StartupUri 삭제
    6. App.xaml에 Bootstrapper 클래스를 리소스사전에 등록
    7. App.xaml.cs에 App() 생성자 추가
    8. ViewModels 폴더에 MainViewModel.cs 클래스 생성
    9. Bootstrapper.cs에 OnStartup()에 내용을 변경
    10. View 폴더에 MainView.xaml 생성

    - 작업(3명) 분리
        - DB 개발자 - DBMS 테이블 생성, Models에 클래스 작업
        - Xaml디자이너 - Views 폴더에 있는 xaml 파일 디자인작업
    
## 4일차
- Caliburn.Micro
    - 작업 분리
        - Xaml디자이너: xaml 파일만 디자인
        - ViewModel개발자: Model에 있는 DB 관련 정보와 View와 연계 전체구현 작업

    - Caliburn.Micro 특징
        - Xmal 디자인시 {Binding ...} 잘 사용하지 않음
        - 대신 x:Name을 사용

    - MVVM 특징
        - 예외발생 시 예외메세지 표시없이 프로그램 종료
        - ViewModel에서 디버깅 시작
        - View.xaml 바인딩, 버튼클릭 이름(ViewModel 속성, 메서드) 지정 주의
        - Model내 속성 DB 테이블 컬럼 이름 일치, CRUD 쿼리문 오타 주의
        - ViewModel 부분
            - 변수, 속성으로 분리
            - 속성이 Model내의 속성과 이름이 일치
            - List 사용불가 -> BindableCollrection으로 변경
            - 메서드와 이름이 동일한 Can... 프로퍼티 지정 => 버튼 활성/비활성화
            - 모든 속성에 NotifyOfPropertyChange() 메서드 존재 필수.(값 변경 알림)

    ![실행화면](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/wpf002.png)


## 5일차
- MahApps.Metro (https://mahapps.com/)
    - Metro(Modern UI) 디자인 접목

    ![실행화면](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/wpf003.png)

    ![저장화면](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/wpf004.png)


- Movie API 연동 앱, MovieFinder 2024
    - DB(SQL Server) 연동
    - MahApps.Metro UI
    - CefSharp WebBrowser 패키지
    - Google.Apis 패키지
    - OpenAPI 두가지 사용
    - MVVM 사용x

    - 좋아하는 영화 즐겨찾기 앱
    - [TMDB](https://www.themoviedb.org/) OpenAPI 활용
        - 회원가입 후 API 신청
    - [Youtube API](https://console.cloud.google.com/) 활용
        - 새 프로젝트 생성
        - API 및 서비스, 라이브러리 선택
        - YouTube Data API v3 선택
        - 사용자 인증정보 만들기 클릭
            1. 사용자 데이터 라이오버튼 클릭. 다음
            2. OAutho 동의화면, 기본내용 입력 후 다음
            3. 범위는 저장 후 계속
            4. OAuth Client ID, 앱유형 -> 데스크톱앱, 이름 입력 후 만들기 클릭

## 6일차
- MovieFinder 2024 남은 것
    - 즐겨찾기 후 다시 선택 즐겨찾기 막기
    - 즐겨찾기 삭제 구현
    - 그리드뷰 영화 더블클릭 시, 영화소개 팝업


## 7일차
- 데이터포털 API 연동앱 예제

## 8일차
- WPF 개인프로젝트 포트폴리오 작업
    ![실행화면](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/pj02.png)
    ![지도화면](https://raw.githubusercontent.com/RiverGang/basic-wpf-2024/main/images/pj03.png)
    
    - 공공데이터포털 API 사용
        - 인천광역시_지진대피소 API (JSON)
        - [OPENAPI](https://www.data.go.kr/data/15065594/openapi.do) 활용
        - JSON DATA 가공
        
    - 기능
        - 옥외 지진대피소 조회
        - 장소분류 코드별 조회기능(콤보박스)
        - DB 저장
        - Google Map 연동 -> 대피장소 위치확인


- 차후 개선
    -  Custom Modern UI Design
        - 색상코드
            #0477BF
            #398CBF
            #03A6A6
            #595858
            #F2F2F2