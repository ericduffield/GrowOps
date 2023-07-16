/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Settings
/// </summary>

namespace GrowOps.Config
{
  public class Settings
  {
    public string EventHubConnectionString { get; set; }
    public string EventHubName { get; set; }
    public string ConsumerGroup { get; set; }
    public string StorageConnectionString { get; set; }
    public string BlobContainerName { get; set; }
    public string HubConnectionString { get; set; }
    public string DeviceId { get; set; }
    public string DeviceConnectionString { get; set; }
  }
}
