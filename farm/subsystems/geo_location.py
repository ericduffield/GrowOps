
from hardware.geolocation.gps import Gps
from hardware.geolocation.buzzer import Buzzer
from hardware.geolocation.accelerometer import Accelerometer
from interfaces.sensors import ISensor, AReading
from interfaces.actuators import IActuator, ACommand
from interfaces.subsystems import ISubsystem

from time import sleep

class GeoLocation(ISubsystem):

    def __init__(self, sensors: list[ISensor], actuators: list[IActuator]) -> None:
        self.sensors = sensors
        self.actuators = actuators

    def read_sensors(self) -> list[AReading]:
        readings: list[AReading] = []

        for sensor in self.sensors:
            sensor_readings = sensor.read_sensor()
            for reading in sensor_readings:
                readings.append(reading)

        return readings

    def control_actuator(self, command: ACommand) -> None:
        for actuator in self.actuators:
            if (actuator.validate_command(command)):
                actuator.control_actuator(command.data)
            else:
                print("Invalid command for actuator")


# Used for testing
if __name__ == "__main__":
    TEST_SLEEP_TIME = 1

    buzzer = Buzzer("ReTerminal Buzzer",
                    ACommand.Type.BUZZER, {"value": "False"})
    accelerometer = Accelerometer("ReTerminal Accelerometer")
    gps = Gps("Air530")

    sensors: list[ISensor] = [
        buzzer,
        accelerometer,
        gps
    ]

    actuators: list[IActuator] = [
        buzzer
    ]

    geo_location = GeoLocation(sensors, actuators)

    while True:
        print(geo_location.read_sensors())
        fake_buzzer_message_body = '{"value": "True"}'
        fake_buzzer_command = ACommand(
            ACommand.Type.BUZZER, fake_buzzer_message_body)

        geo_location.control_actuator(fake_buzzer_command)

        sleep(TEST_SLEEP_TIME)
        print(geo_location.read_sensors())

        fake_buzzer_message_body = '{"value": "False"}'
        fake_buzzer_command = ACommand(
            ACommand.Type.BUZZER, fake_buzzer_message_body)

        geo_location.control_actuator(fake_buzzer_command)

        sleep(TEST_SLEEP_TIME)
