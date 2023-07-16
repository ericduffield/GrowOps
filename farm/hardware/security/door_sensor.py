from interfaces.sensors import ISensor, AReading
from gpiozero import Button

class DoorSensor(ISensor):
    _sensor_model: str
    reading_type: AReading.Type

    def __init__(self, gpio: int,  model: str, type: AReading.Type):
        """Constructor for Motion Sensor class.
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
        if self._sensor.is_active:
            readings.append(
                AReading(self.reading_type, self.unit, 0))
        else:
            readings.append(AReading(self.reading_type, self.unit, 1))
        
        return readings
