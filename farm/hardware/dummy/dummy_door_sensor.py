from interfaces.sensors import ISensor, AReading
from gpiozero import Button
import random as r

class DummyDoorSensor(ISensor):
    _sensor_model: str
    reading_type: AReading.Type

    def __init__(self, gpio: int,  model: str, type: AReading.Type):
        """Constructor for Door Sensor class.
        :param str model: specific model of sensor hardware. Ex. Grove - Adjustable PIR Motion Sensor
        :param ReadingType type: Type of reading this sensor produces. Ex. 'MOTION'
        """
        self._gpio = gpio
        self._sensor_model = model
        self.reading_type = type
        self.unit = AReading.Unit.NONE
        self._sensor = Button(gpio)

    def read_sensor(self) -> list[AReading]:
        """Takes a reading form the sensor
        :return list[AReading]: List of readings measured by the sensor. if Value is 1 then door is open
        0 is closed
        """
        readings = []
        if r.uniform(0,1) < 0.4:
                readings.append(AReading(self.reading_type, self.unit, 0))
        else:
            readings.append(AReading(self.reading_type, self.unit, 1))
        
        return readings
