from time import sleep
from hardware.dummy.dummy_door_lock import DummyDoorLock
from hardware.dummy.dummy_door_sensor import DummyDoorSensor
from hardware.security.luminosity import LuminanceSensor
from hardware.security.noise import NoiseDetector
from subsystems.security import Security
from interfaces.subsystems import ISubsystem

from interfaces.sensors import ISensor, AReading
from interfaces.actuators import IActuator, ACommand


class DeviceController:

    def __init__(self, subsystems: list[ISubsystem]) -> None:
        """DeviceController constructor to manage a group of sensors and actuators.
        :param list[ISensor] sensors: List of sensors to be read.
        :param list[IActuator] actuators: List of actuators to be controlled.
        """
        self._sensors: list[ISensor] = []
        self._actuators: list[IActuator] = []

        for subsystem in subsystems:
            for actuator in subsystem.actuators:
                self._actuators.append(actuator)
            for sensor in subsystem.sensors:
                self._sensors.append(sensor)

    def read_sensors(self) -> list[AReading]:
        """Reads data from all sensors. 
        :return list[AReading]: a list containing all readings collected from sensors.
        """
        readings: list[AReading] = []
        for sen in self._sensors:
            sensor_readings = sen.read_sensor()
            for i in sensor_readings:
                readings.append(i)

        return readings

    def control_actuator(self, command: ACommand) -> None:
        """Controls actuators according to a command.
        :param ACommand command: the command to be dispatched to the corresponding actuator.
        """
        print(str(command))
        for actuator in self._actuators:
            if (actuator.validate_command(command)):
                actuator.control_actuator(command.data)


"""This script is intented to be used as a module, however, code below can be used for testing.
"""

if __name__ == "__main__":

    TEST_SLEEP_TIME = 2

    lock = DummyDoorLock(12, ACommand.Type.LOCK, {'value': 0})
    door_sensor = DummyDoorSensor(16, 'model', AReading.Type.DOOR)
    noise = NoiseDetector("model", AReading.Type.NOISE, 0, 1)
    lumi_sensor = LuminanceSensor("model", AReading.Type.LUMINANCE)

    security_sensors: list[ISensor] = [
        door_sensor,
        noise,
        lumi_sensor
    ]

    security_actuators: list[IActuator] = [
        lock
    ]

    security_subsystem = Security(security_sensors, security_actuators)
    device_manager = DeviceController([security_subsystem])

    while True:
        # Message body should minimally include a key named "value" with
        # a value that is parsable inside the specific actuator.
        fake_led_message_body = '{"value": 1.5}'
        fake_led_command = ACommand(ACommand.Type.LIGHT_PULSE,
                                    fake_led_message_body)

        fake_fan_message_body = '{"value": "on"}'
        fake_fan_command = ACommand(ACommand.Type.FAN, fake_fan_message_body)

        device_manager.control_actuator(fake_led_command)
        device_manager.control_actuator(fake_fan_command)

        sleep(TEST_SLEEP_TIME)