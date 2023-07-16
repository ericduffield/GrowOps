import os
from time import sleep
from rpi_ws281x import Color
from interfaces.actuators import ACommand, IActuator
from grove.grove_ws2813_rgb_led_strip import GroveWS2813RgbStrip


class LEDStrip(IActuator):
    _current_state: dict
    type: ACommand.Type = ACommand.Type.LIGHT_PULSE

    def __init__(
        self,
        model: str,
        type: ACommand.Type,
        initial_state: dict,
        pin: int,
        count: int,
        brightness: int,
    ) -> None:
        self._model = model
        self.type = type
        self._current_state = initial_state
        self.strip = GroveWS2813RgbStrip(pin, count, brightness)
        self.control_actuator(self._current_state)

    def validate_command(self, command: ACommand) -> bool:
        return command.target_type == self.type

    def control_actuator(self, data: dict) -> bool:
        value = data["value"]
        if value.lower() == "true":
            self.turn_on_white()
            self._current_state["value"] = "true"
            return True
        elif value.lower() == "false":
            self.turn_off()
            self._current_state["value"] = "false"
            return True
        elif value.lower() == "rainbow":
            self.make_rainbow(iterations=1)
            return True

    def turn_on_white(self):
        for i in range(self.strip.numPixels()):
            self.strip.setPixelColor(i, Color(255, 255, 255))
        self.strip.show()

    def turn_off(self):
        for i in range(self.strip.numPixels()):
            self.strip.setPixelColor(i, Color(0, 0, 0))
        self.strip.show()

    def wheel(self, pos):
        """Generate rainbow colors across 0-255 positions."""
        if pos < 85:
            return Color(pos * 3, 255 - pos * 3, 0)
        elif pos < 170:
            pos -= 85
            return Color(255 - pos * 3, 0, pos * 3)
        else:
            pos -= 170
            return Color(0, pos * 3, 255 - pos * 3)

    def make_rainbow(self, wait_ms=20, iterations=1):
        """Draw rainbow that fades across all pixels at once."""
        for j in range(256 * iterations):
            for i in range(self.strip.numPixels()):
                self.strip.setPixelColor(i, self.wheel((i + j) & 255))
            self.strip.show()
            sleep(wait_ms / 1000.0)


if __name__ == "__main__":
    pin = 12
    count = 10
    model = "wow"
    type = ACommand.Type.LIGHT_PULSE
    initial_state: dict = {"value": "off"}
    brightness = 255
    led_strip = LEDStrip(model, type, initial_state, pin, count, brightness)
    while True:
        print("making white")
        led_strip.control_actuator({"value": "on"})
        sleep(4)
        print("making rainbow")
        led_strip.control_actuator({"value": "rainbow"})
        sleep(4)
        print("turning off")
        led_strip.control_actuator({"value": "off"})
        sleep(4)
