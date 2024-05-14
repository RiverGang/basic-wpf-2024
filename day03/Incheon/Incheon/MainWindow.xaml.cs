using CefSharp.DevTools.Network;
using Incheon.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incheon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitGrdResult();

        }

        private async void InitGrdResult()
        {
            string openApiUri = "https://smart.incheon.go.kr/server/rest/services/Hosted/%EC%A7%80%EC%A7%84%EB%8C%80%ED%94%BC%EC%86%8C_%EC%A0%95%EB%B3%B4/FeatureServer/204/query?outFields=*&where=1%3D1&f=geojson";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = await reader.ReadToEndAsync();

                //await this.ShowMessageAsync("결과", result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var data = jsonResult["features"];
            var jsonArray = data as JArray; // json 자체에서 [] 안의 배열데이터만 JArray 변환가능

            var placeinfo = new List<PlaceInfo>();
            if (string.IsNullOrEmpty(Helpers.Common.SelectIndex)) // Cbo 선택 없을 시, 전체 조회
            {
                foreach (var item in jsonArray)
                {
                    placeinfo.Add(new PlaceInfo()
                    {
                        Id = 0,
                        Xcoord = Convert.ToDouble(item["geometry"]["coordinates"][0]),
                        Sgg = Convert.ToString(item["properties"]["sgg"]),
                        Address = Convert.ToString(item["properties"]["address"]),
                        Accpt_area = Convert.ToInt32(item["properties"]["accpt_area"]),
                        Accpt_num = Convert.ToInt32(item["properties"]["accpt_num"]),
                        Facilites_nm = Convert.ToString(item["properties"]["facilites_nm"]),
                        Facilities_id = Convert.ToString(item["properties"]["facilities_id"]),
                        Ycoord = Convert.ToDouble(item["geometry"]["coordinates"][1]),
                        Objectid = Convert.ToInt32(item["properties"]["objectid"])
                    });
                }
            }
            else // Cbo 선택 값 있을 시, 일치 값만 조회
            {
                foreach (var item in jsonArray)
                {
                    if (Convert.ToString(item["properties"]["facilities_id"]) == Helpers.Common.SelectIndex)
                        placeinfo.Add(new PlaceInfo()
                        {
                            Id = 0,
                            Xcoord = Convert.ToDouble(item["geometry"]["coordinates"][0]),
                            Sgg = Convert.ToString(item["properties"]["sgg"]),
                            Address = Convert.ToString(item["properties"]["address"]),
                            Accpt_area = Convert.ToInt32(item["properties"]["accpt_area"]),
                            Accpt_num = Convert.ToInt32(item["properties"]["accpt_num"]),
                            Facilites_nm = Convert.ToString(item["properties"]["facilites_nm"]),
                            Facilities_id = Convert.ToString(item["properties"]["facilities_id"]),
                            Ycoord = Convert.ToDouble(item["geometry"]["coordinates"][1]),
                            Objectid = Convert.ToInt32(item["properties"]["objectid"])
                        });
                }
            }

            this.DataContext = placeinfo;
            //StsResult.Content = $"OpenAPI {placeinfo.Count}건 조회완료!";

        }

        private void BtnClosing_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}