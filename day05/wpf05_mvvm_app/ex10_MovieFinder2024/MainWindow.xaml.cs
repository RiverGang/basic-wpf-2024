﻿using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ex10_MovieFinder2024.models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;

namespace ex10_MovieFinder2024
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool isFavorite = false; // 즐겨찾기 or API 검색 구분: false => openAPI, true=> 즐겨찾기보기
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(TxtMovieName.Text))
            {
               await this.ShowMessageAsync("검색", "검색할 영화명을 입력하세요");
                return;
            }

            SearchMovie(TxtMovieName.Text);

        }

        private async void SearchMovie(string movieName)
        {
            // TNDB 사이트에서 제공받은 API키
            string tmdb_apiKey = "1fa9b0a4a5ab2bba039097ceb713da31";
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";

            //Debug.WriteLine(openApiUri);

            string result = string.Empty; // 결과값

            // openapi 실행 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                //tmdb api 요청
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 URL의 결과를 비동기 응답
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json 결과를 문자열로 저장(처음부터끝까지)

                Debug.WriteLine(result);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                // TODO: 메세지박스로 출력
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result의 string을 json으로 변경
            // json: 딕셔너리 형태와 동일. Key와 Value가 한쌍을 이루는 형태
            var jsonResult = JObject.Parse(result); // type.Parse(string) -> string을 type의 자료형으로 변경
            var total = Int32.Parse(jsonResult["total_results"].ToString()); // total_results 라는 Key의 Value가 숫자이니 int형태로 변환(Convert)
            //await this.ShowMessageAsync("검색수", total.ToString()); // int형의 total을 string으로 변환
            var results = jsonResult["results"];
            var jsonArray = results as JArray; // results를 Json 배열 형태로 변환 -> foreach 사용가능


            var movieItems = new List<MovieItem>(); // MovieItem(models 폴더 내 클래스) Type의 List 선언
            foreach (var item in jsonArray)
            {
                var movieItem = new MovieItem() // 각 요소들 대입
                {
                    Adult = Boolean.Parse(item["adult"].ToString()),
                    Id = Int32.Parse(item["id"].ToString()),
                    Original_Language = item["adult"].ToString(),
                    Original_Title = item["original_title"].ToString(),
                    Overview = item["overview"].ToString(),
                    Popularity = Double.Parse(item["popularity"].ToString()),
                    Poster_Path = item["poster_path"].ToString(),
                    Release_Date = item["release_date"].ToString(),
                    Title = item["title"].ToString(),
                    Vote_Average = Double.Parse(item["vote_average"].ToString()),
                    Vote_Count = Int32.Parse(item["vote_count"].ToString())
                };
                movieItems.Add(movieItem); // 위의 리스트에 요소 추가
            }

            this.DataContext = movieItems;
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
               BtnSearch_Click(sender, e);
            }
        }

        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // 재검색하면 데이터그리 결과가 바뀌면서 이 이벤트가 다시 발생
            try
            {
                var movie = GrdResult.SelectedItem as MovieItem;
                var poster_path = movie.Poster_Path;
                if(string.IsNullOrEmpty(poster_path)) 
                {
                    ImgPoster.Source = new BitmapImage(new Uri("sources/No_Picture.png", UriKind.RelativeOrAbsolute)); 
                }
                else
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{poster_path}", UriKind.Absolute));
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            

            //await this.ShowMessageAsync("포스터", poster_path);

        }

        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 추가");
            if(GrdResult.SelectedItems.Count == 0) 
            {
                await this.ShowMessageAsync("즐겨찾기", "추가할 영화를 선택하세요 (복수선택 가능)");
                return;
            }
            
            var addMovieItems = new List<MovieItem>();
            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                addMovieItems.Add(item);
            }

            Debug.WriteLine(addMovieItems.Count);

            try
            {
                var insRes = 0;
                
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();


                    foreach (var item in addMovieItems)
                    {
                        SqlCommand cmd = new SqlCommand(models.MovieItem.INSERT_QUERY, conn);
                        
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Vote_Count", item.Vote_Count);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);

                        insRes += cmd.ExecuteNonQuery(); // 데이터 하나마다 INSERT쿼리 실행
                    }

                    if (insRes == addMovieItems.Count) { await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 저장성공"); }
                }
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }

        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null; // 데이터그리드에 보낸 데이터를 모두 삭제
            TxtMovieName.Text = string.Empty;


            List<MovieItem> favMovieItems = new List<MovieItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(models.MovieItem.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "MovieItem");

                    foreach (DataRow row in dSet.Tables["MovieItem"].Rows)
                    {
                        var movieItem = new MovieItem()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Title = Convert.ToString(row["Title"]),
                            Original_Title = Convert.ToString(row["Original_title"]),
                            Release_Date = Convert.ToString(row["Release_Date"]),
                            Original_Language = Convert.ToString(row["Original_Language"]),
                            Adult = Convert.ToBoolean(row["Adult"]),
                            Vote_Average = Convert.ToDouble(row["Vote_Average"]),
                            Vote_Count = Convert.ToInt32(row["Vote_Count"]),
                            Poster_Path = Convert.ToString(row["Poster_Path"]),
                            Overview = Convert.ToString(row["Overview"]),
                            Reg_Date = Convert.ToDateTime(row["Reg_Date"])
                        };
                        favMovieItems.Add(movieItem);
                    }

                    this.DataContext = favMovieItems;
                    isFavorite = true; // 즐겨찾기
                    StsResult.Content = $"{favMovieItems.Count}건 조회완료";
                    ImgPoster.Source = new BitmapImage(new Uri("sources/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회오류 {ex.Message}");
            }
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 삭제");

        }

        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("Youtube Trailer", "영화를 선택하세요");
                return;
            }

            if(GrdResult.SelectedItems.Count > 1)
            {
                await this.ShowMessageAsync("Youtube Trailer", "영화를 하나만 선택하세요");
                return;
            }

            var movieName = (GrdResult.SelectedItem as MovieItem).Title; // 부모창에서 자식창으로 정보 넘기기
            var trailerWindow = new TrailerWindow(movieName);
            trailerWindow.Owner = this; // trailerWindow창의 부모는 현재(MainWindow)임을 지정
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 부모창을 기준으로 Center
            trailerWindow.ShowDialog(); // 창 띄우기
        }

    }
}