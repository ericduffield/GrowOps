import seeed_python_reterminal.core as rt
from interfaces.sensors import AReading, ISensor


class LuminanceSensor(ISensor):
    _sensor_model: str
    reading_type: AReading.Type

    def __init__(self, model: str, type: AReading.Type):
        """Constructor for Sensor  class. May be called from childclass.
        :param str model: specific model of sensor hardware. Ex. AHT20 or LTR-303ALS-01
        :param ReadingType type: Type of reading this sensor produces. Ex. 'LUMINANCE'
        """
        self.model = model
        self.reading_type = type
        self.unit = AReading.Unit.LUMINANCE
        self._sensor = rt

    def read_sensor(self) -> list[AReading]:
        """Takes a reading form the sensor
        :return list[AReading]: List of readings measured by the sensor. Returns a list
        with the current luminance reading
        """
        readings: list[AReading] = []
        
        readings.append(AReading(self.reading_type, self.unit, self._sensor.illuminance))
        
        return readings
        
