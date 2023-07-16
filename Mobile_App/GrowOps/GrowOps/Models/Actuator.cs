namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public class Actuator
  {
    public string TargetType { get; set; }
    public Actuator(string targetType)
    {
      TargetType = targetType;
    }

    public async Task<bool> SendCommand(ActuatorCommand command)
    {
      //Send to IoT Hub specific actuator
      //Mocked for now
      return await Task.FromResult(true);
    }
  }
}
