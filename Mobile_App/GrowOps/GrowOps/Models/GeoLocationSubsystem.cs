using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// This class holds readings for the GeoLocationSubsystem
    /// </summary>
    public class GeoLocationSubsystem : INotifyPropertyChanged
    {
        public Reading VibrationLevel { get; set; }
        public Reading Pitch { get; set; }
        public Reading Roll { get; set; }
        public Reading Latitude { get; set; }
        public Reading Longitude { get; set; }
        public Reading BuzzerState { get; set; }
        public List<Reading> Readings { get; set; } = new List<Reading>();

        public GeoLocationSubsystem(Reading vibrationLevel, Reading pitch, Reading roll, Reading latitude, Reading longitude, Reading buzzerState)
        {
            VibrationLevel = vibrationLevel;
            Pitch = pitch;
            Roll = roll;
            Latitude = latitude;
            Longitude = longitude;
            BuzzerState = buzzerState;
            Readings.Add(VibrationLevel);
            Readings.Add(Pitch);
            Readings.Add(Roll);
            Readings.Add(Latitude);
            Readings.Add(Longitude);
            Readings.Add(BuzzerState);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Set(Reading reading)
        {
            switch (reading?.Type)
            {
                case Enums.Type.VIBRATION:
                    VibrationLevel = reading;
                    break;
                case Enums.Type.PITCH:
                    Pitch = reading;
                    break;
                case Enums.Type.ROLL:
                    Roll = reading;
                    break;
                case Enums.Type.LATITUDE:
                    Latitude = reading;
                    break;
                case Enums.Type.LONGITUDE:
                    Longitude = reading;
                    break;
                case Enums.Type.BUZZER:
                    BuzzerState = reading;
                    break;
            }
        }
    }
}
