using Caliburn.Micro;
using ex06_calibum_basic.ViewModels;
using System.Windows;

namespace ex06_calibum_basic
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        // MVVM 애플리케이션이 처음시작될 때 이벤트핸들러
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(sender, e);
            DisplayRootViewForAsync<MainViewModel>(); // MainViewModel과 뷰화면을 합쳐서 표시
        }
    }
}
