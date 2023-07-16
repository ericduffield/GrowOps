from hardware.plant.temp_humi_sensor import TempHumiSensor
from hardware.plant.soil_moisture import SoilMoistureSensor
from hardware.plant.water_level import WaterLevelSensor
from hardware.plant.fan import FanController
from hardware.plant.led_strip import LEDStrip
from time import sleep
from interfaces.sensors import ISensor, AReading
from interfaces.actuators import IActuator, ACommand
from interfaces.subsystems import ISubsystem


class Plant(ISubsystem):
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
            if actuator.validate_command(command):
                actuator.control_actuator(command.data)


# Used for testing
if __name__ == "__main__":
    TEST_SLEEP_TIME = 2

    sensors: list[ISensor] = [
        # TempHumiSensor("Temp and Humi", 4),
        SoilMoistureSensor("Soil Moisture", 2),
        WaterLevelSensor("Water Level", 0),
    ]
    actuators: list[IActuator] = [
        FanController("ReTerminal Buzzer", ACommand.Type.FAN,
                      {"value": "off"}, 16),
        LEDStrip("LED strip", ACommand.Type.LIGHT_PULSE,
                 {"value": "off"}, 12, 10, 255),
    ]

    plant = Plant(sensors, actuators)

    while True:
        print(plant.read_sensors())
        # sleep(TEST_SLEEP_TIME)
        # Test FanController
        fake_fan_command_on = ACommand(ACommand.Type.FAN, '{"value": "on"}')
        plant.control_actuator(fake_fan_command_on)
        sleep(TEST_SLEEP_TIME)

        fake_fan_command_off = ACommand(ACommand.Type.FAN, '{"value": "off"}')
        plant.control_actuator(fake_fan_command_off)
        sleep(TEST_SLEEP_TIME)

        # Test LEDStrip
        fake_led_strip_command_on = ACommand(
            ACommand.Type.LIGHT_PULSE, '{"value": "on"}'
        )
        plant.control_actuator(fake_led_strip_command_on)
        sleep(TEST_SLEEP_TIME)

        fake_led_strip_command_rainbow = ACommand(
            ACommand.Type.LIGHT_PULSE, '{"value": "rainbow"}'
        )
        plant.control_actuator(fake_led_strip_command_rainbow)
        sleep(TEST_SLEEP_TIME)

        fake_led_strip_command_off = ACommand(
            ACommand.Type.LIGHT_PULSE, '{"value": "off"}'
        )
        plant.control_actuator(fake_led_strip_command_off)
        sleep(TEST_SLEEP_TIME)
