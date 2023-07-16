from interfaces.actuators import IActuator, ACommand
from interfaces.sensors import ISensor, AReading

import seeed_python_reterminal.core as rt


class Buzzer(IActuator, ISensor):
    def __init__(self, model: str, type: ACommand.Type, initial_state: dict) -> None:
        self._model = model
        self.type = type
        self._current_state = initial_state

    def validate_command(self, command: ACommand) -> bool:
        # check if type is BUZZER and true or false
        return command.target_type == self.type and\
            (command.data["value"].lower() == "true" or
             command.data["value"].lower() == "false")

    def read_sensor(self) -> list[AReading]:
        return [AReading(AReading.Type.BUZZER, AReading.Unit.BOOLEAN, rt.buzzer)]

    def control_actuator(self, data: dict) -> bool:
        rt.buzzer = True if data['value'].lower() == "true" else False

        if self._current_state['value'] != data['value']:
            self._current_state['value'] = data['value']
            return True
        return False
