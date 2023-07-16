using System.ComponentModel;
using GrowOps.Interfaces;

namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public class SecuritySubsystem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Reading Luminosity { get; set; }
        public Reading Motion { get; set; }
        public Reading DoorState { get; set; }
        public Reading Noise { get; set; }
        public Actuator Buzzer { get; set; }
        public Actuator DoorLock { get; set; }

        public List<Reading> Readings { get; set; } = new List<Reading>();

        public SecuritySubsystem(Reading luminosity, Reading motion, Reading doorState, Reading noise, Actuator buzzer, Actuator doorLock)
        {
            Luminosity = luminosity;
            Motion = motion;
            DoorState = doorState;
            Noise = noise;
            Buzzer = buzzer;
            DoorLock = doorLock;
            Readings.Add(Luminosity);
            Readings.Add(Motion);
            Readings.Add(DoorState);
            Readings.Add(Noise);
        }

        public async Task<bool> CreateAndSendCommandAsync(Actuator actuator, string type, string value)
        {
            try
            {
                if (type != actuator.TargetType) throw new ArgumentException($"Type must be the same type as actuator type, received: {type} needed: {actuator.TargetType}");
                CommandPayload payload = new();
                payload.SetPayload(value);
                ActuatorCommand aCommand = new ActuatorCommand(type, payload);
                return await actuator.SendCommand(aCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Set(Reading reading)
        {
            switch (reading?.Type)
            {
                case Enums.Type.LUMINANCE:
                    Luminosity = reading;
                    break;
                case Enums.Type.MOTION:
                    Motion = reading;
                    break;
                case Enums.Type.DOOR_STATE:
                    DoorState = reading;
                    break;
                case Enums.Type.NOISE:
                    Noise = reading;
                    break;
            }
        }
    }
}
