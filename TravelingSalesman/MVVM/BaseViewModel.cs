using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravelingSalesman.MVVM;

internal class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool isBusy;
    public bool IsBusy
    {
        get { return isBusy; }
        set
        {
            isBusy = value;
            OnPropertyChanged();
        }
    }
}
