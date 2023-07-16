using GrowOps.Config;
using GrowOps.Models;
using GrowOps.Services;

namespace GrowOps.Repos
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// This class is a data repo that holds all subsystems
    /// </summary>
    public class DataRepo
    {
        public readonly int[] StateThresholds = new int[] { 30, 50 };

        private PlantSubsystem plantSubsystem;
        private GeoLocationSubsystem geoLocationSubsystem;
        private SecuritySubsystem securitySubsystem;
        private DatabaseService<Reading> readingDb;
        private UserRoleAuthenticationService<User> userDb;
        private int telemetryInterval;
        public bool HasLoggedIn { get; set; } = false;

        public DatabaseService<Reading> ReadingDB
        {
            get
            {
                return readingDb ??= new DatabaseService<Reading>(AuthService.Client.User, nameof(Reading), ResourceStrings.Firebase_DB_URL, nameof(Reading));
            }
        }

        public UserRoleAuthenticationService<User> UserDB
        {
            get
            {
                return userDb ??= new UserRoleAuthenticationService<User>(AuthService.Client.User, nameof(User), ResourceStrings.Firebase_DB_URL, nameof(User));
            }
        }

        public PlantSubsystem PlantSubsystem
        {
            get
            {
                return plantSubsystem ??= new PlantSubsystem(
                    new Reading(0, Enums.Unit.CELCIUS, Enums.Type.TEMPERATURE),
                    new Reading(0, Enums.Unit.DEGREES, Enums.Type.HUMIDITY),
                    new Reading(0, Enums.Unit.UNITS, Enums.Type.WATER_LEVEL),
                    new Reading(0, Enums.Unit.UNITS, Enums.Type.SOIL_MOISTURE),
                    26,
                    new Actuator(Enums.Type.FAN),
                    new Actuator(Enums.Type.LIGHT),
                    false
                    );
            }
        }

        public GeoLocationSubsystem GeoLocationSubsystem
        {
            get
            {
                return geoLocationSubsystem ??= new GeoLocationSubsystem(
                    new Reading(0, Enums.Unit.UNITS, Enums.Type.VIBRATION),
                    new Reading(0, Enums.Unit.DEGREES, Enums.Type.PITCH),
                    new Reading(0, Enums.Unit.DEGREES, Enums.Type.ROLL),
                    new Reading(45.406562, Enums.Unit.DEGREES, Enums.Type.LATITUDE),
                    new Reading(-73.943080, Enums.Unit.DEGREES, Enums.Type.LONGITUDE),
                    new Reading(0, Enums.Unit.BOOLEAN, Enums.Type.BUZZER)
                    );
            }
        }

        public SecuritySubsystem SecuritySubsystem
        {
            get
            {
                return securitySubsystem ??= new SecuritySubsystem(
                  new Reading(0, Enums.Unit.LUMINOSITY, Enums.Type.LUMINANCE),
                  new Reading(0, Enums.Unit.MOTION, Enums.Unit.MOTION),
                  new Reading(0, Enums.Unit.BOOLEAN, Enums.Type.DOOR_STATE),
                  new Reading(0, Enums.Unit.NONE, Enums.Type.NOISE),
                  new Actuator(Enums.Type.BUZZER),
                  new Actuator(Enums.Type.DOOR_LOCK)
                  );
            }
        }


    }
}
