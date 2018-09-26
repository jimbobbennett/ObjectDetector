using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ObjectDetector
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        bool isEnabled = true;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (Set(ref isEnabled, value))
                    OnPropertyChanged(nameof(IsBusy));

            }
        }
        public bool IsBusy => !IsEnabled;
    }
}
