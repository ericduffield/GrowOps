namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public class ActuatorCommand
  {
    public string Type { get; set; }
    public CommandPayload CommandPayload { get; set; }
    public ActuatorCommand(string type, CommandPayload commandPayload)
    {
      if (!commandPayload.Payload.ContainsKey("value"))
      {
        throw new ArgumentException("Payload must contain key named 'Value'");
      }
      Type = type;
      CommandPayload = commandPayload;
    }
  }
}
