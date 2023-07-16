namespace GrowOps.Models
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public class CommandPayload
  {
    public Dictionary<string, string> Payload { get; private set; } = new Dictionary<string, string>();

    public void SetPayload(string value)
    {
      Payload["value"] = value;
    }
  }
}
