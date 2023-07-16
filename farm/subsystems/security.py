from interfaces.actuators import ACommand, IActuator
from interfaces.sensors import AReading, ISensor
from interfaces.subsystems import ISubsystem

from hardware.security.luminosity import LuminanceSensor
from hardware.security.noise import NoiseDetector
from hardware.security.door_sensor import DoorSensor
from interfaces.sensors import AReading
from interfaces.actuators import ACommand
import time
from hardware.security.door_lock import DoorLock

class Security(ISubsystem):
    sensors: list[ISensor]
    actuators: list[IActuator]
    def __init__(self, sensors: list[ISensor], actuators: list[IActuator]) -> None:
        """Creates a subsystem to manage a group of sensors and actuators.
        :param list[ISensor] sensors: List of sensors to be read.
        :param list[IActuator] actuators: List of actuators to be controlled.
        """
        self.sensors = sensors
        self.actuators = actuators

    def read_sensors(self) -> list[AReading]:
        """Reads data from all sensors. 
        :return list[AReading]: a list containing all readings collected from sensors.
        """
        readings: list[AReading] = []
        
        for sensor in self.sensors:
            sensor_readings = sensor.read_sensor()
            for reading in sensor_readings:
                readings.append(reading)
                
        return readings

    def control_actuator(self, command: ACommand, new_state: dict) -> None:
        """Controls actuators according to a command.
        :param ACommand command: the command to be dispatched to the corresponding actuator.
        :param dict new_state: the new state of the actuator
        """
        for actuator in self.actuators:
            if actuator.validate_command(command):
                actuator.control_actuator(new_state)
        


if __name__ == '__main__':
    
    lock = DoorLock(12, ACommand.Type.LOCK, {'value': 'False'})
    door_sensor = DoorSensor(16, 'model', AReading.Type.DOOR)
    noise = NoiseDetector("model", AReading.Type.NOISE, 0, 1)
    lumi_sensor = LuminanceSensor("model", AReading.Type.LUMINANCE)
    while True:
        print(f"Lock is: {lock._current_state['value']}")
        time.sleep(0.5)
        print("About to lock the door...")
        lock.control_actuator({'value': 'True'})
        print(f"Lock is: {lock._current_state['value']}")
        
        time.sleep(0.5)
        
        print("About to unlock the door...")
        lock.control_actuator({'value': 'False'})
        print(f"Lock is now: {lock._current_state['value']}")
        
        time.sleep(0.5)
        
        if door_sensor.read_sensor()[0].value == 0:
            x = 'Closed'
        else:
            x = 'Open'
        print(f"Door status: {x}")
        
        readings = noise.read_sensor()
        for reading in readings:
            print(f"{reading}")
            
        lumi_readings = lumi_sensor.read_sensor()
        for r in lumi_readings:
            print(f"{r}")
            
        time.sleep(1)