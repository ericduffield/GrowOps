from gpiozero import OutputDevice
from time import sleep
from interfaces.actuators import IActuator, ACommand


class FanController(IActuator):
    _current_state: dict
    type: ACommand.Type = ACommand.Type.FAN

    def __init__(
        self,
        model: str,
        type: ACommand.Type,
        initial_state: dict,
        gpio: int,
    ) -> None:
        self.fan = OutputDevice(gpio)
        self.fan.off()
        self._current_state = initial_state
        self._model = model
        self.type = type

    def validate_command(self, command: ACommand) -> bool:
        return command.target_type == self.type

    def control_actuator(self, data: dict) -> bool:
        value = data["value"]
        if value.lower() == "true":
            self.fan.on()
            self._current_state["value"] = "on"
            return True
        elif value.lower() == "false":
            self.fan.off()
            self._current_state["value"] = "off"
            return True
        return False

    def getState(self) -> str:
        return str(self._current_state["value"])


if __name__ == "__main__":
    model = "fan"
    initial_state = {"value": "off"}
    fan_controller = FanController(model, ACommand.Type.FAN, initial_state, 16)

    while True:
        current_state = fan_controller.getState()
        print(f"Current state: {current_state}")

        if current_state == "off":
            print("Turning on")
            fan_controller.control_actuator({"value": "on"})
        else:
            print("Turning off")
            fan_controller.control_actuator({"value": "off"})

        sleep(4)
