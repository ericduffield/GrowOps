from interfaces.actuators import ACommand, IActuator

class DummyLed(IActuator):
    _current_state: dict
    type: ACommand.Type

    def __init__(self, gpio: int, type: ACommand.Type, initial_state: dict) -> None:
        """Constructor for Actuator class. Must define interface's class properties
        :param ACommand.Type type: Type of command the actuator can respond to.
        :param dict initial_state: initializes 'current_state' property of a new actuator.
        Should minimally include the key "value" with corresponding value of initial state
        """
        self.gpio = gpio
        self.type = type
        self._current_state = initial_state

    def validate_command(self, command: ACommand) -> bool:
        """Validates that the command is the same type as the Actuator
        :param ACommand command: the command to be validated.
        :return bool: True if command can be consumed by the actuator.
        """
        return command.Type == self.type

    def control_actuator(self, data: dict) -> bool:
        """Sets the actuator to the value passed as argument.
        :param dict value: dictionary containing keys and values with command information.
        :return bool: True if the state of the actuator changed, false otherwise.
        """
        if self._current_state['value'] == data['value']:
            return False
        
        self._current_state = data
        return True