using System.ComponentModel;

namespace WpfApp1.Mvvm
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        internal void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
