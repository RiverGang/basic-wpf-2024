﻿

// TMDB API로 넘어온 결과를 담는 객체, List<MovieItem> 형태로 받을 예정
namespace ex10_MovieFinder2024.models
{
    public class MovieItem
    {
        public bool Adult { get; set; }
        public int Id { get; set; } //TMDB키
        public string Original_Language { get; set; }
        public string Original_Title { get; set;}
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string Poster_Path { get; set; }
        public string Release_Date { get; set; }
        public string Title { get; set; }
        public double Vote_Average {  get; set; }
        public int Vote_Count { get; set; }
        public DateTime? Reg_Date { get; set; } // 최초에는 없기때문에 Nullable지정(=> type 뒤에 ? 붙이기)

        // 쿼리파트
        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                            , [Title]
                                                            , [Original_Title]
                                                            , [Release_Date]
                                                            , [Original_Language]
                                                            , [Adult]
                                                            , [Popularity]
                                                            , [Vote_Average]
                                                            , [Vote_Count]
                                                            , [Poster_Path]
                                                            , [Overview]
                                                            , [Reg_Date]
                                                         FROM [MovieItem]";

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[MovieItem]
                                                                  ([Id]
                                                                   ,[Title]
                                                                   ,[Original_Title]
                                                                   ,[Release_Date]
                                                                   ,[Original_Language]
                                                                   ,[Adult]
                                                                   ,[Popularity]
                                                                   ,[Vote_Average]
                                                                   ,[Vote_Count]
                                                                   ,[Poster_Path]
                                                                   ,[Overview]
                                                                   ,[Reg_Date])
                                                             VALUES
                                                                   (@Id
                                                                   ,@Title
                                                                   ,@Original_Title
                                                                   ,@Release_Date
                                                                   ,@Original_Language
                                                                   ,@Adult
                                                                   ,@Popularity
                                                                   ,@Vote_Average
                                                                   ,@Vote_Count
                                                                   ,@Poster_Path
                                                                   ,@Overview
                                                                   ,GETDATE())";

       // public static readonly string UPDATE_QUERY = @"";

        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[MovieItem] WHERE Id=@Id";


    }
}