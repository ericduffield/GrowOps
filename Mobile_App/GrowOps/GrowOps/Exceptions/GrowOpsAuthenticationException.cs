namespace GrowOps.Exceptions
/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// </summary>
{
    public class GrowOpsAuthenticationException : Exception
  {
    public string Message { get; set; }
    public GrowOpsAuthenticationException(string message)
    {
      Message = message;
    }
  }
}
