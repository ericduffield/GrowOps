from interfaces.sensors import AReading, ISensor
import random as r


class DummyHumiditySensor(ISensor):

    # Properties to be initialized in constructor of implementation classes
    _sensor_model: str
    reading_type: AReading.Type

    def __init__(self, gpio: int, model: str, type: AReading.Type):
        """Constructor for Sensor  class. May be called from childclass.
        :param str model: specific model of sensor hardware. Ex. AHT20 or LTR-303ALS-01
        :param ReadingType type: Type of reading this sensor produces. Ex. 'TEMPERATURE'
        """
        self.gpio = gpio
        self._sensor_model = model
        self.reading_type = type
        self.unit = AReading.Unit.HUMIDITY

    def read_sensor(self) -> list[AReading]:
        """Takes a reading form the sensor
        :return list[AReading]: List of readinds measured by the sensor. Most sensors return a list with a single item.
        """
        a = AReading(self.reading_type, self.unit, r.uniform(0, 99))
        list = []
        list.append(a)
        return list