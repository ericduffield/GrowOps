using Firebase.Auth.Providers;
using Firebase.Auth;
using GrowOps.Config;

namespace GrowOps.Services
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public static class AuthService
  {
    // Configure...
    private static FirebaseAuthConfig config = new FirebaseAuthConfig
    {
      ApiKey = ResourceStrings.Firebase_APIKEY,
      AuthDomain = ResourceStrings.Firebase_AuthorizedDomain,
      Providers = new FirebaseAuthProvider[]
        {
                // Add and configure individual providers
                new EmailProvider()
        },
    };
    // ...and create your FirebaseAuthClient
    public static FirebaseAuthClient Client { get; } = new FirebaseAuthClient(config);
    public static UserCredential UserCreds { get; set; }
  }
}
