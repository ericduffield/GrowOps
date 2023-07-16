import time
import asyncio
from connection_manager import ConnectionManager
from device_controller import DeviceController
from hardware.dummy.dummy_door_sensor import DummyDoorSensor
from hardware.dummy.dummy_door_lock import DummyDoorLock


from connection_manager import ConnectionManager


from subsystems.plant import Plant
from subsystems.security import Security


from interfaces.sensors import ISensor, AReading
from interfaces.actuators import IActuator, ACommand


from hardware.plant.fan import FanController
from hardware.plant.led_strip import LEDStrip
from hardware.plant.soil_moisture import SoilMoistureSensor
from hardware.plant.temp_humi_sensor import TempHumiSensor
from hardware.plant.water_level import WaterLevelSensor


from hardware.security.door_lock import DoorLock
from hardware.security.luminosity import LuminanceSensor
from hardware.security.noise import NoiseDetector
from hardware.security.door_sensor import DoorSensor
from hardware.security.motion_sensor import MotionSensor


from hardware.geolocation.accelerometer import Accelerometer
from hardware.geolocation.buzzer import Buzzer
from hardware.geolocation.gps import Gps


class Farm:

    DEBUG = True

    def __init__(self) -> None:

        self._connection_manager = ConnectionManager()

        fan = FanController("fan", ACommand.Type.FAN, {"value": "False"}, 16)
        led_strip = LEDStrip("wow", ACommand.Type.LIGHT_PULSE, {
                             "value": "False"}, 18, 10, 255)
        soil_moisture = SoilMoistureSensor("moisture", 2)
        temp_humi = TempHumiSensor("temp_humi", 4)
        water_level = WaterLevelSensor("water_level", 6)

        plant_sensors: list[ISensor] = [
            soil_moisture,
            temp_humi,
            water_level
        ]

        plant_actuators: list[IActuator] = [
            fan,
            led_strip
        ]
        self._plant_subsystem = Plant(plant_sensors, plant_actuators)

        lock = DoorLock(12, ACommand.Type.LOCK, {'value': 0})
        door_sensor = DoorSensor(24, 'model', AReading.Type.DOOR)
        noise = NoiseDetector("model", AReading.Type.NOISE, 0, 1)
        lumi_sensor = LuminanceSensor("model", AReading.Type.LUMINANCE)
        motion_sensor = MotionSensor(5, "model", AReading.Type.MOTION)

        security_sensors: list[ISensor] = [
            door_sensor,
            noise,
            lumi_sensor,
            motion_sensor
        ]

        security_actuators: list[IActuator] = [
            lock
        ]

        self._security_subsystem = Security(
            security_sensors, security_actuators)

        buzzer = Buzzer("ReTerminal Buzzer",
                        ACommand.Type.BUZZER, {"value": "False"})
        accelerometer = Accelerometer("ReTerminal Accelerometer")
        gps = Gps("Air530")

        geo_location_sensors: list[ISensor] = [
            buzzer,
            accelerometer,
            gps
        ]

        geo_location_actuators: list[IActuator] = [
            buzzer
        ]

        self._geo_location_subsystem = Security(
            geo_location_sensors, geo_location_actuators)

        self._device_manager = DeviceController(
            [self._plant_subsystem, self._security_subsystem, self._geo_location_subsystem])

    async def loop(self) -> None:
        """Main loop of the farm System. Collect new readings, send them to connection
        manager, collect new commands and dispatch them to device manager.
        """

        await self._connection_manager.connect()
        self._connection_manager.register_command_callback(
            self._device_manager.control_actuator)

        state = True

        while True:
            try:
                # Collect new readings
                readings = self._device_manager.read_sensors()
                if self.DEBUG:
                    print(readings)

                    # test_actuators(self._device_manager, state)
                    state = not state

                # sleep
                await asyncio.sleep(self._connection_manager.telemetry_interval)

                # send Collected readings
                await self._connection_manager.send_readings(readings)
            except:
                test_actuators(self._device_manager, False)
                quit()


"""This script is intented to be used as a module, however, code below can be used for testing.
"""


def test_actuators(device_manager: DeviceController, state: bool):
    if (state):
        device_manager.control_actuator(ACommand(
            ACommand.Type.LOCK, '{"value": "True"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.BUZZER, '{"value": "True"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.FAN, '{"value": "True"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.LIGHT_PULSE, '{"value": "True"}'))

        # device_manager.control_actuator(ACommand(
        #     ACommand.Type.LIGHT_PULSE, '{"value": "rainbow"}'))
    else:
        device_manager.control_actuator(ACommand(
            ACommand.Type.LOCK, '{"value": "False"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.BUZZER, '{"value": "False"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.FAN, '{"value": "False"}'))

        device_manager.control_actuator(ACommand(
            ACommand.Type.LIGHT_PULSE, '{"value": "False"}'))


async def farm_main():
    farm = Farm()
    await farm.loop()


if __name__ == "__main__":
    asyncio.run(farm_main())
