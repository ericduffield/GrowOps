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
    /// </summary>
    public class PlantSubsystem : INotifyPropertyChanged
    {
        public Reading Temperature { get; set; }
        public Reading Humidity { get; set; }
        public Reading RelativeWaterLevel { get; set; }
        public Reading SoilMoistureLevel { get; set; }
        public Reading Longitude { get; set; }
        public double TemperatureThreshold { get; set; }
        public Actuator Fan { get; set; }
        public Actuator Light { get; set; }
        public bool FanState { get; set; }

        public List<Reading> Readings { get; set; } = new List<Reading>();


        public PlantSubsystem(Reading temperature, Reading humidity, Reading relativewaterlevel, Reading soilmoisturelevel, double temperatureThreshold, Actuator fan, Actuator light, bool fanState)
        {
            Temperature = temperature;
            Humidity = humidity;
            RelativeWaterLevel = relativewaterlevel;
            SoilMoistureLevel = soilmoisturelevel;
            TemperatureThreshold = temperatureThreshold;
            Fan = fan;
            Light = light;
            FanState = fanState;
            Readings.Add(Temperature);
            Readings.Add(Humidity);
            Readings.Add(RelativeWaterLevel);
            Readings.Add(SoilMoistureLevel);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void Set(Reading reading)
        {
            switch (reading?.Type)
            {
                case Enums.Type.TEMPERATURE:
                    Temperature = reading;
                    break;
                case Enums.Type.HUMIDITY:
                    Humidity = reading;
                    break;
                case Enums.Type.WATER_LEVEL:
                    RelativeWaterLevel = reading;
                    break;
                case Enums.Type.SOIL_MOISTURE:
                    SoilMoistureLevel = reading;
                    break;
            }
        }
    }
}
