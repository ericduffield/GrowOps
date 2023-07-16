using System;
using System.ComponentModel;
using GrowOps.Interfaces;

namespace GrowOps.Models
{
    public class User : IHasUKey, INotifyPropertyChanged
    {
        public string Key { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Role { get; set; }
        public UserPreferences Preferences { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

