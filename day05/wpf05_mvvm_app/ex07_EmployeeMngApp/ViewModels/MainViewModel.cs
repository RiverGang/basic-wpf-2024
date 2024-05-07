using Caliburn.Micro;
using ex07_EmployeeMngApp.Helpers;
using ex07_EmployeeMngApp.Models;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ex07_EmployeeMngApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private IDialogCoordinator _dialogCoordinator;
        // 멤버 변수
        private int id;
        private string empName;
        private decimal salary;
        private string deptName;
        private string addr;

        private BindableCollection<Employees> listEmployees;

        private Employees selectedEmployee;
        
        public Employees SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                // 데이터를 TextBox들에 전달
                if(selectedEmployee != null)
                {
                    Id = value.Id;
                    empName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Addr = value.Addr;

                    NotifyOfPropertyChange(() => SelectedEmployee); // View에 데이터가 표시되려면 필수!
                    NotifyOfPropertyChange(() => Id);
                    NotifyOfPropertyChange(() => EmpName);
                    NotifyOfPropertyChange(() => Salary);
                    NotifyOfPropertyChange(() => DeptName);
                    NotifyOfPropertyChange(() => Addr);
                }
            }
        }

        // 속성
        public int Id
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(()=> Id);
                NotifyOfPropertyChange(()=> CanDelEmployee); // 삭제여부 속성도 변경했다 공지
            }
        }
        public string EmpName
        {
            get => empName; 
            set
            {
                empName = value;
                NotifyOfPropertyChange(()=> EmpName);
                NotifyOfPropertyChange(()=> CanSaveEmployee);
            }
        }
        public decimal Salary
        {
            get => salary; set
            {
                salary = value;
                NotifyOfPropertyChange(()=> Salary);
                NotifyOfPropertyChange(()=> CanSaveEmployee);
            }
        }
        public string DeptName
        {
            get => deptName; 
            set
            {
                deptName = value;
                NotifyOfPropertyChange(()=> DeptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }
        public string Addr
        {
            get => addr; 
            set
            {
                addr = value;
                NotifyOfPropertyChange(()=> Addr);
            }
        }


        // List는 정적인 컬렉션. 따라서 MVVM처럼 동적으로 데이터를 받아오는 경우 사용불가
        // 동적으로 처리하는 경우, BindableCollection 사용
        public BindableCollection<Employees> ListEmployees
        {
            get => listEmployees;
            set
            {
                listEmployees = value;
                // 값이 변경된 것을 시스템에 알려줌
                NotifyOfPropertyChange(() => ListEmployees); // 필수
            }
        }

        public MainViewModel()
        {
            DisplayName = "직원관리 시스템";
            // 조회 실행
            GetEmployees();
        }



        /// <summary>
        /// Caliburn.Micro가 Xaml의 버튼 x:Name과 동일한 이름의 메서드로 매핑시켜줌
        /// </summary>
        public async void SaveEmployee()
        {
            if(Helpers.Common.DialogCoordinator != null)
            {
                this._dialogCoordinator = Common.DialogCoordinator;
            }

            //MessageBox.Show("저장버튼 동작!");
            using(SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if(Id == 0) // INSERT
                    cmd.CommandText = Models.Employees.INSERT_QUERY;
                else
                    cmd.CommandText = Models.Employees.UPDATE_QUERY;

                SqlParameter prmEmpName = new SqlParameter("@EmpName", EmpName);
                cmd.Parameters.Add(prmEmpName);
                SqlParameter prmSalary = new SqlParameter("@Salary", Salary);
                cmd.Parameters.Add(prmSalary);
                SqlParameter prmDeptName = new SqlParameter("@DeptName", DeptName);
                cmd.Parameters.Add(prmDeptName);
                SqlParameter prmAddr = new SqlParameter("@Addr", Addr ?? (object)DBNull.Value); // 빈 값을 DB컬럼에 null값으로 입력
                cmd.Parameters.Add(prmAddr);

                if(Id != 0) //업데이트면 Id 파라미터가 필요
                {
                    SqlParameter prmId = new SqlParameter("@Id", Id);
                    cmd.Parameters.Add(prmId);
                }

                var result = cmd.ExecuteNonQuery();

                if(result >0)
                {
                    //MessageBox.Show("저장성공");
                    await this._dialogCoordinator.ShowMessageAsync(this, "저장 성공", "저장");
                }
                else
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "저장 실패", "저장");
                }

                GetEmployees(); // 그리드 재조회
                NewEmployee(); // 입력 초기화
            }
        }

        public void GetEmployees()
        {
            using(SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.Employees.SELECT_QUERY, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListEmployees = new BindableCollection<Employees>();

                while(reader.Read())
                {
                    ListEmployees.Add(new Employees()
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Addr =  reader["Addr"].ToString()

                    });
                }
                
            }
        }

        
        // 삭제버튼을 누를 수 있는지 여부확인
        public bool CanDelEmployee
        {
            get { return Id != 0; } // TextBox Id 속성의 값이 0이면 false, 아니면 true 반환
        }

        public bool CanSaveEmployee
        {
            get
            {
                if (string.IsNullOrEmpty(EmpName) | Salary == 0 | string.IsNullOrEmpty(DeptName))
                    return false;
                else 
                    return true;
            }
        }
        public async void DelEmployee()
        {
            if (Helpers.Common.DialogCoordinator != null)
            {
                this._dialogCoordinator = Common.DialogCoordinator;
            }

            if (Id == 0)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "삭제불가", "삭제");
                return;
            }

            var val = await this._dialogCoordinator.ShowMessageAsync(this, "삭제하시겠습니까?", "삭제여부", MessageDialogStyle.AffirmativeAndNegative);
            if(val == MessageDialogResult.Negative)
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection (Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.Employees.DELETE_QUERY, conn);
                SqlParameter prmId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(prmId);

                var res = cmd.ExecuteNonQuery();
                if (res >=0){ await this._dialogCoordinator.ShowMessageAsync(this, "삭제 성공", "삭제"); }
                else { await this._dialogCoordinator.ShowMessageAsync(this, "삭제 실패", "삭제"); }

                GetEmployees();
                NewEmployee();
            }

        }
        public void NewEmployee()
        {
            Id = 0;
            Salary = 0;
            EmpName = DeptName = Addr = "";
        }
    }
}
