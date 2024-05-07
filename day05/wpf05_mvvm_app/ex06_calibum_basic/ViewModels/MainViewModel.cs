using Caliburn.Micro;

namespace ex06_calibum_basic.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public string Greeting { get { return "헬로, 칼리번!"; } }
    }
}
