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
using ex11_Gimhae_FindDust.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ex11_Gimhae_FindDust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitComboDateFromDB();
            BtnReqRealtime_Click(sender, e);
        }

        private void InitComboDateFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.PlaceInfo.GETDATE_QUERY, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dSet = new DataSet();
                adapter.Fill(dSet);
                List<string> saveDates = new List<string>();

                foreach (DataRow row in dSet.Tables[0].Rows)
                {
                    saveDates.Add(row["Facilities_id"].ToString());
                }
                CboReqDate.ItemsSource = saveDates;
            }
        }

        // 실시간조회버튼 클릭
        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
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
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
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
                    if(Convert.ToString(item["properties"]["facilities_id"]) == Helpers.Common.SelectIndex)
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
            StsResult.Content = $"OpenAPI {placeinfo.Count}건 조회완료!";
            
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("경고", "실시간 조회 후 저장하세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (PlaceInfo item in GrdResult.Items)
                    {
                        SqlCommand cmd = new SqlCommand(Models.PlaceInfo.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Sgg", item.Sgg);
                        cmd.Parameters.AddWithValue("@Address", item.Address);
                        cmd.Parameters.AddWithValue("@Xcoord", item.Xcoord);
                        cmd.Parameters.AddWithValue("@Ycoord", item.Ycoord);
                        cmd.Parameters.AddWithValue("@Accpt_area", item.Accpt_area);
                        cmd.Parameters.AddWithValue("@Accpt_num", item.Accpt_num);
                        cmd.Parameters.AddWithValue("@Facilites_nm", item.Facilites_nm);
                        cmd.Parameters.AddWithValue("@Facilities_id", item.Facilities_id);
                        cmd.Parameters.AddWithValue("@Objectid", item.Objectid);
                        
                        insRes += cmd.ExecuteNonQuery();
                    }
                    
                    if (insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "DB저장성공!");
                        StsResult.Content = $"DB저장 {insRes}건 성공!";
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류 {ex.Message}");
            }


            InitComboDateFromDB();
        }

        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helpers.Common.SelectIndex = CboReqDate.SelectedValue.ToString();

        }

        // 지도에 위치 표시
        private void GrrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as PlaceInfo;

            var mapWindow = new MapWindow(curItem.Ycoord, curItem.Xcoord);
            mapWindow.Owner = this;
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();
        }
    }
}