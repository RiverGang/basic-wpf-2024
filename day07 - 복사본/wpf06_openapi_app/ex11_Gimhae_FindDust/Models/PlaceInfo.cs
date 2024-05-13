using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ex11_Gimhae_FindDust.Models
{
    public class PlaceInfo
    {
        public int Id { get; set; } // 추가생성. DB에 넣을때 사용할 값
        public double Xcoord { get; set; } // 경도
        public string Sgg {  get; set; } // 군구명
        public string Address { get; set; } // 상세주소
        public int Accpt_area { get; set; } // 수용가능 면적
        public int Accpt_num { get; set; } // 수용가능 인원
        public string Facilites_nm { get; set; } // 시설명
        public string Facilities_id { get; set; } // 시설구분
        public double Ycoord { get; set; } // 위도

        public int Objectid { get; set; }

        



        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[PlaceInfo]
                                                                   ([Sgg]
                                                                   ,[Address]
                                                                   ,[Xcoord]
                                                                   ,[Ycoord]
                                                                   ,[Accpt_area]
                                                                   ,[Accpt_num]
                                                                   ,[Facilites_nm]
                                                                   ,[Facilities_id]
                                                                   ,[Objectid])
                                                             VALUES
                                                                   (@Sgg
                                                                   ,@Address
                                                                   ,@Xcoord
                                                                   ,@Ycoord
                                                                   ,@Accpt_area
                                                                   ,@Accpt_num
                                                                   ,@Facilites_nm
                                                                   ,@Facilities_id
                                                                   ,@Objectid)";

        public static readonly string SELECT_QUERY = @"SELECT SELECT [Id]
                                                                      ,[Sgg]
                                                                      ,[Address]
                                                                      ,[Xcoord]
                                                                      ,[Ycoord]
                                                                      ,[Accpt_area]
                                                                      ,[Accpt_num]
                                                                      ,[Facilites_nm]
                                                                      ,[Facilities_id]
                                                                      ,[Objectid]
                                                                  FROM [dbo].[PlaceInfo]
                                                                 WHERE Facilities_id = @Facilities_id";
        

        public static readonly string GETDATE_QUERY = @"SELECT Facilities_id
                                                          FROM [dbo].[PlaceInfo]
                                                         GROUP BY Facilities_id";
    }
}
