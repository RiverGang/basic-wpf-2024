using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex05_wpf_bikeshop
{
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged; // 우리가 만드는 클래스의 프로퍼티 값이 변경되면, 변경되는 것을 알려주는 이벤트핸들러

        // 프로퍼티가 변경됨
        protected void OnPropertyChanged(string propertyName) 
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
