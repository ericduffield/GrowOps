from gpiozero import Servo
from interfaces.actuators import ACommand, IActuator

class DummyDoorLock(IActuator):
    _current_state: dict
    type: ACommand.Type

    def __init__(self, gpio: int, type: ACommand.Type, initial_state: dict) -> None:
        """Constructor for Door Lock class. Instantiates a new door lock
        object that can be used to control a door lock servo
        :param ACommand.Type type: Type of command the actuator can respond to.
        :param dict initial_state: initializes 'current_state' property of a new actuator.
        Should minimally include the key "value" with corresponding value of initial state
        """
        self._gpio = gpio
        self.type = type
        if not 'value' in initial_state:
            raise Exception('initial_state must have a key for \'value\'')
        self._current_state = initial_state
        self._actuator = Servo(gpio)
        self._actuator.min()

    def validate_command(self, command: ACommand) -> bool:
        """Validates that a command can be used with the door lock servo.
        :param ACommand command: the command to be validated.
        :return bool: True if command can be consumed by the servo.
        """
        return command.target_type == self.type

    def control_actuator(self, data: dict) -> bool:
        """Sets the actuator to the value passed as argument.
        :param dict value: dictionary containing keys and values with command information.
        Must contain a key 'value' to be consumable by the actuator
        :return bool: True if the state of the actuator changed, false otherwise.
        """
        try:
            if self._current_state['value'] == data['value']:
                return False
            self._current_state = data
            if self._current_state['value'] != 0:
                self._actuator.max()
            else:
                self._actuator.min()
            return True
        except:
            return False
        
