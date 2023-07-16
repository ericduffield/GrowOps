using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowOps.Models
{
    public class IntWrapper : INotifyPropertyChanged
    {
        public int Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
