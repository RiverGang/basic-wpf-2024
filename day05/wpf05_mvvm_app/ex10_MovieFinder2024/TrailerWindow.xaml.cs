using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ex10_MovieFinder2024.models;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls;
using Google.Apis.Services;
using MahApps.Metro.Controls.Dialogs;

namespace ex10_MovieFinder2024
{
    /// <summary>
    /// TrailerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrailerWindow : MetroWindow
    {
        List<YoutubeItem> youtubeItems = null; // 유튜브API 검색 결과 담을 객체리스트

        public TrailerWindow()

        {
            InitializeComponent();
        }
        
        // MainWindow 그리드에서 선택된 영화제목 넘기면서 생성
        // 재정의 생성자

        public TrailerWindow(string movieName) : this() 
        {
            // 재정의 생성자
            // this() => TrailerWindow() 생성자를 먼저 실행한 뒤, 현재 생성자 실행
            LblMovieName.Content = $"{movieName} 예고편";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            youtubeItems = new List<YoutubeItem>(); // 초기화
            SearchYoutubeApi(); // 핵심메서드 실행
        }

        private async void SearchYoutubeApi()
        {
            await LoadDataCollection();
            LsvResult.ItemsSource = youtubeItems;
            LsvResult.SelectedIndex = 0;
        }

        private async Task LoadDataCollection()
        {
            // YoutubeService용 패키지 다운로드
            var service = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyAz16upAC00VGZQMHRpJ8KRF3FNf9qNeCU",
                    ApplicationName = this.GetType().ToString()
                }               
            );

            var req = service.Search.List("snippet"); // 유튜브서비스의 코드스니펫
            req.Q = LblMovieName.Content.ToString(); // 검색창에 들어갈 내용
            req.MaxResults = 10; // 보여지는 영상 개수

            var res = await req.ExecuteAsync(); // Youtube 서버에서 요청된 값 실행하고 결과를 리턴 (비동기)

            //await this.ShowMessageAsync("검색결과", res.Items.Count.ToString());
            foreach (var item in res.Items)
            {
                if (item.Id.Kind.Equals("youtube#video")) // 동영상플레이가 가능한 아이템만
                {
                    var youtube = new YoutubeItem()
                    {
                        Title = item.Snippet.Title,
                        ChannelTitle = item.Snippet.ChannelTitle,
                        URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}", // 유튜브 플레이링크
                        Author = item.Snippet.ChannelId
                    };

                    youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute));
                    youtubeItems.Add(youtube); // 앞서 제일 첫 문장에서 만들었던 리스트
                }
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 한번식 CefSharp 브라우저에서 메모리 릭발생
            BrsYoutube.Address = string.Empty;
            BrsYoutube.Dispose(); // 앱 종료시 객체를 완전해제
        }

        private void LsvResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LsvResult.SelectedItem is YoutubeItem) // is 키워드 => true/false
            {
                var video = LsvResult.SelectedItem as YoutubeItem; // as => 형변환(casting), 실패시 null이 넘어옴
                BrsYoutube.Address = video.URL;
            }
        }
    }
}
