using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// This class holds information about a place.
    /// Code taken from: https://github.com/icebeam7/MapDemoNetMaui/tree/bindable-behavior
    /// </summary>
    public class Place
    {
        public Location Location { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
