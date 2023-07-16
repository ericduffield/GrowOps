from abc import ABC, abstractmethod

from interfaces.sensors import ISensor, AReading
from interfaces.actuators import IActuator, ACommand


class ISubsystem(ABC):

    actuators: list[IActuator]
    sensors: list[ISensor]
    @abstractmethod
    def __init__(self, sensors: list[ISensor], actuators: list[IActuator]) -> None:
        """Creates a subsystem to manage a group of sensors and actuators.
        :param list[ISensor] sensors: List of sensors to be read.
        :param list[IActuator] actuators: List of actuators to be controlled.
        """

    @abstractmethod
    def read_sensors(self) -> list[AReading]:
        """Reads data from all sensors. 
        :return list[AReading]: a list containing all readings collected from sensors.
        """
        pass

    @abstractmethod
    def control_actuator(self, command: ACommand) -> None:
        """Controls actuators according to a command.
        :param ACommand command: the command to be dispatched to the corresponding actuator.
        """
