using System.ComponentModel;

namespace GrowOps.Models
{
    public class UserPreferences : INotifyPropertyChanged
    {
        public bool SaveLogin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
