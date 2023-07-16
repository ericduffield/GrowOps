from grove.adc import ADC
from typing import List
from time import sleep

from interfaces.sensors import AReading, ISensor


class SoilMoistureSensor(ISensor):
    """Concrete class for soil moisture sensor. Inherits from ISensor interface."""

    def __init__(self, model: str, channel: int):
        self.channel = channel
        self.adc = ADC(0x04)
        self._model = model

    def read_sensor(self) -> List[AReading]:
        moisture = self.adc.read_raw(self.channel)
        return [AReading(AReading.Type.SOIL_MOISTURE, AReading.Unit.VOLTAGE, moisture)]


if __name__ == "__main__":
    channel = (
        # Set the channel number according to your setup (e.g., A0, A1, A2, etc.)
        2
    )

    sensor = SoilMoistureSensor("wow", channel)
    while True:
        readings = sensor.read_sensor()
        for reading in readings:
            print(reading)
        sleep(2)
