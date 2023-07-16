from grove.grove_temperature_humidity_aht20 import GroveTemperatureHumidityAHT20
from typing import List, Tuple
from time import sleep

from interfaces.sensors import AReading, ISensor


class TempHumiSensor(ISensor):
    """Concrete class for temperature and humidity sensor. Inherits from ISensor interface."""

    def __init__(self, model: str, bus: int):
        self.sensor = GroveTemperatureHumidityAHT20(bus=bus)
        self._model = model

    def read_sensor(self) -> List[AReading]:
        temperature, humidity = self.sensor.read()
        return [
            AReading(AReading.Type.TEMPERATURE,
                     AReading.Unit.CELCIUS, temperature),
            AReading(AReading.Type.HUMIDITY, AReading.Unit.HUMIDITY, humidity),
        ]


if __name__ == "__main__":
    model = "temp_and_humi"
    sensor = TempHumiSensor(model, gpio=4)
    while True:
        readings = sensor.read_sensor()
        for reading in readings:
            print(reading)
        sleep(2)
